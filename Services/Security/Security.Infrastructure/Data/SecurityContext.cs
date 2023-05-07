using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;

namespace Security.Infrastructure.Data
{
    public class SecurityContext : DbContext
    {
        public SecurityContext(DbContextOptions<SecurityContext> options) : base(options)
        {

        }

        // Specify DbSet properties etc
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permissions>()
            .HasOne(e => e.PermissionTypeRef);
        }

        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<PermissionsType> Customers { get; set; }
    }
}
