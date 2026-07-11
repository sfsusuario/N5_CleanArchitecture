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

            // PermissionDate is a calendar date with no time-of-day/timezone meaning, and the
            // frontend sends a plain date string that deserializes with DateTime.Kind=Unspecified.
            // Npgsql 6+ throws at runtime if an Unspecified-kind DateTime is written to
            // "timestamp with time zone" (its default mapping for DateTime), so map this
            // column as Postgres "date" instead — sidesteps the Kind check entirely and is the
            // more correct type for the data anyway.
            modelBuilder.Entity<Permissions>()
                .Property(p => p.PermissionDate)
                .HasColumnType("date");

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
