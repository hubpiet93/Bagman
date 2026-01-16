using Bagman.Domain.Models;
using Bagman.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    // Domain models — start with `User`. Add other DbSet<TEntity> as needed.
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // TODO: add DbSet<Table>, DbSet<Match>, DbSet<Bet>, DbSet<Pool>, DbSet<Stat>
    // and configure mappings (fluent API) to match the existing database schema.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure mapping for domain `User` to existing `users` table (snake_case columns)
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id).HasName("pk_users_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login)
                .HasColumnName("login")
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(512)
                .IsRequired(false);
        });

        modelBuilder.Entity<RefreshTokenEntity>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(e => e.Token).HasName("pk_refresh_tokens_token");
            entity.Property(e => e.Token).HasColumnName("token").HasMaxLength(512).IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.HasIndex(e => e.UserId).HasDatabaseName("ix_refresh_tokens_user_id");
        });

        // TODO: Add mappings for other domain entities (Table, Match, Bet, Pool, Stat)
    }
}
