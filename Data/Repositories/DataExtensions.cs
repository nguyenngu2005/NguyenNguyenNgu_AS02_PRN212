using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Repositories
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<FumhContext>(opt => opt.UseSqlServer(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
