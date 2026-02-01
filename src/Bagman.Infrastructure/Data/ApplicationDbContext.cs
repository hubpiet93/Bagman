using Bagman.Domain.Models;
using Bagman.Domain.Common.ValueObjects;
using Bagman.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    // Domain models
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!;
    public DbSet<Table> Tables { get; set; } = null!;
    public DbSet<Match> Matches { get; set; } = null!;
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

            // Relationships (TableMemberships and Bets are owned by Table and Match aggregates)
            entity.HasMany(e => e.CreatedTables)
                .WithOne(t => t.CreatedByUser)
                .HasForeignKey(t => t.CreatedBy)
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
            
            // Value object: TableName
            entity.OwnsOne(e => e.Name, name =>
            {
                name.Property(n => n.Value)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(100);
            });

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.MaxPlayers)
                .HasColumnName("max_players")
                .IsRequired();

            // Value object: Money (Stake)
            entity.OwnsOne(e => e.Stake, stake =>
            {
                stake.Property(s => s.Amount)
                    .HasColumnName("stake")
                    .HasPrecision(10, 2);
            });

            entity.Property(e => e.CreatedBy).HasColumnName("created_by");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(e => e.IsSecretMode)
                .HasColumnName("is_secret_mode")
                .HasDefaultValue(false);

            // Owned entities: TableMembers collection (use backing field for proper change tracking)
            entity.OwnsMany(e => e.Members, member =>
            {
                member.ToTable("table_members");
                member.WithOwner(m => m.Table)
                    .HasForeignKey(m => m.TableId);
                
                member.HasKey(nameof(TableMember.UserId), nameof(TableMember.TableId));
                
                member.Property(m => m.UserId).HasColumnName("user_id");
                member.Property(m => m.TableId).HasColumnName("table_id");
                member.Property(m => m.IsAdmin)
                    .HasColumnName("is_admin")
                    .HasDefaultValue(false);
                member.Property(m => m.JoinedAt).HasColumnName("joined_at");
                
                // Navigation to User (owned by Table aggregate)
                member.HasOne(m => m.User)
                    .WithMany()
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Indexes
                member.HasIndex(m => m.UserId).HasDatabaseName("idx_table_members_user_id");
                member.HasIndex(m => m.TableId).HasDatabaseName("idx_table_members_table_id");
            });

            // Relationships
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

        // Configure Match entity
        modelBuilder.Entity<Match>(entity =>
        {
            entity.ToTable("matches");
            entity.HasKey(e => e.Id).HasName("pk_matches_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TableId).HasColumnName("table_id");

            // Value object: Country1
            entity.OwnsOne(e => e.Country1, country =>
            {
                country.Property(c => c.Name)
                    .HasColumnName("country_1")
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Value object: Country2
            entity.OwnsOne(e => e.Country2, country =>
            {
                country.Property(c => c.Name)
                    .HasColumnName("country_2")
                    .IsRequired()
                    .HasMaxLength(100);
            });

            entity.Property(e => e.MatchDateTime)
                .HasColumnName("match_datetime");

            // Value object: Score (Result)
            entity.OwnsOne(e => e.Result, result =>
            {
                result.Property(r => r.Value)
                    .HasColumnName("result")
                    .HasMaxLength(10)
                    .IsRequired(false);
            });

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .HasDefaultValue("scheduled");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            // Owned entities: Bets collection (use backing field for proper change tracking)
            entity.OwnsMany(e => e.Bets, bet =>
            {
                bet.ToTable("bets");
                bet.WithOwner(b => b.Match)
                    .HasForeignKey(b => b.MatchId);
                
                bet.HasKey(nameof(Bet.Id));
                bet.Property(b => b.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();  // CRITICAL: Auto-generate Guid for new bets
                bet.Property(b => b.UserId).HasColumnName("user_id");
                bet.Property(b => b.MatchId).HasColumnName("match_id");
                
                // Value object: Prediction
                bet.OwnsOne(b => b.Prediction, prediction =>
                {
                    prediction.Property(p => p.Value)
                        .HasColumnName("prediction")
                        .IsRequired()
                        .HasMaxLength(10);
                });
                
                bet.Property(b => b.EditedAt).HasColumnName("edited_at");
                
                // Navigation to User (owned by Match aggregate)
                bet.HasOne(b => b.User)
                    .WithMany()
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Unique constraint
                bet.HasIndex(b => new { b.UserId, b.MatchId })
                    .IsUnique()
                    .HasDatabaseName("uk_bets_user_match");
                
                // Indexes
                bet.HasIndex(b => b.UserId).HasDatabaseName("idx_bets_user_id");
                bet.HasIndex(b => b.MatchId).HasDatabaseName("idx_bets_match_id");
            });

            // Relationships
            entity.HasMany(e => e.Pools)
                .WithOne(p => p.Match)
                .HasForeignKey(p => p.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.TableId).HasDatabaseName("idx_matches_table_id");
            entity.HasIndex(e => e.MatchDateTime).HasDatabaseName("idx_matches_datetime");
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
            entity.HasKey(e => new {e.PoolId, e.UserId}).HasName("pk_pool_winners");

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
            entity.HasKey(e => new {e.UserId, e.TableId}).HasName("pk_user_stats");

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
