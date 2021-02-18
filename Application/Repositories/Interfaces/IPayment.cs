using System.Threading.Tasks;
using Domain;

namespace Application.Repositories.Interfaces
{
    public interface IPayment
    {
        Task<bool> Process (Payment payment);
    }
}