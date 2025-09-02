using Cuzdan.Application.Interfaces;
using Cuzdan.Infrastructure.Data;

namespace Cuzdan.Infrastructure.Repositories
{
    public class UnitOfWork(
        CuzdanContext context,
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository) : IUnitOfWork, IDisposable
    {
        public IWalletRepository Wallets { get; } = walletRepository;
        public ITransactionRepository Transactions { get; } = transactionRepository;
        public IUserRepository Users { get; } = userRepository;
        public IRefreshTokenRepository RefreshTokens { get; } = refreshTokenRepository;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
        public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var efTransaction = await context.Database.BeginTransactionAsync(cancellationToken);

            return new EfDbContextTransaction(efTransaction);
        }

        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                context.Dispose();
            }
            _disposed = true;
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}