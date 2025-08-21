using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface ITransactionRepository
{

    Task AddTransactionAsync(Transaction transaction);

}