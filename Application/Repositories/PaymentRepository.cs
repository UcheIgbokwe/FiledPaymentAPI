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
            var success = false;
            try
            {
                _databaseContext.Payments.Add(payment);
                var numberOfPaymentsCreated = await _databaseContext.SaveChangesAsync().ConfigureAwait(false);

                if(numberOfPaymentsCreated == 1)
                    success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return success;
        }
    }
}