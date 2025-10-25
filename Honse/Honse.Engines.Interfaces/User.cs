using System.ComponentModel.DataAnnotations;

namespace Honse.Engines.Common
{
    public class UserLogin
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class UserRegister
    {
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
