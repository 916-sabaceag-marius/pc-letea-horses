using Honse.Resources.Interfaces;

namespace Honse.Resources
{
    public class ProductCategoryResource : Resource<Interfaces.Entities.ProductCategory>, IProductCategoryResource
    {
        public ProductCategoryResource(AppDbContext dbContext) : base(dbContext)
        {
            dbSet = dbContext.ProductCategory;
        }
    }
}
