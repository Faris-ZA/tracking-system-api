using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs.Floors;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FloorsController : ControllerBase
    {
        private readonly IFloorService _floorService;

        public FloorsController(IFloorService floorService)
        {
            _floorService = floorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var floors = await _floorService.GetAllAsync();

            return Ok(floors);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var floor = await _floorService.GetByIdAsync(id);

            if (floor == null)
            {
                return NotFound(new
                {
                    message = "Floor not found."
                });
            }

            return Ok(floor);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateFloorDto dto)
        {
            try
            {
                var floor =
                    await _floorService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = floor.Id },
                    floor);
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
            UpdateFloorDto dto)
        {
            try
            {
                var floor =
                    await _floorService.UpdateAsync(id, dto);

                if (floor == null)
                {
                    return NotFound(new
                    {
                        message = "Floor not found."
                    });
                }

                return Ok(floor);
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
                var deleted =
                    await _floorService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Floor not found."
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