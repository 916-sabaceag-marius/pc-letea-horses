
namespace Honse.Engines.Validation.Interface
{
    public interface IUserValidationEngine
    {
        void ValidateRegister(Common.UserRegister user);

        void ValidateLogin(Common.UserLogin user);
    }
}
