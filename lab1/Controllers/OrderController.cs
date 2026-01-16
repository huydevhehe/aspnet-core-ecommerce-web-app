using lab1.Models;
using lab1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace lab1.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public OrderController(OrderService orderService, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _orderService = orderService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userName = _userManager.GetUserName(User); // Lấy UserName thay vì UserId
            Console.WriteLine($"UserName hiện tại: {userName}");

            var orders = await _orderService.GetOrdersByUser(userName); // Truy vấn theo UserName
            Console.WriteLine($"Số đơn hàng lấy được: {orders.Count}");

            return View(orders ?? new List<Order>());
        }



        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDetails(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        public async Task<bool> SaveOrder(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu đơn hàng: {ex.Message}");
                return false;
            }
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _orderService.GetOrderDetails(id);
            if (order == null)
            {
                return NotFound();
            }
            return View("OrderDetails", order);
        }
    }
}
