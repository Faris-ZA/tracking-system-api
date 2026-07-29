using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs.Positions;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionsController(
            IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePositionDto dto)
        {
            var result =
                await _positionService.CreateAsync(dto);

            return Ok(result);
        }
    }
}