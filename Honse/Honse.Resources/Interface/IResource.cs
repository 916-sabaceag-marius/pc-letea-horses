
namespace Honse.Resources.Interface
{
    public interface IResource<T> where T : Entity
    {
        public Task<IEnumerable<T>> GetAll(Guid userId);

        public Task<T?> GetById(Guid id, Guid userId);

        public Task<T> Add(T t);

        public Task<T?> Update(Guid id, Guid userId, T t);

        public Task<bool> Delete(Guid id, Guid userId);
    }

    public abstract class Entity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
    }
}
