using System.Threading.Tasks;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPayment _repository;

        public PaymentService(IPayment repository)
        {
            _repository = repository;
        }
        public async Task<Payment> Process(Payment payment)
        {
            var success = await _repository.Process(payment).ConfigureAwait(false);

            if(success)
                return payment;
            else
                return null;
        }
    }
}