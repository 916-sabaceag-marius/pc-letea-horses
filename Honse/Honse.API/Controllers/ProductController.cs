
using Azure;
using Honse.Global.Extensions;
using Honse.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Honse.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductManager productManager;
        private readonly IUserManager userManager;

        public ProductController(Managers.Interfaces.IProductManager productManager, Managers.Interfaces.IUserManager userManager)
        {
            this.productManager = productManager;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .First()
                    .ErrorMessage;

                return BadRequest((new { errorMessage }));
            }

            string? userName = User.FindFirstValue(ClaimTypes.GivenName);

            var userResponse = await userManager.GetUserByName(userName).WithTryCatch();

            if (!userResponse.IsSuccessfull)
            {
                return BadRequest(userResponse.Exception.Message);
            }

            Global.User user = userResponse.Result;

            var productResponse = await productManager.GetProductById(id, user.Id).WithTryCatch();

            if (!userResponse.IsSuccessfull)
            {
                return BadRequest(productResponse.Exception.Message);
            }

            return Ok(productResponse.Result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFilteredProducts([FromQuery] Managers.Interfaces.ProductFilterRequest request)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .First()
                    .ErrorMessage;

                return BadRequest((new { errorMessage }));
            }

            string? userName = User.FindFirstValue(ClaimTypes.GivenName);

            var userResponse = await userManager.GetUserByName(userName).WithTryCatch();

            if (!userResponse.IsSuccessfull)
            {
                return BadRequest(userResponse.Exception.Message);
            }

            Global.User user = userResponse.Result;

            request.UserId = user.Id;

            var productResponse = await productManager.FilterProducts(request).WithTryCatch();

            if (!userResponse.IsSuccessfull)
            {
                return BadRequest(productResponse.Exception.Message);
            }

            return Ok(productResponse.Result);
        }

        [Authorize]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .First()
                    .ErrorMessage;

                return BadRequest((new { errorMessage }));
            }

            string? userName = User.FindFirstValue(ClaimTypes.GivenName);

            var userResponse = await userManager.GetUserByName(userName).WithTryCatch();

            if (!userResponse.IsSuccessfull)
            {
                return BadRequest(userResponse.Exception.Message);
            }

            Global.User user = userResponse.Result;

            var productResponse = await productManager.GetAllProducts(user.Id).WithTryCatch();

            if (!userResponse.IsSuccessfull)
            {
                return BadRequest(productResponse.Exception.Message);
            }

            return Ok(productResponse.Result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Managers.Interfaces.CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .First()
                    .ErrorMessage;

                return BadRequest((new { errorMessage }));
            }

            string? userName = User.FindFirstValue(ClaimTypes.GivenName);

            var userResponse = await userManager.GetUserByName(userName).WithTryCatch();

            if (!userResponse.IsSuccessfull)
            {
                return BadRequest(userResponse.Exception.Message);
            }

            Global.User user = userResponse.Result;

            request.UserId = user.Id;

            var productResponse = await productManager.AddProduct(request).WithTryCatch();

            if (!productResponse.IsSuccessfull)
            {
                return BadRequest(productResponse.Exception.Message);
            }

            return Created();
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .First()
                    .ErrorMessage;

                return BadRequest((new { errorMessage }));
            }

            string? userName = User.FindFirstValue(ClaimTypes.GivenName);

            var userResponse = await userManager.GetUserByName(userName).WithTryCatch();

            if (!userResponse.IsSuccessfull)
            {
                return BadRequest(userResponse.Exception.Message);
            }

            Global.User user = userResponse.Result;

            var productResponse = await productManager.DeleteProduct(id, user.Id).WithTryCatch();

            if (!userResponse.IsSuccessfull)
            {
                return BadRequest(productResponse.Exception.Message);
            }

            return Ok();
        }
    }
}
