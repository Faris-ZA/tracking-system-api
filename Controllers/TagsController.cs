using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs.Tags;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _tagService.GetAllAsync();

            return Ok(tags);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);

            if (tag == null)
            {
                return NotFound(new
                {
                    message = "Tag not found."
                });
            }

            return Ok(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTagDto dto)
        {
            try
            {
                var tag = await _tagService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = tag.Id },
                    tag);
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
            UpdateTagDto dto)
        {
            try
            {
                var tag = await _tagService.UpdateAsync(id, dto);

                if (tag == null)
                {
                    return NotFound(new
                    {
                        message = "Tag not found."
                    });
                }

                return Ok(tag);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
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
                var deleted = await _tagService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Tag not found."
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
