using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Honse.Resources.Common
{
    public class AppDbContext : IdentityDbContext<Global.User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Interface.Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> context) : base(context)
        {
        }
    }
}
