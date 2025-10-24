using Microsoft.Extensions.DependencyInjection;
using Business.Abstractions;
using Business.Services;  

namespace Business
{
    public static class BusinessExtensions
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IBookingService, BookingService>();   
            services.AddScoped<IReportService, ReportService>();
            return services;
        }
    }
}
