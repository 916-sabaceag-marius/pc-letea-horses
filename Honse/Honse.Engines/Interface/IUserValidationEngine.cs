
using System.ComponentModel.DataAnnotations;

namespace Honse.Engines.Interface
{
    public interface IUserValidationEngine
    {
        void ValidateRegister(User.UserRegister user);

        void ValidateLogin(User.UserLogin user);
    }
}
