using lab1.Models;
using lab1.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace lab1.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly OrderService _orderService; // Sử dụng OrderService thay vì IOrderRepository
        private const string CartSessionKey = "CartSession";

        public CartService(IHttpContextAccessor httpContextAccessor, OrderService orderService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        private List<CartItem> GetCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return new List<CartItem>();
            }

            var cartJson = session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartJson)!;
        }

        private void SaveCart(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
            }
        }

        public List<CartItem> GetCartItems()
        {
            return GetCart();
        }

        public void AddToCart(Product product)
        {
            if (product == null)
            {
                return;
            }

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem { ProductId = product.Id, Product = product, Quantity = 1 });
            }

            SaveCart(cart);
        }

        public bool RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCart(cart);
                return true;
            }
            return false;
        }

        public void ClearCart()
        {
            SaveCart(new List<CartItem>());
        }

        public int GetCartItemCount()
        {
            return GetCart().Sum(c => c.Quantity);
        }

        // Sửa ProcessCheckout để trả về orderId thay vì bool
        public async Task<int> ProcessCheckout(string userId, string fullName, string address, string phoneNumber, decimal totalPrice)
        {
            var cartItems = GetCartItems();
            if (cartItems == null || cartItems.Count == 0)
            {
                return 0; // Giỏ hàng trống
            }

            // Gọi OrderService với đầy đủ thông tin
            int orderId = await _orderService.CreateOrder(userId, fullName, address, phoneNumber, cartItems, totalPrice);

            if (orderId != 0)
            {
                ClearCart();
            }

            return orderId;
        }


        public bool UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);

            if (cartItem == null || quantity <= 0)
            {
                return false;
            }

            cartItem.Quantity = quantity;
            SaveCart(cart);
            return true;
        }
    }
}