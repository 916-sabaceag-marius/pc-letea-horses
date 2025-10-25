using Honse.Resources.Interfaces;

namespace Honse.Resources
{
    public class ProductResource : FilterResource<Interfaces.Entities.Product>, IProductResource
    {
        public ProductResource(AppDbContext dbContext) : base(dbContext)
        {
            dbSet = dbContext.Product;
        }
    }
}
