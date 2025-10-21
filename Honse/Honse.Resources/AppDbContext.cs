using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Honse.Resources
{
    public class AppDbContext : IdentityDbContext<Honse.Global.User, IdentityRole<Guid>, Guid>
    {

    }
}
