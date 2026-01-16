using lab1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;  // 🚀 Thêm dòng này


public class CategoryMenuViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context; // Hoặc một Repository

    public CategoryMenuViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _context.Categories.ToListAsync(); // Lấy danh mục từ DB
        return View("Default", categories); // Xác nhận view "Default"
    }
}
