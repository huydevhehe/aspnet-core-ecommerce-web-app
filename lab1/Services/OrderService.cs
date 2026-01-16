using lab1.Models;
using lab1.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lab1.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<int> CreateOrder(string userId, string fullName, string address, string phoneNumber, List<CartItem> cartItems, decimal totalPrice)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phoneNumber) ||
                cartItems == null || cartItems.Count == 0)
            {
                return 0;
            }

            var order = new Order
            {
                UserId = userId,
                FullName = fullName,  // Đảm bảo lưu FullName
                Address = address,    // Lưu Address
                PhoneNumber = phoneNumber,  // Lưu PhoneNumber
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                Status = "Pending",
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Product?.Price ?? 0
                }).ToList()
            };

            bool result = await _orderRepository.SaveOrder(order);
            return result ? order.Id : 0;
        }






        public async Task<List<Order>> GetOrdersByUser(string userId)
        {
            Console.WriteLine($"🔍 Đang lấy đơn hàng cho UserId: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("⚠️ Lỗi: UserId rỗng hoặc null.");
                return new List<Order>(); // Trả về danh sách rỗng thay vì `null`
            }

            var orders = await _orderRepository.GetOrdersByUser(userId) ?? new List<Order>();

            Console.WriteLine($"✅ Số đơn hàng tìm thấy: {orders.Count}");

            return orders;
        }





        public async Task<Order?> GetOrderDetails(int id)
        {
            return await _orderRepository.GetOrderById(id);
        }

    }
}