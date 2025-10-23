
using Honse.Global.Specification;
using Honse.Resources.Interface;
using Microsoft.EntityFrameworkCore;

namespace Honse.Resources
{
    public class ProductResource : Resource<Interface.Product>, IFilterResource<Product>
    {
        public ProductResource(AppDbContext dbContext) : base(dbContext)
        {
            dbSet = dbContext.Products;
        }
        public async Task<PaginatedResult<Product>> Filter(Specification<Product> specification, int pageSize, int pageNumber)
        {
            List<Product> products = await dbSet
            .Where(specification.Expr)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            int count = await dbSet.Where(specification.Expr).CountAsync();

            return new PaginatedResult<Product>
            {
                Result = products,
                PageNumber = pageNumber,
                TotalCount = count
            };
        }
    }
}
