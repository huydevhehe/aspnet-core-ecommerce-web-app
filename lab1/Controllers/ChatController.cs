using Microsoft.AspNetCore.Mvc;
using lab1.Services;

namespace lab1.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;

        public ChatController()
        {
            _chatService = new ChatService();
        }

        [HttpPost]
        public IActionResult GetAnswer([FromBody] string question)
        {
            var answer = _chatService.GetAnswer(question);
            return Json(new { reply = answer });
        }
    }
}
