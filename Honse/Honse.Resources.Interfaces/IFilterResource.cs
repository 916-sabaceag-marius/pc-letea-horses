
namespace Honse.Resources.Interfaces
{
    public interface IFilterResource<T> : IResource<T> where T : Entities.Entity
    {
        public Task<Global.PaginatedResult<T>> Filter(Global.Specification.Specification<T> specification, int pageSize, int pageNumber);
    }

    
}
