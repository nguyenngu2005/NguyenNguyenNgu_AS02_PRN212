using System.Threading.Tasks;

namespace Business.Abstractions
{
    public interface IAuthService
    {
        Task<(bool ok, string role, int? customerId, string? error)> LoginAsync(string email, string password);
    }
}
