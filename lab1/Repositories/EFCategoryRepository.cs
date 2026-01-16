using System.Collections.Generic;
using System.Threading.Tasks;
using lab1.Models;
using Microsoft.EntityFrameworkCore;

namespace lab1.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ danh sách Category
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Lấy Category theo Id (Tối ưu)
        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id)
                ?? throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        // Thêm mới Category
        public async Task AddAsync(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        // Cập nhật Category (Tối ưu)
        public async Task UpdateAsync(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            _context.Categories.Attach(category);
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Xóa Category theo Id (Tối ưu)
        public async Task DeleteAsync(int id)
        {
            var affectedRows = await _context.Categories
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();

            if (affectedRows == 0)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
        }
    }
}
