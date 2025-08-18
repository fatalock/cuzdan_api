using Cuzdan.Api.Schemas;

namespace Cuzdan.Api.Interfaces;

public interface ITransactionService
{
    Task<ApiResponse> TransferTransactionAsync(TransactionDto TransactionDto, Guid UserId);


}