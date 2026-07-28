using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;

namespace WebApplication2.Repositories.Implementations
{
    public class PositionRepository : IPositionRepository
    {
        private readonly AppDbContext _context;

        public PositionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LastPosition?>
            GetLastPositionByPersonIdAsync(int personId)
        {
            return await _context.LastPositions
                .FirstOrDefaultAsync(position =>
                    position.PeopleId == personId);
        }

        public async Task AddLastPositionAsync(
            LastPosition lastPosition)
        {
            await _context.LastPositions
                .AddAsync(lastPosition);
        }

        public async Task AddPositionHistoryAsync(
            PositionHistory positionHistory)
        {
            await _context.PositionHistories
                .AddAsync(positionHistory);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}