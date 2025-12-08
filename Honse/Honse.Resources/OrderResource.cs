using Honse.Resources.Interfaces;
using Honse.Resources.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;

namespace Honse.Resources
{
    public class OrderResource : IOrderResource
    {
        private readonly AppDbContext dbContext;
        private readonly DbSet<Order> dbSet;

        public OrderResource(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Orders;
        }

        public async Task<Order> Add(Order order)
        {
            await dbSet.AddAsync(order);
            await dbContext.SaveChangesAsync();
            return order;
        }
    }
}
