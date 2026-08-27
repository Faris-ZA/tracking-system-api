using Microsoft.AspNetCore.Mvc;
using KafkaOrderService.DTOs;
using KafkaOrderService.Services;

namespace KafkaOrderService.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IKafkaOrderService _orderService;

        public OrdersController(IKafkaOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            try
            {
                var order = await _orderService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = order.OrderId },
                    order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new { message = "A required service is unavailable." });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();

            return Ok(orders);
        }

        [HttpGet("/customers/{customerId:int}/orders")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var orders = await _orderService.GetByCustomerIdAsync(customerId);

            return Ok(orders);
        }
    }
}

