using Honse.Global.Specification;
using Microsoft.EntityFrameworkCore;

namespace Honse.Resources.Interfaces
{
    public class FilterResource<T> : Resource<T>, IFilterResource<T> where T : Entities.Entity
    {
        public FilterResource(AppDbContext appDbContext) : base(appDbContext)
        {
            
        }

        public async Task<Global.PaginatedResult<T>> Filter(Specification<T> specification, int pageSize, int pageNumber)
        {
            List<T> entities = await dbSet
            .Where(specification.Expression)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            int count = await dbSet.Where(specification.Expression).CountAsync();

            return new Global.PaginatedResult<T>
            {
                Result = entities,
                PageNumber = pageNumber,
                TotalCount = count
            };
        }
    }
}
