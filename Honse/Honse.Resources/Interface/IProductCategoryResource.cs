
namespace Honse.Resources.Interface
{
    public interface IProductCategoryResource : IResource<ProductCategory>
    {
    }

    public class ProductCategory : Entity
    {
        public string Name { get; set; } = string.Empty;
    }
}
