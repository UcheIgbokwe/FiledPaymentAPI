using System;
using System.Threading.Tasks;
using Application.Repositories.Interfaces;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Repositories
{
    public class PaymentRepository : IPayment
    {
        private readonly IServiceScope _scope;
        private readonly ILogger<PaymentRepository> _logger;
        private readonly DataContext _databaseContext;
        public PaymentRepository(IServiceProvider services, ILogger<PaymentRepository> logger)
        {
            _logger = logger;
            _scope = services.CreateScope();
            _databaseContext = _scope.ServiceProvider.GetRequiredService<DataContext>();
        }
        public async Task<bool> Process(Payment payment)
        {
            int reprocessTime = 0;
            try
            {
                if(payment.Amount < 20)
                {
                    return await ProcessCheap(payment).ConfigureAwait(false);
                }

                bool isExpensive = IExpensivePaymentGateway(payment.Amount);
                if(isExpensive)
                {
                   var status = await SaveToDb(payment).ConfigureAwait(false);
                   if(!status)
                    status = await ProcessCheap(payment).ConfigureAwait(false);
                    reprocessTime ++;
                    return status;
                }

                return await ProcessPremium(payment).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return false;
        }

        private static bool IExpensivePaymentGateway(decimal amount)
        {
            return amount > 20 && amount < 501;
        }
        private static bool PremiumPaymentService(decimal amount)
        {
            return amount > 500;
        }

        private async Task<bool> SaveToDb(Payment payment)
        {
            var success = false;
            try
            {
                _databaseContext.Payments.Add(payment);
                var numberOfPaymentsCreated = await _databaseContext.SaveChangesAsync().ConfigureAwait(false);

                if(numberOfPaymentsCreated == 1)
                    success = true;
                    return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return success;
        }

        private async Task<bool> ProcessCheap(Payment payment)
        {
            return await SaveToDb(payment).ConfigureAwait(false);
        }
        private async Task<bool> ProcessPremium(Payment payment)
        {
            int reprocessTime = 0;
            var status = false;
            bool isPremium = PremiumPaymentService(payment.Amount);
            if(isPremium)
            {
                status = await SaveToDb(payment).ConfigureAwait(false);
                while(reprocessTime < 3 && !status)
                    status = await SaveToDb(payment).ConfigureAwait(false);
                    reprocessTime ++;

                return status;
            }
            return status;
        }
    }
}