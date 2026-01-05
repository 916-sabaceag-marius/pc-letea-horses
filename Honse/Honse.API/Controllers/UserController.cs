using Honse.Global.Extensions;
using Honse.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Honse.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager userManager;

        public UserController(Managers.Interfaces.IUserManager userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .First()
                    .ErrorMessage;

                return BadRequest((new { errorMessage }));
            }

            var response = await userManager.Register(request).WithTryCatch();

            if (!response.IsSuccessfull)
            {
                return BadRequest(response.Exception.Message);
            }

            return Ok(response.Result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginRequest request)
        {

            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .First()
                    .ErrorMessage;

                return BadRequest((new { errorMessage = errorMessage }));
            }

            var response = await userManager.Login(request).WithTryCatch();

            if (!response.IsSuccessfull)
            {
                return BadRequest(response.Exception.Message);
            }

            return Ok(response.Result);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var username =
                User.FindFirst(ClaimTypes.GivenName)?.Value ??
                User.FindFirst("username")?.Value ??
                User.Identity?.Name;

            var email =
                User.FindFirst(ClaimTypes.Email)?.Value ??
                User.FindFirst("email")?.Value;


            return Ok(new
            {
                username,
                email
            });
        }

    }
}
