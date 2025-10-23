
namespace Honse.Resources.Interface
{
    public interface IFilterResource<T> : IResource<T> where T : Entity
    {
        public Task<PaginatedResult<T>> Filter(Global.Specification.Specification<T> specification, int pageSize, int pageNumber);
    }

    public class PaginatedResult<T>
    {
        public List<T> Result { get; set; }

        public int PageNumber { get; set; }

        public int TotalCount { get; set; }
    }
}
