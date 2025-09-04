using Digital_Library.Core.Models;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;

namespace Digital_Library.Service.Implementation
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddItemAsync(string userId, CartDetail item)
        {
            var cart = await _unitOfWork.Carts.GetSingleAsync(c => c.UserId == userId , c => c.CartDetails);
            if (cart == null)
            {
                throw new KeyNotFoundException($"This user has no cart");
            }

            cart.CartDetails.Add(item);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetSingleAsync(c => c.UserId == userId, c => c.CartDetails);
            if (cart == null)
            {
                throw new KeyNotFoundException($"This user has no cart");
            }

            cart.CartDetails.Clear();

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Cart> GetCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetSingleAsync(c => c.UserId == userId, c => c.CartDetails);
            if (cart == null)
            {
                throw new KeyNotFoundException($"This user has no cart");
            }

            return cart;
        }

        public async Task RemoveItemAsync(string userId, CartDetail item)
        {
            var cart = await _unitOfWork.Carts.GetSingleAsync(c => c.UserId == userId, c => c.CartDetails);
            if (cart == null)
            {
                throw new KeyNotFoundException($"This user has no cart");
            }

            cart.CartDetails.Remove(item);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(string userId, string productId, int quantity)
        {
            var cart = await _unitOfWork.Carts.GetSingleAsync(c => c.UserId == userId, c => c.CartDetails);
            if (cart == null)
            {
                throw new KeyNotFoundException($"This user has no cart");
            }

            var cartDetail = cart.CartDetails.FirstOrDefault(d => d.Id == productId);

            if (cartDetail == null)
            {
                throw new KeyNotFoundException($"The cart doesn’t contain this item");

            }

            cartDetail.Quantity = quantity;
        }
    }
}
