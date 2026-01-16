using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using lab1.Models;
using lab1.Repositories;

[Authorize(Roles = "Admin")] // Bắt buộc đăng nhập để truy cập
public class CategoryController : Controller
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // Tất cả mọi người đều có thể xem danh sách  

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return View(categories);
    }

    [Authorize(Roles = "Admin")] // Chỉ Admin được phép thêm
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Chỉ Admin được phép thêm
    public async Task<IActionResult> Add(Category category)
    {
        if (ModelState.IsValid)
        {
            await _categoryRepository.AddAsync(category);
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    [Authorize(Roles = "Admin")] // Chỉ Admin được phép sửa
    public async Task<IActionResult> Update(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Chỉ Admin được phép sửa
    public async Task<IActionResult> Update(int id, Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _categoryRepository.UpdateAsync(category);
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    [Authorize(Roles = "Admin")] // Chỉ Admin được phép xóa
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost, ActionName("DeleteConfirmed")]
    [Authorize(Roles = "Admin")] // Chỉ Admin được phép xóa
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _categoryRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
