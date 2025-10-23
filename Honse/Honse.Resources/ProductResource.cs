using Honse.Resources.Common;
using Honse.Resources.Interface;

namespace Honse.Resources
{
    public class ProductResource : FilterResource<Product>
    {
        public ProductResource(AppDbContext dbContext) : base(dbContext)
        {
            dbSet = dbContext.Products;
        }
    }
}
