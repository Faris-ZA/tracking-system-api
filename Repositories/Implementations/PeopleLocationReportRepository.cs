using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.DTOs.Reports;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;

namespace WebApplication2.Repositories.Implementations
{
    public class PeopleLocationReportRepository
        : IPeopleLocationReportRepository
    {
        private readonly AppDbContext _context;

        public PeopleLocationReportRepository(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PeopleLocationReportDto>>
            GetPeopleLocationDataAsync()
        {
            var activePeople =
                _context.People
                    .AsNoTracking()
                    .Where(person =>
                        person.UpdateStatus != UpdateStatus.Deleted);

            var reportData =
                await (
                    from person in activePeople

                    join lastPosition in
                        _context.LastPositions.AsNoTracking()
                        on person.Id equals lastPosition.PeopleId
                        into lastPositionGroup

                    from lastPosition in
                        lastPositionGroup.DefaultIfEmpty()

                    select new PeopleLocationReportDto
                    {
                        PersonId = person.Id,
                        PersonName = person.Name,

                        VenueName = lastPosition != null
                            ? lastPosition.Venue.Name
                            : null,

                        FloorName = lastPosition != null
                            ? lastPosition.Floor.Name
                            : null,

                        ZoneName = lastPosition != null
                            ? lastPosition.Zone.Name
                            : null,

                        X = lastPosition != null
                            ? lastPosition.X
                            : null,

                        Y = lastPosition != null
                            ? lastPosition.Y
                            : null,

                        LastSeen = lastPosition != null
                            ? lastPosition.LastUpdate
                            : null
                    })
                    .ToListAsync();

            return reportData;
        }
    }
}