namespace Honse.Resources.Interfaces
{
    public interface IOrderResource
    {
        Task<Entities.Order> Add(Entities.Order order);
    }
}
