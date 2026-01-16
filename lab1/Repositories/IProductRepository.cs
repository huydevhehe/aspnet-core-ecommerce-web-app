using lab1.Models;

namespace lab1.Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> GetQueryable();
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
