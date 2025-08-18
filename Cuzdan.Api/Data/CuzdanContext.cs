using Microsoft.EntityFrameworkCore;
using Cuzdan.Api.Models;

namespace Cuzdan.Api.Data;

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

        // Transaction.Amount sütununun tipini ayarla
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasColumnType("decimal(18, 2)");
    }


    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
            
}