using Cuzdan.Application.Interfaces;
using Cuzdan.Infrastructure.Data;

namespace Cuzdan.Infrastructure.Repositories
{
    public class UnitOfWork(CuzdanContext context) : IUnitOfWork
    {
        private readonly CuzdanContext _context = context;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var efTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            return new EfDbContextTransaction(efTransaction);
        }
    }
}