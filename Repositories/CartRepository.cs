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

        // ========== SYNCHRONOUS METHODS ==========

        /// <summary>
        /// ✅ Get cart by customer ID (sync)
        /// </summary>
        public Cart? GetCartByCustomerId(string customerId)
        {
            return _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefault(c => c.CustomerId == customerId);
        }

        /// <summary>
        /// ✅ Create new cart (sync)
        /// </summary>
        public void CreateCart(Cart cart)
        {
            _context.Carts.Add(cart);
            _context.SaveChanges();
        }

        /// <summary>
        /// ✅ Add cart item (sync)
        /// </summary>
        public void AddCartItem(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            _context.SaveChanges();
        }

        /// <summary>
        /// ✅ Update cart item quantity (sync)
        /// </summary>
        public void UpdateCartItem(int cartItemId, int quantity)
        {
            var item = _context.CartItems.Find(cartItemId);
            if (item != null)
            {
                item.Quantity = quantity;
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// ✅ Delete cart item (sync)
        /// </summary>
        public void DeleteCartItem(int cartItemId)
        {
            var item = _context.CartItems.Find(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// ✅ Clear all cart items (sync)
        /// </summary>
        public void ClearCart(string customerId)
        {
            var cart = GetCartByCustomerId(customerId);
            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// ✅ Get cart item by ID (sync)
        /// </summary>
        public CartItem? GetCartItemById(int cartItemId)
        {
            return _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefault(ci => ci.CartItemId == cartItemId);
        }

        /// <summary>
        /// ✅ Update cart timestamp (sync)
        /// </summary>
        public void UpdateCartTimestamp(int cartId)
        {
            var cart = _context.Carts.Find(cartId);
            if (cart != null)
            {
                cart.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }

        // ========== ASYNCHRONOUS METHODS (giữ lại cho các trường hợp cần) ==========

        public async Task<Cart?> GetCartByCustomerIdAsync(string customerId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task CreateCartAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(int cartItemId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                item.Quantity = quantity;
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
            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateCartTimestampAsync(int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart != null)
            {
                cart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}