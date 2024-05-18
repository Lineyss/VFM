using Microsoft.EntityFrameworkCore;
using VFM.Models.Users;

namespace VFM.Models.Help
{
    public class LiteDbContext : DbContext
    {
        public LiteDbContext(DbContextOptions<LiteDbContext> options) : base(options)
        {
        }

        public DbSet<User> user { get; set; }
    }
}
