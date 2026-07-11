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

            // Speeds up the dispatcher's "pending messages" poll (WHERE ProcessedAt IS NULL).
            modelBuilder.Entity<OutboxMessage>()
                .HasIndex(m => m.ProcessedAt);
        }

        /// <summary>
        /// Permissions repository
        /// </summary>
        public DbSet<Permissions> Permissions { get; set; }

        /// <summary>
        /// Permission types lookup table (Vacation, Sick leave, etc.)
        /// </summary>
        public DbSet<PermissionsType> PermissionTypes { get; set; }

        /// <summary>
        /// Outbox pattern table: see <see cref="OutboxMessage"/>.
        /// </summary>
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}
