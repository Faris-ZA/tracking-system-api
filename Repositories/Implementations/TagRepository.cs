using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using System.Diagnostics;
using WebApplication2.DTOs.Performance;
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
                .Include(t => t.PeopleAssociations)
                .ThenInclude (a => a.Person)
                .Where(t => t.UpdateStatus != UpdateStatus.Deleted)
                .ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags
                .Include(t => t.PeopleAssociations)
                .ThenInclude(a => a.Person)
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

        public async Task<DatabasePageResultDto<Tag>>
          GetPerformancePageAsync(
            TagPerformanceQueryDto queryDto)
        {
            var query = _context.Tags
                .AsNoTracking()
                .Include(t => t.PeopleAssociations)
                .ThenInclude(a => a.Person)
                .Where(t =>
                    t.UpdateStatus != UpdateStatus.Deleted)
                .AsQueryable();

            if (queryDto.TagId.HasValue)
            {
                query = query.Where(t =>
                    t.Id == queryDto.TagId.Value);
            }

            if (!string.IsNullOrWhiteSpace(
                queryDto.TagLabel))
            {
                var normalizedLabel =
                    queryDto.TagLabel
                        .Trim()
                        .ToLower();

                query = query.Where(t =>
                    t.Label.ToLower()
                        .Contains(normalizedLabel));
            }
            if (queryDto.IsAssigned.HasValue)
            {
                if (queryDto.IsAssigned.Value)
                {
                    query = query.Where(t =>
                        t.PeopleAssociations.Any(a =>
                            a.Person.UpdateStatus !=
                            UpdateStatus.Deleted));
                }
                else
                {
                    query = query.Where(t =>
                        !t.PeopleAssociations.Any(a =>
                            a.Person.UpdateStatus !=
                            UpdateStatus.Deleted));
                }
            }

            var stopwatch = Stopwatch.StartNew();

            var totalRecords =
                await query.CountAsync();

            var tags = await query
                .OrderBy(t => t.Id)
                .Skip(
                    (queryDto.PageNumber - 1) *
                    queryDto.PageSize)
                .Take(queryDto.PageSize)
                .ToListAsync();

            stopwatch.Stop();

            return new DatabasePageResultDto<Tag>
            {
                Items = tags,
                TotalRecords = totalRecords,
                DatabaseQueryTimeMs =
                    stopwatch.ElapsedMilliseconds
            };
        }

        public async Task<List<Tag>> GetBatchAsync(
            int skip,
            int take)
        {
            return await _context.Tags
                .AsNoTracking()
                .Include(t => t.PeopleAssociations)
                .ThenInclude(a => a.Person)
                .Where(t => t.UpdateStatus != UpdateStatus.Deleted)
                .OrderBy(t => t.Id)
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

