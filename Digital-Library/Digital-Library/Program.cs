using Digital_Library.Core.Services;
using Digital_Library.Infrastructure;
using Digital_Library.Infrastructure.Context;
using Digital_Library.Service;
using Digital_Library.Service.Helpers;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Digital_Library.Service.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Digital_Library
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			#region AddDBContext
			builder.Services.AddDbContext<EBookContext>(option =>
			{
				option.UseSqlServer(builder.Configuration.GetConnectionString("DevConn"));
			});
			#endregion
			builder.Services.AddControllersWithViews()
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
				});


			builder.Services.AddControllersWithViews()
											.AddJsonOptions(options =>
											{
												options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
												options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
											});

			builder.Services.Configure<SecurityStampValidatorOptions>(options =>
			{
				options.ValidationInterval = TimeSpan.Zero;
			});

			builder.Services.AddSingleton(new VendorPdfEncryption(
				builder.Configuration["Encryption:PdfKey"]
));
			builder.Services.AddSingleton(new UserPdfEncryptionService());
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policy =>
				{
					policy
									.AllowAnyOrigin()
									.AllowAnyHeader()
									.AllowAnyMethod();
				});
			});

			var jwtSettings = builder.Configuration.GetSection("Jwt");
			var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

			builder.Services.AddAuthentication()
							.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
							{
								options.TokenValidationParameters = new TokenValidationParameters
								{
									ValidateIssuer = true,
									ValidateAudience = true,
									ValidateLifetime = true,
									ValidateIssuerSigningKey = true,
									ValidIssuer = builder.Configuration["Jwt:Issuer"],
									ValidAudience = builder.Configuration["Jwt:Audience"],
									IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
								};
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
			app.UseCors("AllowAll");
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
