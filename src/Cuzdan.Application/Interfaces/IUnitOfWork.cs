namespace Cuzdan.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        IWalletRepository Wallets { get; }
        ITransactionRepository Transactions { get; }
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
    }
}