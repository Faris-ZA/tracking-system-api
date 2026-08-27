using AnalyticsService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers
{
    [ApiController]
    [Route("analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(
            IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var summary =
                    await _analyticsService.GetSummaryAsync();

                return Ok(summary);
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new
                    {
                        message =
                            "One or more required services are unavailable."
                    });
            }
        }
    }
}
