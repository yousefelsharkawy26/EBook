using Digital_Library.Core.Models;
using Digital_Library.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			}).AddEntityFrameworkStores<EBookContext>().AddDefaultTokenProviders();
			#endregion
			return services;
		}
	}
}
