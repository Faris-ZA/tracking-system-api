using WebApplication2.Models;

namespace WebApplication2.Repositories.Interfaces
{
    public interface IVenueRepository
    {
        Task<List<Venue>> GetAllAsync();

        Task<Venue?> GetByIdAsync(int id);

        Task<bool> ActiveNameExistsAsync(string name, int? excludeId = null);

        Task<bool> HasActiveFloorsAsync(int venueId);

        Task AddAsync(Venue venue);

        Task SaveChangesAsync();
    }
}