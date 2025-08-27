using Cuzdan.Domain.Constants;
using Cuzdan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Data;

public class CuzdanContext(DbContextOptions<CuzdanContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasOne(t => t.From)
                  .WithMany(w => w.SentTransactions)
                  .HasForeignKey(t => t.FromId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.To)
                  .WithMany(w => w.ReceivedTransactions)
                  .HasForeignKey(t => t.ToId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Wallet>()
            .Property(w => w.Balance)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<Wallet>()
            .Property(w => w.AvailableBalance)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = SystemConstants.SystemUserId,
                Name = "System",
                Email = "system@cuzdan.local",
                PasswordHash = string.Empty,
                Role = "System",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Wallet>().HasData(
            new Wallet
            {
                Id = SystemConstants.SystemWalletId,
                UserId = SystemConstants.SystemUserId,
                Balance = 0,
                AvailableBalance = 0,
                WalletName = "System Wallet",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
            
}