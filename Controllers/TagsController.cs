using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs.Tags;
using WebApplication2.Services.Interfaces;
using WebApplication2.DTOs.Performance;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(
            ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _tagService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result =
                await _tagService.GetByIdAsync(id);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateTagDto dto)
        {
            var result =
                await _tagService.CreateAsync(dto);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateTagDto dto)
        {
            var result =
                await _tagService.UpdateAsync(
                    id,
                    dto);

            return Ok(result);
        }

        [HttpGet("performance/database")]
        public async Task<IActionResult> GetDatabasePerformance(
            [FromQuery] TagPerformanceQueryDto queryDto)
        {
            var result =
                await _tagService
                    .GetDatabasePerformanceAsync(queryDto);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _tagService.DeleteAsync(id);

            return NoContent();
        }
    

        [HttpGet("performance/cache")]
        public async Task<IActionResult> GetCachePerformance(
            [FromQuery] TagPerformanceQueryDto queryDto)
        {
            var result =
                await _tagService
                    .GetCachePerformanceAsync(
                        queryDto);

            return Ok(result);
        }
    }
}

