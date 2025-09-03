using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Digital_Library.Service
{
	public static class Module_Service_Dependencies
	{
		public static IServiceCollection Add_Module_Service_Dependencies(this IServiceCollection services)
		{
			services.AddTransient<IFileService, FileService>();
			return services;
		}
	}
}
