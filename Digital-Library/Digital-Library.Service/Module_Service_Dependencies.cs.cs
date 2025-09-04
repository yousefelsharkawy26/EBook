using Digital_Library.Core.Constant;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Digital_Library.Service.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Digital_Library.Service
{
	public static class Module_Service_Dependencies
	{
		public static IServiceCollection Add_Module_Service_Dependencies(this IServiceCollection services)
		{
   services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IBookService, BookService>();
			services.AddScoped<IBorrowService, BorrowService>();
<<<<<<< HEAD
			services.AddScoped<ICartService, CartService>();
			services.AddScoped<ICategoryService, CategoryService>();
			services.AddTransient<IEmailSender, EmailSender>();
			services.AddTransient<IFileService, FileService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IVendorService, VendorService>();
			return services;

=======
			services.AddScoped<ICartService,     CartService>();
			services.AddScoped<IUserService,   UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IVendorService, VendorService>();
            return services;
>>>>>>> 30560cf55767ca06c42bc46c89ed2e31ba2b445b
		}

		public static IServiceCollection Add_Module_Configuration_Services(this IServiceCollection services, IConfiguration configuration)
		{
            // 1. Configure the EmailSettings class with values from appsettings.json
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

			return services;
        }
	}
}
