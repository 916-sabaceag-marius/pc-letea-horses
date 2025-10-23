using Honse.Resources.Common;
using Honse.Resources.Interface;

namespace Honse.Resources
{
    public class ProductResource : FilterResource<Product>, IProductResource
    {
        public ProductResource(AppDbContext dbContext) : base(dbContext)
        {
            dbSet = dbContext.Products;
        }
    }
}
