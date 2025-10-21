using System.ComponentModel.DataAnnotations;

namespace Honse.Managers.Interface
{
    public interface IUserManager
    {
        public Task<UserAuthenticationResponse> Register(UserRegisterRequest request);

        public Task<UserAuthenticationResponse> Login(UserRegisterRequest request);

    }

    public class UserRegisterRequest
    {
        public string UserName { get; set; }

        [EmailAddress]
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
        public required bool Succeeded { get; set; }

        public required string Token { get; set; }

        public required string Username { get; set; }
    }
}
