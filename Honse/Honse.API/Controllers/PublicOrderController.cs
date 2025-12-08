using Microsoft.AspNetCore.Mvc;

namespace Honse.API.Controllers
{
    [Route("api/public/orders")]
    [ApiController]
    public class PublicOrderController : ControllerBase
    {
        private readonly Managers.Interfaces.IOrderManager orderManager;

        public PublicOrderController(Managers.Interfaces.IOrderManager orderManager)
        {
            this.orderManager = orderManager;
        }

        /// <summary>
        /// Places a new order for a customer
        /// </summary>
        [HttpPost]
        [Route("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] Managers.Interfaces.PlaceOrderRequest request)
        {
            try
            {
                var response = await orderManager.PlaceOrder(request);
                return Ok(response);
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while placing the order.", details = ex.Message });
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOrderDetails([FromRoute] Guid id)
        {
            return Ok();
        }

        [HttpPost]
        [Route("cancel/{id}")]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid id)
        {
            return Ok();
        }
    }
}
