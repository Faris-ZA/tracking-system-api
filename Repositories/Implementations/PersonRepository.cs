using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using System.Diagnostics;
using WebApplication2.DTOs.Performance;
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
                .Include(p => p.TagAssociations)
                .ThenInclude(a => a.Tag)
                .Where(p => p.UpdateStatus != UpdateStatus.Deleted)
                .ToListAsync();
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _context.People
                .Include(p => p.TagAssociations)
                .ThenInclude(a => a.Tag)
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

        public async Task<DatabasePageResultDto<Person>>
        GetPerformancePageAsync(
          PeoplePerformanceQueryDto queryDto)
        {
            var query = _context.People
                .AsNoTracking()
                .Include(p => p.TagAssociations)
                .ThenInclude(a => a.Tag)
                .Where(p =>
                    p.UpdateStatus != UpdateStatus.Deleted)
                .AsQueryable();

            if (queryDto.PersonId.HasValue)
            {
                query = query.Where(p =>
                    p.Id == queryDto.PersonId.Value);
            }

            if (!string.IsNullOrWhiteSpace(
                queryDto.PersonName))
            {
                var normalizedName =
                    queryDto.PersonName
                        .Trim()
                        .ToLower();

                query = query.Where(p =>
                    p.Name.ToLower()
                        .Contains(normalizedName));
            }
            if (queryDto.IsAssigned.HasValue)
            {
                if (queryDto.IsAssigned.Value)
                {
                    query = query.Where(p =>
                        p.TagAssociations.Any(a =>
                            a.Tag.UpdateStatus !=
                            UpdateStatus.Deleted));
                }
                else
                {
                    query = query.Where(p =>
                        !p.TagAssociations.Any(a =>
                            a.Tag.UpdateStatus !=
                            UpdateStatus.Deleted));
                }
            }

            var stopwatch = Stopwatch.StartNew();

            var totalRecords =
                await query.CountAsync();

            var people = await query
                .OrderBy(p => p.Id)
                .Skip(
                    (queryDto.PageNumber - 1) *
                    queryDto.PageSize)
                .Take(queryDto.PageSize)
                .ToListAsync();

            stopwatch.Stop();

            return new DatabasePageResultDto<Person>
            {
                Items = people,
                TotalRecords = totalRecords,
                DatabaseQueryTimeMs =
                    stopwatch.ElapsedMilliseconds
            };
        }

        public async Task<List<Person>> GetBatchAsync(
            int skip,
            int take)
        {
            return await _context.People
                .AsNoTracking()
                .Include(p => p.TagAssociations)
                .ThenInclude(a => a.Tag)
                .Where(p => p.UpdateStatus != UpdateStatus.Deleted)
                .OrderBy(p => p.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
