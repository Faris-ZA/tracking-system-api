using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs.Zones;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZonesController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public ZonesController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _zoneService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _zoneService.GetByIdAsync(id);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateZoneDto dto)
        {
            var result =
                await _zoneService.CreateAsync(dto);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateZoneDto dto)
        {
            var result =
                await _zoneService.UpdateAsync(id, dto);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _zoneService.DeleteAsync(id);

            return NoContent();
        }
    }
}
