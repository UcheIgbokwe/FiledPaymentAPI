using System.Threading.Tasks;
using Domain;

namespace Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> Process (Payment payment);
    }
}