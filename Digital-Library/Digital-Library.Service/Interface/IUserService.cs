using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Interface
{
	public interface IUserService
	{
		Task<Response> UpdateProfileAsync(string userId, UserRequest request);
		Task<Response> GetProfileAsync(string userId);
	}
}
