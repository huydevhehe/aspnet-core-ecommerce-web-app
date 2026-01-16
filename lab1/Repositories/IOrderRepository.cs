using lab1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lab1.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> SaveOrder(Order order);
        Task<Order?> GetOrderById(int id);
        Task<List<Order>?> GetOrdersByUser(string userId);
    }
}