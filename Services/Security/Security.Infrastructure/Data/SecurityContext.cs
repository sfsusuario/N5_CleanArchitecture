using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;

namespace Security.Infrastructure.Data
{
    /// <summary>
    /// Security context for repositories
    /// </summary>
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

        /// <summary>
        /// Permissions repository
        /// </summary>
        public DbSet<Permissions> Permissions { get; set; }

        /// <summary>
        /// Permissions customers
        /// </summary>
        public DbSet<PermissionsType> Customers { get; set; }
    }
}
