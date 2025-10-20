
namespace Honse.Resources.Interface
{
    public interface IResource<T>
    {
        public Task<IEnumerable<T>> GetAll();

        public Task<T> GetById(Guid id, Guid userId);

        public Task<T> GetByIdNoTracking(Guid id, Guid userId);

        public Task<T> Add(T t);

        public Task<T> Update(T t);

        public Task<T> Delete(T t);
    }
}
