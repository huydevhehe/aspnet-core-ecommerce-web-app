
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using lab1.Models;
using lab1.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace lab1.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<IActionResult> Index(int? categoryId = null, string searchQuery = null)
        {
            var categories = await _categoryRepository.GetAllAsync() ?? Enumerable.Empty<Category>();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
            ViewBag.SelectedCategory = categoryId?.ToString();
            ViewBag.SearchQuery = searchQuery;

            var products = await _productRepository.GetAllAsync() ?? Enumerable.Empty<Product>();

            if (categoryId.HasValue && categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = products.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            var productList = products.ToList();

            if (productList.Count == 1)
            {
                return RedirectToAction("Display", new { id = productList.First().Id });
            }

            if (!productList.Any())
            {
                return RedirectToAction("NoResults");
            }

            return View(productList);
        }
        public IActionResult NoResults()
        {
            return View();
        }

        [Authorize(Roles = "Admin")] //cam truy cap admin

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync() ?? Enumerable.Empty<Category>();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Admin")] // cam truy cap add
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync() ?? Enumerable.Empty<Category>();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(product);
            }

            if (imageUrl != null && IsImage(imageUrl))
            {
                product.ImageUrl = await SaveImage(imageUrl);
            }
            else if (imageUrl != null)
            {
                ModelState.AddModelError("ImageUrl", "Chỉ được tải lên file ảnh (jpg, jpeg, png, gif).");
                var categories = await _categoryRepository.GetAllAsync() ?? Enumerable.Empty<Category>();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(product);
            }

            await _productRepository.AddAsync(product);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/images/" + uniqueFileName;
        }

        private bool IsImage(IFormFile file)
        {
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            return file != null && permittedExtensions.Contains(Path.GetExtension(file.FileName).ToLower());
        }

        public async Task<IActionResult> Display(int id)
        {
            if (id <= 0) return RedirectToAction(nameof(Index));

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound("Sản phẩm không tồn tại.");

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var categories = await _categoryRepository.GetAllAsync() ?? Enumerable.Empty<Category>();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync() ?? Enumerable.Empty<Category>();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
                return View(product);
            }

            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null) return NotFound();

            if (imageUrl != null && IsImage(imageUrl))
            {
                existingProduct.ImageUrl = await SaveImage(imageUrl);
            }
            else if (imageUrl != null)
            {
                ModelState.AddModelError("ImageUrl", "Chỉ được tải lên file ảnh (jpg, jpeg, png, gif).");
                var categories = await _categoryRepository.GetAllAsync() ?? Enumerable.Empty<Category>();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
                return View(product);
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;

            await _productRepository.UpdateAsync(existingProduct);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, bool confirm = true)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return RedirectToAction(nameof(Index));

            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XemDanhMuc(int? categoryId)
        {
            var products = await _productRepository.GetAllAsync() ?? Enumerable.Empty<Product>();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            var categories = await _categoryRepository.GetAllAsync() ?? Enumerable.Empty<Category>();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.SelectedCategory = categoryId;

            return View(products.ToList()); // Đưa danh sách đã lọc vào View
        }


    }
}
