using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Requests
{
	public class UserRequest
	{
		public string? FullName { get; set; }

		public IFormFile? ImageProfile	{ get; set; }


	}
}
