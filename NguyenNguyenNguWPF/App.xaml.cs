using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Windows;
using Business;
using Data.Repositories;
using NguyenNguyenNguWPF.ViewModels;
using NguyenNguyenNguWPF.Views;

namespace NguyenNguyenNguWPF
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
                .ConfigureServices((ctx, services) =>
                {
                    var conn = ctx.Configuration.GetConnectionString("FUMH")!;
                    services.AddDataLayer(conn);
                    services.AddBusinessLayer();

                    services.AddTransient<LoginViewModel>();

                    services.AddTransient<LoginWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();
            var login = AppHost.Services.GetRequiredService<LoginWindow>();
            login.DataContext = AppHost.Services.GetRequiredService<LoginViewModel>();

            login.Show();
            base.OnStartup(e);
        }
    }
}