using WebApplication2.Models;

namespace WebApplication2.Repositories.Interfaces
{
    public interface IPositionRepository
    {
        Task<LastPosition?> GetLastPositionByPersonIdAsync(
            int personId);

        Task AddLastPositionAsync(
            LastPosition lastPosition);

        Task AddPositionHistoryAsync(
            PositionHistory positionHistory);

        Task SaveChangesAsync();
    }
}