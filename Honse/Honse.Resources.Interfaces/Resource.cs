using Microsoft.EntityFrameworkCore;

namespace Honse.Resources.Interfaces
{
    public abstract class Resource<T> : IResource<T> where T : Entities.Entity
    {
        private readonly AppDbContext dbContext;

        protected DbSet<T> dbSet;

        public Resource(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<T> Add(T t)
        {
            await dbSet.AddAsync(t);

            await dbContext.SaveChangesAsync();

            return t;
        }

        public async Task<bool> Delete(Guid id, Guid userId)
        {

            T? t = await dbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (t == null)
                return false;

            dbSet.Remove(t);

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<T>> GetAll(Guid userId)
        {
            return await dbSet.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<T?> GetById(Guid id, Guid userId)
        {
            return await dbSet.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<T?> Update(Guid id, Guid userId, T t)
        {
            T? existingT = await dbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (existingT == null)
                return null;

            dbSet.Update(t);

            await dbContext.SaveChangesAsync();

            return t;
        }
    }
}
