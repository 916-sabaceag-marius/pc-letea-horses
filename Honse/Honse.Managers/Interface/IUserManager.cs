using System.ComponentModel.DataAnnotations;

namespace Honse.Managers.Interface
{
    public interface IUserManager
    {
        public Task<UserAuthenticationResponse> Register(UserRegisterRequest request);

        public Task<UserAuthenticationResponse> Login(UserLoginRequest request);

        public Task<Global.User> GetUserByName(string userName);
    }

    public class UserRegisterRequest
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class UserLoginRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class UserAuthenticationResponse
    {
        public bool Succeeded { get; set; }

        public string Token { get; set; }

        public string Username { get; set; }
    }
}
