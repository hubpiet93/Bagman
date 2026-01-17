using Bagman.Domain.Models;
using Bagman.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    // Domain models
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!;
    public DbSet<Table> Tables { get; set; } = null!;
    public DbSet<TableMember> TableMembers { get; set; } = null!;
    public DbSet<Match> Matches { get; set; } = null!;
    public DbSet<Bet> Bets { get; set; } = null!;
    public DbSet<Pool> Pools { get; set; } = null!;
    public DbSet<PoolWinner> PoolWinners { get; set; } = null!;
    public DbSet<UserStats> UserStats { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

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

            // Relationships
            entity.HasMany(e => e.TableMemberships)
                .WithOne(tm => tm.User)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.CreatedTables)
                .WithOne(t => t.CreatedByUser)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Bets)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Stats)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.PoolWinnings)
                .WithOne(pw => pw.User)
                .HasForeignKey(pw => pw.UserId)
                .OnDelete(DeleteBehavior.Cascade);
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

        // Configure Table entity
        modelBuilder.Entity<Table>(entity =>
        {
            entity.ToTable("tables");
            entity.HasKey(e => e.Id).HasName("pk_tables_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.MaxPlayers)
                .HasColumnName("max_players")
                .IsRequired();

            entity.Property(e => e.Stake)
                .HasColumnName("stake")
                .HasPrecision(10, 2);

            entity.Property(e => e.CreatedBy).HasColumnName("created_by");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(e => e.IsSecretMode)
                .HasColumnName("is_secret_mode")
                .HasDefaultValue(false);

            // Relationships
            entity.HasMany(e => e.Members)
                .WithOne(tm => tm.Table)
                .HasForeignKey(tm => tm.TableId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Matches)
                .WithOne(m => m.Table)
                .HasForeignKey(m => m.TableId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.UserStats)
                .WithOne(s => s.Table)
                .HasForeignKey(s => s.TableId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            entity.HasIndex(e => e.CreatedBy).HasDatabaseName("idx_tables_created_by");
        });

        // Configure TableMember entity
        modelBuilder.Entity<TableMember>(entity =>
        {
            entity.ToTable("table_members");
            entity.HasKey(e => new { e.UserId, e.TableId }).HasName("pk_table_members");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.IsAdmin)
                .HasColumnName("is_admin")
                .HasDefaultValue(false);

            entity.Property(e => e.JoinedAt)
                .HasColumnName("joined_at");

            // Indexes
            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_table_members_user_id");
            entity.HasIndex(e => e.TableId).HasDatabaseName("idx_table_members_table_id");
        });

        // Configure Match entity
        modelBuilder.Entity<Match>(entity =>
        {
            entity.ToTable("matches");
            entity.HasKey(e => e.Id).HasName("pk_matches_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TableId).HasColumnName("table_id");

            entity.Property(e => e.Country1)
                .HasColumnName("country_1")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Country2)
                .HasColumnName("country_2")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.MatchDateTime)
                .HasColumnName("match_datetime");

            entity.Property(e => e.Result)
                .HasColumnName("result")
                .HasMaxLength(10)
                .IsRequired(false);

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .HasDefaultValue("scheduled");

            entity.Property(e => e.Started)
                .HasColumnName("started")
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            // Relationships
            entity.HasMany(e => e.Bets)
                .WithOne(b => b.Match)
                .HasForeignKey(b => b.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Pools)
                .WithOne(p => p.Match)
                .HasForeignKey(p => p.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.TableId).HasDatabaseName("idx_matches_table_id");
            entity.HasIndex(e => e.MatchDateTime).HasDatabaseName("idx_matches_datetime");
        });

        // Configure Bet entity
        modelBuilder.Entity<Bet>(entity =>
        {
            entity.ToTable("bets");
            entity.HasKey(e => e.Id).HasName("pk_bets_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.MatchId).HasColumnName("match_id");

            entity.Property(e => e.Prediction)
                .HasColumnName("prediction")
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at");

            // Unique constraint on (user_id, match_id)
            entity.HasIndex(e => new { e.UserId, e.MatchId })
                .IsUnique()
                .HasDatabaseName("uk_bets_user_match");

            // Indexes
            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_bets_user_id");
            entity.HasIndex(e => e.MatchId).HasDatabaseName("idx_bets_match_id");
        });

        // Configure Pool entity
        modelBuilder.Entity<Pool>(entity =>
        {
            entity.ToTable("pools");
            entity.HasKey(e => e.Id).HasName("pk_pools_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MatchId).HasColumnName("match_id");

            entity.Property(e => e.Amount)
                .HasColumnName("amount")
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .HasDefaultValue("active");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            // Relationships
            entity.HasMany(e => e.Winners)
                .WithOne(pw => pw.Pool)
                .HasForeignKey(pw => pw.PoolId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            entity.HasIndex(e => e.MatchId).HasDatabaseName("idx_pools_match_id");
        });

        // Configure PoolWinner entity
        modelBuilder.Entity<PoolWinner>(entity =>
        {
            entity.ToTable("pool_winners");
            entity.HasKey(e => new { e.PoolId, e.UserId }).HasName("pk_pool_winners");

            entity.Property(e => e.PoolId).HasColumnName("pool_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.Property(e => e.AmountWon)
                .HasColumnName("amount_won")
                .HasPrecision(10, 2);
        });

        // Configure UserStats entity
        modelBuilder.Entity<UserStats>(entity =>
        {
            entity.ToTable("user_stats");
            entity.HasKey(e => new { e.UserId, e.TableId }).HasName("pk_user_stats");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");

            entity.Property(e => e.MatchesPlayed)
                .HasColumnName("matches_played")
                .HasDefaultValue(0);

            entity.Property(e => e.BetsPlaced)
                .HasColumnName("bets_placed")
                .HasDefaultValue(0);

            entity.Property(e => e.PoolsWon)
                .HasColumnName("pools_won")
                .HasDefaultValue(0);

            entity.Property(e => e.TotalWon)
                .HasColumnName("total_won")
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            // Indexes
            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_user_stats_user_id");
            entity.HasIndex(e => e.TableId).HasDatabaseName("idx_user_stats_table_id");
        });
    }
}
