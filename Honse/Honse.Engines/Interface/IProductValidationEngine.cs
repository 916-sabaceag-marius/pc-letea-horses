
namespace Honse.Engines.Interface
{
    public interface IProductValidationEngine
    {
        Task ValidateCreate(Product.Product product);

        Task ValidateUpdate(Guid id, Product.Product product);
    }
}
