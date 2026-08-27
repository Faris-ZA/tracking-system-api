using KafkaInventoryService.DTOs;
using KafkaInventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace KafkaInventoryService.Controllers
{
    [ApiController]
    [Route("inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IKafkaInventoryService _inventoryService;

        public InventoryController(IKafkaInventoryService inventoryService)
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

