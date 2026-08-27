using InventoryService.DTOs;
using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> Reserve(ReserveInventoryDto dto)
        {
            var result = await _inventoryService.ReserveAsync(dto);

            return Ok(result);
        }
    }
}
