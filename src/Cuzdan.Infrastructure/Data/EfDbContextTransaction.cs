using Cuzdan.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cuzdan.Infrastructure.Data;

internal sealed class EfDbContextTransaction(IDbContextTransaction efTransaction) : ITransaction
{
    private readonly IDbContextTransaction _efTransaction = efTransaction;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _efTransaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _efTransaction.RollbackAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _efTransaction.DisposeAsync();
    }
}