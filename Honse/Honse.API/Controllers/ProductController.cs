using Honse.Global.Extensions;
using Honse.Managers.Interface;
using Honse.Resources.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Honse.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUserManager userManager;
        private readonly IProductResource productResource;

        public ProductController(Managers.Interface.IUserManager userManager, Resources.Interface.IProductResource productResource)
        {
            this.userManager = userManager;
            this.productResource = productResource;
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

            try
            {
                Resources.Interface.Product? response = await productResource.GetById(id, user.Id);
                
                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Managers.Interface.Product request)
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
            request.Id = Guid.NewGuid();

            try
            {
                Resources.Interface.Product? response = await productResource.Add(request.DeepCopyTo<Resources.Interface.Product>());

                return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
    }
}
