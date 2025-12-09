using Honse.Global.Extensions;
using Honse.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Honse.API.Controllers
{
    [Route("api/public/orders")]
    [ApiController]
    public class PublicOrderController : ControllerBase
    {
        private readonly IOrderManager orderManager;

        public PublicOrderController(IOrderManager orderManager)
        {
            this.orderManager = orderManager;
        }

        /// <summary>
        /// Places a new order for authenticated or guest customers
        /// </summary>
        [HttpPost]
        [Route("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] Managers.Interfaces.PlaceOrderRequest request)
        {
            try
            {
                Guid? userId = null;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var response = await orderManager.PlaceOrder(request, userId);
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
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .First()
                    .ErrorMessage;

                return BadRequest((new { errorMessage }));
            }

            var orderResponse = await orderManager.GetOrderByIdPublic(id).WithTryCatch();

            if (!orderResponse.IsSuccessfull)
            {
                return BadRequest(orderResponse.Exception.Message);
            }

            if (orderResponse.Result == null)
            {
                return NotFound(new { errorMessage = "Order not found" });
            }

            return Ok(orderResponse.Result);
        }

        /// <summary>
        /// Allows customer to cancel their order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel/{id}")]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .First()
                    .ErrorMessage;

                return BadRequest((new { errorMessage }));
            }

            var cancelResponse = await orderManager.CancelOrderPublic(id).WithTryCatch();

            if (!cancelResponse.IsSuccessfull)
            {
                return BadRequest(cancelResponse.Exception.Message);
            }

            return Ok(new { message = "Order cancelled successfully" });
        }
    }
}
