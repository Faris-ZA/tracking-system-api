using WebApplication2.Models;

namespace WebApplication2.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        Task<List<Person>> GetAllAsync();

        Task<Person?> GetByIdAsync(int id);

        Task<bool> ActiveNameExistsAsync(string name);

        Task<bool> HasTagAssociationAsync(int personId);

        Task AddAsync(Person person);

        Task SaveChangesAsync();
    }
}