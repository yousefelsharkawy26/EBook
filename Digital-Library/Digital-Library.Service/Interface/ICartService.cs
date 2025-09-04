using Digital_Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Interface
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(string userId);

        Task AddItemAsync(string userId , CartDetail item);

        Task RemoveItemAsync(string userId , CartDetail item);

        Task UpdateQuantityAsync(string userId, string productId, int quantity);

        Task ClearCartAsync(string userId);
    }
}
