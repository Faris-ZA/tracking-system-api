using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs.Venues;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var venues = await _venueService.GetAllAsync();

            return Ok(venues);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var venue = await _venueService.GetByIdAsync(id);

            if (venue == null)
            {
                return NotFound(new
                {
                    message = "Venue not found."
                });
            }

            return Ok(venue);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateVenueDto dto)
        {
            try
            {
                var venue = await _venueService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = venue.Id },
                    venue);
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
            UpdateVenueDto dto)
        {
            try
            {
                var venue = await _venueService.UpdateAsync(id, dto);

                if (venue == null)
                {
                    return NotFound(new
                    {
                        message = "Venue not found."
                    });
                }

                return Ok(venue);
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
            try
            {
                var deleted = await _venueService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Venue not found."
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}