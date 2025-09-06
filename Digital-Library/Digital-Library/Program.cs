using Digital_Library.Infrastructure;
using Digital_Library.Infrastructure.Context;
using Digital_Library.Service;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Digital_Library.Service.Seed;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Digital_Library
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllersWithViews();
			#region AddDBContext
			builder.Services.AddDbContext<EBookContext>(option =>
			{
				option.UseSqlServer(builder.Configuration.GetConnectionString("DevConn"));
			});
            #endregion

            //add on detail cart
            builder.Services.AddControllersWithViews()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });


            #region Dependency injections

            builder.Services.Add_Module_Infrastructure_Dependencies()
.Add_Module_Service_Dependencies()
.Add_Module_Configuration_Services(builder.Configuration);

			#endregion

			var app = builder.Build();

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");

				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapStaticAssets();
			app.MapControllerRoute(
							name: "default",
							pattern: "{controller=Home}/{action=Index}/{id?}")
							.WithStaticAssets();
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var context = services.GetRequiredService<EBookContext>();
				//context.Database.Migrate();
				RoleSeeder.SeedRolesAsync(services).Wait();
			}

			app.Run();
		}
	}
}
