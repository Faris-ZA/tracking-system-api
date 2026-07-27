using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;

namespace WebApplication2.Repositories.Implementations
{
    public class VenueRepository : IVenueRepository
    {
        private readonly AppDbContext _context;

        public VenueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Venue>> GetAllAsync()
        {
            return await _context.Venues
                .Where(v => v.UpdateStatus != UpdateStatus.Deleted)
                .ToListAsync();
        }

        public async Task<Venue?> GetByIdAsync(int id)
        {
            return await _context.Venues
                .FirstOrDefaultAsync(v =>
                    v.Id == id &&
                    v.UpdateStatus != UpdateStatus.Deleted);
        }

        public async Task<bool> ActiveNameExistsAsync(
            string name,
            int? excludeId = null)
        {
            return await _context.Venues.AnyAsync(v =>
                v.UpdateStatus != UpdateStatus.Deleted &&
                v.Name.ToLower() == name.ToLower() &&
                (!excludeId.HasValue || v.Id != excludeId.Value));
        }

        public async Task<bool> HasActiveFloorsAsync(int venueId)
        {
            return await _context.Floors.AnyAsync(f =>
                f.VenueId == venueId &&
                f.UpdateStatus != UpdateStatus.Deleted);
        }

        public async Task AddAsync(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}