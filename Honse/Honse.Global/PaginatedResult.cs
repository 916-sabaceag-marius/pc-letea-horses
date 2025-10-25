
namespace Honse.Global
{
    public class PaginatedResult<T>
    {
        public List<T> Result { get; set; }

        public int PageNumber { get; set; }

        public int TotalCount { get; set; }
    }
}
