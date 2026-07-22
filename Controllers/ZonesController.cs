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
            var zones = await _zoneService.GetAllAsync();

            return Ok(zones);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var zone = await _zoneService.GetByIdAsync(id);

            if (zone == null)
            {
                return NotFound(new
                {
                    message = "Zone not found."
                });
            }

            return Ok(zone);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateZoneDto dto)
        {
            try
            {
                var zone = await _zoneService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = zone.Id },
                    zone);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateZoneDto dto)
        {
            try
            {
                var zone =
                    await _zoneService.UpdateAsync(id, dto);

                if (zone == null)
                {
                    return NotFound(new
                    {
                        message = "Zone not found."
                    });
                }

                return Ok(zone);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _zoneService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Zone not found."
                });
            }

            return NoContent();
        }
    }
}