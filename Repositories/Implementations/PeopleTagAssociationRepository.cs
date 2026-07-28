using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;

namespace WebApplication2.Repositories.Implementations
{
    public class PeopleTagAssociationRepository
        : IPeopleTagAssociationRepository
    {
        private readonly AppDbContext _context;

        public PeopleTagAssociationRepository(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<PeopleTagAssociation?> GetAsync(
            int personId,
            int tagId)
        {
            return await _context.PeopleTagAssociations
                .FirstOrDefaultAsync(a =>
                    a.PeopleId == personId &&
                    a.TagId == tagId);
        }

        public async Task AddAsync(
            PeopleTagAssociation association)
        {
            await _context.PeopleTagAssociations
                .AddAsync(association);
        }

        public void Remove(
            PeopleTagAssociation association)
        {
            _context.PeopleTagAssociations
                .Remove(association);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}