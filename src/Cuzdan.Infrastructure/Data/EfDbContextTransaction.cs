using Cuzdan.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cuzdan.Infrastructure.Data;

internal sealed class EfDbContextTransaction(IDbContextTransaction efTransaction) : ITransaction
{

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await efTransaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await efTransaction.RollbackAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await efTransaction.DisposeAsync();
    }
}