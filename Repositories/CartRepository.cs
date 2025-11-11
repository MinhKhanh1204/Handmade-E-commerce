using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class CartRepository
    {
        private readonly MyStoreContext _context;

        public CartRepository(MyStoreContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByCustomerIdAsync(string customerId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task UpdateCartItemAsync(int cartItemId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                item.Quantity = quantity;
                _context.CartItems.Update(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCartItemAsync(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string customerId)
        {
            var cart = await GetCartByCustomerIdAsync(customerId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }
        }
    }
}
