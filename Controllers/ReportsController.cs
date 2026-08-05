using Microsoft.AspNetCore.Mvc;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IPeopleLocationExportService
            _exportService;

        public ReportsController(
            IPeopleLocationExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpGet("people-location")]
        public async Task<IActionResult>
            ExportPeopleLocationReport()
        {
            var exportResult =
                await _exportService.ExportAllPeopleAsync();

            return File(
                exportResult.Content,
                exportResult.ContentType,
                exportResult.FileName);
        }
    }
}