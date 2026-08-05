using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs.Imports;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleImportController : ControllerBase
    {
        private readonly IPeopleImportService
            _peopleImportService;

        public PeopleImportController(
            IPeopleImportService peopleImportService)
        {
            _peopleImportService =
                peopleImportService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PeopleImportSummaryDto>>
            ImportPeople(IFormFile file)
        {
            var summary =
                await _peopleImportService.ImportAsync(file);

            return Ok(summary);
        }
    }
}