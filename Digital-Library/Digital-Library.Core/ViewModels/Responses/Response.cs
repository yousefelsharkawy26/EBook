using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Responses
{
	public class Response
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public object? Data { get; set; }

		public static Response Ok(string message, object? data = null) =>
						new Response { Success = true, Message = message, Data = data };

		public static Response Fail(string message) =>
						new Response { Success = false, Message = message };
	}

}
