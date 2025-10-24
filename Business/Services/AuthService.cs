using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Data.Repositories;

namespace Business.Services
{
    public class AuthService : Abstractions.IAuthService
    {
        private readonly IConfiguration _cfg;
        private readonly IUnitOfWork _uow;

        public AuthService(IConfiguration cfg, IUnitOfWork uow)
        {
            _cfg = cfg; _uow = uow;
        }

        public async Task<(bool ok, string role, int? customerId, string? error)> LoginAsync(string email, string password)
        {
            var adminEmail = _cfg["AdminAccount:Email"];
            var adminPass = _cfg["AdminAccount:Password"];

            if (string.Equals(email, adminEmail, System.StringComparison.OrdinalIgnoreCase)
               && password == adminPass)
                return (true, "Admin", null, null);

            var cust = await _uow.Customers.FirstOrDefaultAsync(c => c.EmailAddress == email && c.Password == password);
            if (cust != null)
                return (true, "Customer", cust.CustomerID, null);

            return (false, "", null, "Invalid email or password.");
        }
    }
}
