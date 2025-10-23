using Honse.Global.Specification;
using Honse.Resources.Interface;
using Microsoft.EntityFrameworkCore;

namespace Honse.Resources.Common
{
    public class FilterResource<T> : Resource<T>, IFilterResource<T> where T : Entity
    {
        public FilterResource(AppDbContext appDbContext) : base(appDbContext)
        {
            
        }

        public async Task<PaginatedResult<T>> Filter(Specification<T> specification, int pageSize, int pageNumber)
        {
            List<T> entities = await dbSet
            .Where(specification.Expr)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            int count = await dbSet.Where(specification.Expr).CountAsync();

            return new PaginatedResult<T>
            {
                Result = entities,
                PageNumber = pageNumber,
                TotalCount = count
            };
        }
    }
}
