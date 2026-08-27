using KafkaAnalyticsService.Data;
using KafkaAnalyticsService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KafkaAnalyticsService.Controllers
{
    [ApiController]
    [Route("analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AnalyticsDbContext _dbContext;

        public AnalyticsController(
            AnalyticsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var state =
                await _dbContext.AnalyticsStates
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        s => s.Id == 1);

            if (state == null)
            {
                return Ok(
                    new AnalyticsSummaryDto());
            }

            return Ok(
                new AnalyticsSummaryDto
                {
                    TotalCustomers =
                        state.TotalCustomers,

                    ActiveCustomers =
                        state.ActiveCustomers,

                    InactiveCustomers =
                        state.InactiveCustomers,

                    TotalOrders =
                        state.TotalOrders,

                    CreatedOrders =
                        state.CreatedOrders,

                    ConfirmedOrders =
                        state.ConfirmedOrders,

                    RejectedOrders =
                        state.RejectedOrders,

                    InventoryReservedEvents =
                        state.InventoryReservedEvents,

                    InventoryRejectedEvents =
                        state.InventoryRejectedEvents,

                    TotalReservedQuantity =
                        state.TotalReservedQuantity
                });
        }
    }
}
