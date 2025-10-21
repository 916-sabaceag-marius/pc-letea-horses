
using Honse.Global;
using Honse.Global.Extensions;
using Honse.Managers.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Honse.Managers
{
    public class UserManager : Interface.IUserManager
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<User> signInManager;

        public UserManager(UserManager<Global.User> userManager,
        IConfiguration configuration,
        SignInManager<Global.User> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        public Task<UserAuthenticationResponse> Login(UserRegisterRequest request)
        {

            throw new NotImplementedException();
        }

        public async Task<UserAuthenticationResponse> Register(UserRegisterRequest request)
        {
            //TODO: Add validation

            Global.User user = request.DeepCopyTo<Global.User>();

            var createdUser = await userManager.CreateAsync(user, request.Password);

            if (!createdUser.Succeeded)
            {
                throw new Exception(createdUser.Errors.Aggregate());
            }

            throw new NotImplementedException();
        }
    }
}
