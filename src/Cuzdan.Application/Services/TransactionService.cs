using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;
using Cuzdan.Application.Extensions;

namespace Cuzdan.Application.Services;


public class TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, IUnitOfWork unitOfWork) : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly IWalletRepository _walletRepository = walletRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ApiResponse> TransferTransactionAsync(TransactionDto transactionDto, Guid UserId)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {


            var senderWallet = await _walletRepository.GetByIdAsync(transactionDto.FromId);

            var receiverWallet = await _walletRepository.GetByIdAsync(transactionDto.ToId);

            if (senderWallet == null || receiverWallet == null) throw new NotFoundException("Wallet not found.");

            if (senderWallet.UserId != UserId) throw new ForbiddenAccessException("Not your wallet");


            if (senderWallet.Balance < transactionDto.Amount) throw new InsufficientBalanceException("Not enough balance");


            if (transactionDto.Amount <= 0 | transactionDto.Amount > 1000000) throw new ArgumentOutOfRangeException("transactionDto.Amount", "Invalid Amount");




            senderWallet.Balance -= transactionDto.Amount;
            receiverWallet.Balance += transactionDto.Amount;

            var newTransaction = new Transaction
            {
                FromId = transactionDto.FromId,
                ToId = transactionDto.ToId,
                Amount = transactionDto.Amount,
            };
            await _transactionRepository.AddAsync(newTransaction);
            await _unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync();

            var response = new ApiResponse
            {
                IsSuccessful = true,
                SuccessMessage = "Transaction done",
            };

            return response;

        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

    }

    public async Task<PagedResult<TransactionDto>> GetTransactionsByWalletAsync(
        Guid userId,
        Guid walletId,
        TransactionFilterDto filter)
    {
        if (filter.PageNumber < 1) filter.PageNumber = 1;
        if (filter.PageSize < 1 || filter.PageSize > 100) filter.PageSize = 10;

        var walletBelongsToUser = await _walletRepository.DoesWalletBelongToUserAsync(walletId, userId);
        
        if (!walletBelongsToUser)
        {
            throw new NotFoundException("Wallet not found or access is denied.");
        }

        var predicate = PredicateBuilder.True<Transaction>();
        predicate = (filter.Type?.ToLowerInvariant()) switch
        {
            "sent" => predicate.And(t => t.FromId == walletId),
            "received" => predicate.And(t => t.ToId == walletId),
            _ => predicate.And(t => t.FromId == walletId || t.ToId == walletId),
        };

        if (filter.MinAmount.HasValue)
        {
            predicate = predicate.And(t => t.Amount >= filter.MinAmount.Value);
        }

        if (filter.MaxAmount.HasValue)
        {
            predicate = predicate.And(t => t.Amount <= filter.MaxAmount.Value);
        }

        if (filter.StartDate.HasValue)
        {
            predicate = predicate.And(t => t.CreatedAt >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            var endDate = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            predicate = predicate.And(t => t.CreatedAt <= endDate);
        }
        
        var pagedTransactions = await _transactionRepository.FindAsync(
            predicate: predicate,
            pageNumber: filter.PageNumber,
            pageSize: filter.PageSize,
            orderBy: t => t.CreatedAt,
            isDescending: true
        );



        var transactionDtos = (pagedTransactions.Items ?? Enumerable.Empty<Transaction>()).Select(t => new TransactionDto
        {
            Amount = t.Amount,
            CreatedAt = t.CreatedAt,
            FromId = t.FromId,
            ToId = t.ToId,
        }).ToList();

        return new PagedResult<TransactionDto>
        {
            Page = pagedTransactions.Page,
            PageSize = pagedTransactions.PageSize,
            TotalCount = pagedTransactions.TotalCount,
            Items = transactionDtos
        };

    }


}