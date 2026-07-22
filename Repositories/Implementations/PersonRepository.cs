using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;

namespace WebApplication2.Repositories.Implementations
{
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;

        public PersonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Person>> GetAllAsync()
        {
            return await _context.People
                .Where(p => p.UpdateStatus != UpdateStatus.Deleted)
                .ToListAsync();
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _context.People
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.UpdateStatus != UpdateStatus.Deleted);
        }

        public async Task<bool> ActiveNameExistsAsync(string name)
        {
            var normalizedName = name.ToLower();

            return await _context.People.AnyAsync(p =>
                p.UpdateStatus != UpdateStatus.Deleted &&
                p.Name.ToLower() == normalizedName);
        }

        public async Task<bool> HasTagAssociationAsync(int personId)
        {
            return await _context.PeopleTagAssociations
                .AnyAsync(a => a.PeopleId == personId);
        }

        public async Task AddAsync(Person person)
        {
            await _context.People.AddAsync(person);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}