using Digital_Library.Core.Constant;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Digital_Library.Service
{
	public static class Module_Service_Dependencies
	{
		public static IServiceCollection Add_Module_Service_Dependencies(this IServiceCollection services)
		{
			services.AddTransient<IFileService, FileService>();
			services.AddTransient<IEmailSender, EmailSender>();
			services.AddScoped<IBookService, BookService>();
			return services;
		}

		public static IServiceCollection Add_Module_Configuration_Services(this IServiceCollection services, IConfiguration configuration)
		{
            // 1. Configure the EmailSettings class with values from appsettings.json
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

			return services;
        }
	}
}
