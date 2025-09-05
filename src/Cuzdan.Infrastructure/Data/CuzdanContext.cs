using Cuzdan.Domain.Constants;
using Cuzdan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Cuzdan.Application.DTOs;
using Npgsql;
using Cuzdan.Domain.Enums;

namespace Cuzdan.Infrastructure.Data;

public class CuzdanContext(DbContextOptions<CuzdanContext> options) : DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresEnum<CurrencyType>();
        modelBuilder.HasPostgresEnum<TransactionStatus>();
        modelBuilder.HasPostgresEnum<TransactionType>();
        modelBuilder.HasPostgresEnum<UserRole>();

        modelBuilder.Entity<UserBalanceByCurrencyDto>().HasNoKey();

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
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasColumnType("user_role");

        modelBuilder.Entity<Wallet>()
            .Property(w => w.Balance)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<Wallet>()
            .Property(w => w.AvailableBalance)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<Transaction>()
            .Property(t => t.OriginalAmount)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = SystemConstants.SystemUserId,
                Name = "System",
                Email = "system@cuzdan.local",
                PasswordHash = string.Empty,
                Role = UserRole.System,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = SystemConstants.AdminUserId,
                Name = "Admin",
                Email = "admin@cuzdan.local",
                PasswordHash = "$2a$11$2PmkLLThOwinpu5hDWfCAel78J6gQSEaQt5T6P6jCJC0r2pv4VSja", // admin123
                Role = UserRole.Admin,
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
                Currency = CurrencyType.TRY,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
            
}