using InventoryService.Models;

namespace InventoryService.Repositories
{
    public interface IInventoryRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
    }
}
