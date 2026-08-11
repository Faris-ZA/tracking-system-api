using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs.People;
using WebApplication2.DTOs.Performance;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PeopleController(
            IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _personService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result =
                await _personService.GetByIdAsync(id);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePersonDto dto)
        {
            var result =
                await _personService.CreateAsync(dto);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdatePersonDto dto)
        {
            var result =
                await _personService.UpdateAsync(
                    id,
                    dto);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _personService.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("performance/database")]
        public async Task<IActionResult> GetDatabasePerformance(
            [FromQuery] PeoplePerformanceQueryDto queryDto)
        {
            var result =
                await _personService
                    .GetDatabasePerformanceAsync(queryDto);

            return Ok(result);
        }

        [HttpGet("performance/redis")]
        public async Task<IActionResult> GetRedisPerformance(
            [FromQuery] PeoplePerformanceQueryDto queryDto)
        {
            var result =
                await _personService
                    .GetRedisPerformanceAsync(queryDto);

            return Ok(result);
        }   
    }
}
