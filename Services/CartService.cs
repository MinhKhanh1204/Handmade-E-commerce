using BussinessObject;
using Repositories;

namespace Services
{
    public class CartService
    {
        private readonly CartRepository _cartRepo;
        private readonly IProductRepository _productRepo;

        public CartService(CartRepository cartRepo, IProductRepository productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public Cart? GetCartByCustomerId(string customerId)
        {
            return _cartRepo.GetCartByCustomerId(customerId);
        }

        /// <summary>
        /// ✅ Add product to cart with validation
        /// </summary>
        public void AddToCart(string customerId, string productId, int quantity)
        {
            // ✅ Validate product exists
            var product = _productRepo.GetProductById(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found!");
            }

            if (product.Status != "Active")
            {
                throw new InvalidOperationException("Product is not available!");
            }

            // ✅ Validate stock quantity
            if ((product.StockQuantity ?? 0) < quantity)
            {
                throw new InvalidOperationException($"Only {product.StockQuantity} items available in stock!");
            }

            // Lấy cart hiện tại
            var cart = _cartRepo.GetCartByCustomerId(customerId);

            // Nếu chưa có cart, tạo mới
            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CartItems = new List<CartItem>()
                };
                _cartRepo.CreateCart(cart);

                // Reload cart
                cart = _cartRepo.GetCartByCustomerId(customerId);
            }

            // Kiểm tra sản phẩm đã có trong cart chưa
            var existingItem = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                // Kiểm tra tổng số lượng sau khi thêm
                int newQuantity = (existingItem.Quantity ?? 0) + quantity;

                if ((product.StockQuantity ?? 0) < newQuantity)
                {
                    throw new InvalidOperationException($"Cannot add {quantity} more items. Only {(product.StockQuantity ?? 0) - (existingItem.Quantity ?? 0)} items available!");
                }

                existingItem.Quantity = newQuantity;
                _cartRepo.UpdateCartItem(existingItem.CartItemId, existingItem.Quantity.Value);
            }
            else
            {
                // Thêm mới
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _cartRepo.AddCartItem(cartItem);
            }

            // Update timestamp
            _cartRepo.UpdateCartTimestamp(cart.CartId);
        }

        public void UpdateCartItem(int cartItemId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            // ✅ Validate stock
            var cartItem = _cartRepo.GetCartItemById(cartItemId);
            if (cartItem == null)
                throw new InvalidOperationException("Cart item not found!");

            var product = _productRepo.GetProductById(cartItem.ProductId);
            if (product == null)
                throw new InvalidOperationException("Product not found!");

            if ((product.StockQuantity ?? 0) < quantity)
                throw new InvalidOperationException($"Only {product.StockQuantity} items available!");

            _cartRepo.UpdateCartItem(cartItemId, quantity);
        }

        public void DeleteCartItem(int cartItemId)
        {
            _cartRepo.DeleteCartItem(cartItemId);
        }

        public void ClearCart(string customerId)
        {
            _cartRepo.ClearCart(customerId);
        }
    }
}