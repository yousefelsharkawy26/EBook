using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Responses
{
	public class Response
	{
		public bool IsSuccess { get; set; }
		public string? Message { get; set; }
		public object? Data { get; set; }

	}
}
