using Honse.Engines.Validation.Interface;
using System.ComponentModel.DataAnnotations;

namespace Honse.Engines.Validation.User
{
    public class UserValidationEngine : IUserValidationEngine
    {
        public void ValidateLogin(Common.UserLogin user)
        {
            string errorMessage = "";

            if (string.IsNullOrEmpty(user.UserName))
                errorMessage += "The username is not valid!\n";

            if (string.IsNullOrEmpty(user.Password))
                errorMessage += "The password is not valid!\n";

            if (!string.IsNullOrEmpty(errorMessage))
                throw new ValidationException(errorMessage);
        }

        public void ValidateRegister(Common.UserRegister user)
        {
            string errorMessage = "";

            ValidationContext validationContext = new ValidationContext(user) { MemberName = nameof(UserRegister.Email) };

            if (!Validator.TryValidateProperty(user.Email, validationContext, new List<ValidationResult>()))
                errorMessage += "The email is not valid!\n";

            if (string.IsNullOrEmpty(user.UserName))
                errorMessage += "The username is not valid!\n";

            if (user.Password != user.ConfirmPassword)
                errorMessage += "The passwords don't match!\n";

            if (!string.IsNullOrEmpty(errorMessage))
                throw new ValidationException(errorMessage);
        }
    }
}
