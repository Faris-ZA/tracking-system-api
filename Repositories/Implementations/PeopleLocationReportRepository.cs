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
            var reportData =
                await (
                    from person in _context.People
                        .AsNoTracking()

                    where person.UpdateStatus
                        != UpdateStatus.Deleted

                    join lastPosition in _context.LastPositions
                            .AsNoTracking()
                        on person.Id equals lastPosition.PeopleId
                        into lastPositionGroup

                    from lastPosition
                        in lastPositionGroup.DefaultIfEmpty()

                    join venue in _context.Venues
                            .AsNoTracking()
                        on lastPosition!.VenueId equals venue.Id
                        into venueGroup

                    from venue
                        in venueGroup.DefaultIfEmpty()

                    join floor in _context.Floors
                            .AsNoTracking()
                        on lastPosition!.FloorId equals floor.Id
                        into floorGroup

                    from floor
                        in floorGroup.DefaultIfEmpty()

                    join zone in _context.Zones
                            .AsNoTracking()
                        on lastPosition!.ZoneId equals zone.Id
                        into zoneGroup

                    from zone
                        in zoneGroup.DefaultIfEmpty()

                    select new PeopleLocationReportDto
                    {
                        PersonId = person.Id,
                        PersonName = person.Name,

                        VenueName =
                            venue != null
                                ? venue.Name
                                : null,

                        FloorName =
                            floor != null
                                ? floor.Name
                                : null,

                        ZoneName =
                            zone != null
                                ? zone.Name
                                : null,

                        LastSeen =
                            lastPosition != null
                                ? lastPosition.LastUpdate
                                : null
                    })
                    .ToListAsync();

            return reportData;
        }
    }
}