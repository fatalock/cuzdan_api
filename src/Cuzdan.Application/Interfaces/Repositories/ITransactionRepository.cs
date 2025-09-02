using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task TransferTransactionAsync(Guid FromId, Guid ToId, decimal Amount, decimal conversionRate);
    
}