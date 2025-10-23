using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Honse.Resources
{
    public class AppDbContext : IdentityDbContext<Honse.Global.User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Resources.Interface.Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> context) : base(context)
        {
        }
    }
}
