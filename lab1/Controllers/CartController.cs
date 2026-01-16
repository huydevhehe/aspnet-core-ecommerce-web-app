using lab1.Models;
using lab1.Repositories;
using lab1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // Import thư viện để đọc appsettings.json


namespace lab1.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly IProductRepository _productRepository;
        private readonly OrderService _orderService;
        private readonly IConfiguration _configuration;


        public CartController(CartService cartService, IProductRepository productRepository, OrderService orderService, IConfiguration configuration)
        {
            _cartService = cartService;
            _productRepository = productRepository;
            _orderService = orderService;
            _configuration = configuration; // Đảm bảo chỉ có 1 constructor duy nhất
        }

        public IActionResult Index()
        {
            var cartItems = _cartService.GetCartItems();
            decimal totalPrice = cartItems.Sum(item => item.TotalPrice);
            ViewBag.TotalPrice = totalPrice;
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromForm] int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "ID sản phẩm không hợp lệ!" });
            }

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
            }

            _cartService.AddToCart(product);
            var cartItems = _cartService.GetCartItems();
            decimal totalPrice = cartItems.Sum(item => item.TotalPrice);

            return Json(new
            {
                success = true,
                message = "Đã thêm sản phẩm vào giỏ hàng!",
                cartCount = _cartService.GetCartItemCount(),
                totalPrice = totalPrice.ToString("N0")
            });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            bool isRemoved = _cartService.RemoveFromCart(id);
            if (!isRemoved)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng!" });
            }

            var cartItems = _cartService.GetCartItems();
            decimal totalPrice = cartItems.Sum(item => item.TotalPrice);

            return Json(new
            {
                success = true,
                message = "Đã xóa sản phẩm khỏi giỏ hàng!",
                cartCount = _cartService.GetCartItemCount(),
                totalPrice = totalPrice.ToString("N0")
            });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Số lượng phải lớn hơn 0!" });
            }

            bool updated = _cartService.UpdateQuantity(id, quantity);
            if (!updated)
            {
                return Json(new { success = false, message = "Cập nhật số lượng thất bại!" });
            }

            var cartItems = _cartService.GetCartItems();
            decimal totalPrice = cartItems.Sum(item => item.TotalPrice);

            return Json(new { success = true, totalPrice = totalPrice.ToString("N0") });
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            _cartService.ClearCart();
            return Json(new
            {
                success = true,
                message = "Đã xóa toàn bộ giỏ hàng!",
                totalPrice = "0"
            });
        }

        [HttpGet]
        [Authorize]
        public IActionResult CheckoutPage()
        {
            var cartItems = _cartService.GetCartItems();
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            decimal totalPrice = cartItems.Sum(item => item.TotalPrice);
            // Tạo URL QR Code thanh toán tự động
            string qrUrl = $"https://img.vietqr.io/image/MB-9999910082004-qr_only.png?amount={totalPrice:F0}&addInfo=ThanhToanDonHang";
            ViewBag.QRCodeUrl = qrUrl;


            // Truyền danh sách sản phẩm vào ViewBag
            ViewBag.CartItems = cartItems;
            ViewBag.TotalPrice = totalPrice;

            return View();
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProcessCheckout(string FullName, string UserEmail, string Address, string PhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(UserEmail) ||
                string.IsNullOrWhiteSpace(Address) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin!";
                return RedirectToAction("CheckoutPage");
            }

            var cartItems = _cartService.GetCartItems();
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("CheckoutPage");
            }

            decimal totalPrice = cartItems.Sum(item => item.TotalPrice);
            string userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Không thể xác định người dùng!";
                return RedirectToAction("CheckoutPage");
            }

            // Xử lý đơn hàng
            int orderId = await _cartService.ProcessCheckout(userId, FullName, Address, PhoneNumber, totalPrice);
            if (orderId == 0)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo đơn hàng!";
                return RedirectToAction("CheckoutPage");
            }

            // Nội dung email xác nhận
            string emailContent = $"<h3>Xin chào {FullName},</h3><p>Bạn đã đặt hàng thành công!</p>"
                                + $"<p>Thông tin đơn hàng:</p>"
                                + $"<ul><li>Họ tên: {FullName}</li>"
                                + $"<li>Địa chỉ: {Address}</li>"
                                + $"<li>SĐT: {PhoneNumber}</li>"
                                + $"<li>Tổng tiền: {totalPrice:N0} VNĐ</li></ul>"
            +$"<p style='color:red; font-weight:bold;'>📢 Cảm ơn bạn đã mua hàng! Đơn hàng sẽ sớm giao đến bạn .Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi.</p>";


            // Gửi email xác nhận
            SendEmail(UserEmail, "Xác nhận đơn hàng", emailContent);

            TempData["SuccessMessage"] = "Đơn hàng đã được đặt thành công , 1 EMAIL xác nhận đã được gửi đến bạn!";
            return RedirectToAction("OrderDetails", "Order", new { id = orderId });
        }
        private void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                string senderEmail = _configuration["EmailSettings:SenderEmail"];
                string senderPassword = _configuration["EmailSettings:SenderPassword"];
                string smtpServer = _configuration["EmailSettings:SMTPServer"];
                int port = int.Parse(_configuration["EmailSettings:Port"]);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(senderEmail);
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpServer, port))
                    {
                        smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể gửi email xác nhận. Vui lòng kiểm tra lại!";
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> BuyNow(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index", "Home");
            }

            _cartService.ClearCart(); // Xóa giỏ hàng cũ
            _cartService.AddToCart(product); // Thêm sản phẩm vào giỏ

            return RedirectToAction("CheckoutPage");
        }



    }
}