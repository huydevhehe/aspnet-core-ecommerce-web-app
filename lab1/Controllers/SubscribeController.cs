using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
// file này là dky email nhận thông báo 
namespace lab1.Controllers
{
    [Route("api/subscribe")]
    [ApiController]
    public class SubscribeController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Subscribe([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { message = "Email không hợp lệ" });

            // Gửi email xác nhận
            bool emailSent = await SendEmail(request.Email);
            if (emailSent)
                return Ok(new { message = "Email đã được gửi!" });

            return StatusCode(500, new { message = "Lỗi gửi email!" });
        }

        private async Task<bool> SendEmail(string recipientEmail)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Hỗ trợ", "your-email@gmail.com"));  // Email của bạn
                email.To.Add(new MailboxAddress("", recipientEmail));
                email.Subject = "Xác nhận đăng ký nhận tin";
                email.Body = new TextPart("plain")
                {
                    Text = "Cảm ơn bạn đã đăng ký nhận tin bởi website của nguyễn quốc huy ! Chúng tôi sẽ cập nhật thông tin mới nhất đến bạn."
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, false);
                await smtp.AuthenticateAsync("nguyenquochuyc7@gmail.com", "oqyz hoyy skzi fsme");  // Thay bằng email và mật khẩu ứng dụng
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class EmailRequest
    {
        public string Email { get; set; }
    }
}
