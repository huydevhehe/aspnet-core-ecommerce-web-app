using lab1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace lab1.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveOrder(Order order)
        {
            Console.WriteLine($"📝 Đang lưu đơn hàng cho UserId: {order.UserId}, Tổng tiền: {order.TotalPrice}");

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ Đơn hàng đã được lưu vào database.");
            return true;
        }


        public async Task<Order?> GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>?> GetOrdersByUser(string userName)
        {
            Console.WriteLine($"🔍 Truy vấn đơn hàng cho UserName: {userName}");

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userName) // Đổi từ UserId sang UserName
                .ToListAsync();

            Console.WriteLine($"✅ Số đơn hàng tìm thấy: {orders.Count}");
            return orders;
        }


    }
}