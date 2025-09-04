using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Requests
{
	public class CategoryRequest
	{
		[Required]
		public string CategoryName { get; set; }
		[Required]
		public string Description { get; set; }
	}
}
