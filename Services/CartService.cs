using BussinessObject;
using Repositories;

namespace Services
{
    public class CartService
    {
        private readonly CartRepository _cartRepo;

        public CartService(CartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

        /// <summary>
        /// Lấy giỏ hàng theo CustomerId (bao gồm CartItems, Product, Image)
        /// </summary>
        public async Task<Cart?> GetCartByCustomerIdAsync(string customerId)
        {
            return await _cartRepo.GetCartByCustomerIdAsync(customerId);
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm trong giỏ hàng
        /// </summary>
        public async Task UpdateCartItemAsync(int cartItemId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0");

            await _cartRepo.UpdateCartItemAsync(cartItemId, quantity);
        }

        /// <summary>
        /// Xóa một sản phẩm khỏi giỏ hàng
        /// </summary>
        public async Task DeleteCartItemAsync(int cartItemId)
        {
            await _cartRepo.DeleteCartItemAsync(cartItemId);
        }

        /// <summary>
        /// Xóa toàn bộ giỏ hàng của khách hàng
        /// </summary>
        public async Task ClearCartAsync(string customerId)
        {
            await _cartRepo.ClearCartAsync(customerId);
        }
    }
}
