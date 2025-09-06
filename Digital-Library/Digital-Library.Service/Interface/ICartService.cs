using Digital_Library.Core.Enums;
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
	public interface ICartService
	{
		Task<Response> GetCartAsync(string userId);

		Task<Response> AddItemAsync(string userId, CartDetailRequest request);

		Task<Response> RemoveItemAsync(string cartDetailId);

		Task<Response> UpdateCartAsync(string cartDetailId, int quantity);

		Task<Response> ClearCartAsync(string userId);
	}

}
