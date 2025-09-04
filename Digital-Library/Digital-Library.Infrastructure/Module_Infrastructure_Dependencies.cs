using Digital_Library.Core.Models;
using Digital_Library.Infrastructure.Context;
using Digital_Library.Infrastructure.Repositories.Implementation;
using Digital_Library.Infrastructure.Repositories.Interface;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Digital_Library.Infrastructure
{
	public static class Module_Infrastructure_Dependencies
	{
		public static IServiceCollection Add_Module_Infrastructure_Dependencies(this IServiceCollection services)
		{
			#region Add Identity
			services.AddIdentity<User, IdentityRole>(option =>
			{
				// Password settings.
				option.Password.RequireDigit = false;
				option.Password.RequireLowercase = false;
				option.Password.RequireNonAlphanumeric = false;
				option.Password.RequireUppercase = false;
				option.Password.RequiredLength = 6;
				option.Password.RequiredUniqueChars = 1;

				// Lockout settings.
				option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				option.Lockout.MaxFailedAccessAttempts = 5;
				option.Lockout.AllowedForNewUsers = true;

				// User settings.
				option.User.AllowedUserNameCharacters =
				"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				option.User.RequireUniqueEmail = false;
				option.SignIn.RequireConfirmedEmail = false;

			}).AddEntityFrameworkStores<EBookContext>()
			  .AddDefaultTokenProviders();
			#endregion
			services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
			services.AddScoped<IUnitOfWork, UnitOfWork.Implementation.UnitOfWork>();
			return services;
		}
	}
}
