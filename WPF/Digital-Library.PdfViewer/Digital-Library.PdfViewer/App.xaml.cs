using Digital_Library.PdfViewer.Services;
using Digital_Library.PdfViewer.ViewModels;
using Digital_Library.PdfViewer.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Digital_Library.PdfViewer
{

    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .Build();
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context, services);
                })
                .Build();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHttpClient("E-Book Client" ,
                options =>
                {
                    options.BaseAddress = new Uri($"{context.Configuration["BaseURL"]!}api/Client/");
                });

            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<LoginWindow>();
            services.AddTransient<MyBooksViewModel>();
            services.AddTransient<MyBooksWindow>();

            services.AddSingleton<IBookService, BookService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IKeyManagementService, KeyManagementService>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var loginWindow = _host.Services.GetRequiredService<LoginWindow>();

            loginWindow.Show();
            base.OnStartup(e);
        }
    }
}



