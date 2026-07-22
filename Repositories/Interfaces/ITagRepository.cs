using WebApplication2.Models;

namespace WebApplication2.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllAsync();

        Task<Tag?> GetByIdAsync(int id);

        Task<bool> ActiveMacExistsAsync(string mac);

        Task<bool> HasPeopleAssociationAsync(int tagId);

        Task AddAsync(Tag tag);

        Task SaveChangesAsync();
    }
}
