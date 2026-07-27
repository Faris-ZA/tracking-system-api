using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;

namespace WebApplication2.Repositories.Implementations
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            return await _context.Tags
                .Where(t => t.UpdateStatus != UpdateStatus.Deleted)
                .ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.UpdateStatus != UpdateStatus.Deleted);
        }

        public async Task<bool> ActiveMacExistsAsync(string mac)
        {
            var normalizedMac = mac.ToLower();

            return await _context.Tags.AnyAsync(t =>
                t.UpdateStatus != UpdateStatus.Deleted &&
                t.Mac.ToLower() == normalizedMac);
        }

        public async Task<bool> HasPeopleAssociationAsync(int tagId)
        {
            return await _context.PeopleTagAssociations
                .AnyAsync(a => a.TagId == tagId);
        }

        public async Task AddAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
