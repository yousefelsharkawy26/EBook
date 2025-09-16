using Digital_Library.Core.Services;
using Digital_Library.Infrastructure;
using Digital_Library.Infrastructure.Context;
using Digital_Library.Service;
using Digital_Library.Service.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Digital_Library.AdminPanel
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDbContext<EBookContext>(option =>
			{
				option.UseSqlServer(builder.Configuration.GetConnectionString("DevConn"));
			});

			builder.Services.AddHttpClient();
			#region Dependency injections

			builder.Services.Add_Module_Infrastructure_Dependencies()
																												.Add_Module_Service_Dependencies()
																												.Add_Module_Configuration_Services(builder.Configuration);

			#endregion
			// Add services to the container.
			builder.Services.AddControllersWithViews()
																			.AddJsonOptions(options =>
																			{
																				options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
																				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
																			});
			builder.Services.AddSingleton(new VendorPdfEncryption(
	builder.Configuration["Encryption:PdfKey"]
));
			builder.Services.AddSingleton(new UserPdfEncryptionService());
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

			app.Run();
		}
	}
}
