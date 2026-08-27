using KafkaInventoryService.DTOs;
using KafkaInventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace KafkaInventoryService.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly IKafkaInventoryService _inventoryService;

        public ProductsController(IKafkaInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var product = await _inventoryService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                product);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _inventoryService.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _inventoryService.GetAllAsync();

            return Ok(products);
        }

        [HttpPut("{id:int}/stock")]
        public async Task<IActionResult> UpdateStock(
            int id,
            UpdateStockDto dto)
        {
            var product = await _inventoryService.UpdateStockAsync(id, dto);

            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}

