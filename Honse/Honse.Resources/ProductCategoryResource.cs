using Honse.Resources.Common;
using Honse.Resources.Interface;

namespace Honse.Resources
{
    public class ProductCategoryResource : Resource<ProductCategory>, IProductCategoryResource
    {
        public ProductCategoryResource(AppDbContext dbContext) : base(dbContext)
        {
            dbSet = dbContext.ProductCategories;
        }
    }
}
