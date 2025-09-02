using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;
using Cuzdan.Application.Extensions;
using System.Linq.Expressions;
using Cuzdan.Domain.Enums;
using Cuzdan.Domain.Constants;

namespace Cuzdan.Application.Services;


public class TransactionService(
    IPaymentGatewayService paymentGatewayService,
    IUnitOfWork unitOfWork, 
    ICurrencyConversionService currencyConversionService) : ITransactionService
{

    public async Task<ApiResponse> TransferTransactionAsync(CreateTransactionDto transactionDto, Guid UserId)
    {

        var senderWallet = await unitOfWork.Wallets.GetByIdAsync(transactionDto.FromId);

        var receiverWallet = await unitOfWork.Wallets.GetByIdAsync(transactionDto.ToId);

        if (senderWallet == null || receiverWallet == null) throw new NotFoundException("Wallet not found.");

        if (senderWallet.UserId != UserId) throw new ForbiddenAccessException("Not your wallet");

        if (senderWallet.AvailableBalance < transactionDto.Amount) throw new InsufficientBalanceException("Not enough balance");

        var conversionRate = await currencyConversionService.GetConversionRateAsync(senderWallet.Currency, receiverWallet.Currency);

        await unitOfWork.Transactions.TransferTransactionAsync(transactionDto.FromId, transactionDto.ToId, transactionDto.Amount, conversionRate);

        var response = new ApiResponse
        {
            IsSuccessful = true,
            SuccessMessage = "Transaction done",
        };

        return response;
    }

    public async Task<PagedResult<TransactionDto>> GetTransactionsByWalletAsync(
        Guid userId,
        Guid walletId,
        TransactionFilterDto filter)
    {
        if (filter.PageNumber < 1) filter.PageNumber = 1;
        if (filter.PageSize < 1 || filter.PageSize > 100) filter.PageSize = 10;

        var walletBelongsToUser = await unitOfWork.Wallets.DoesWalletBelongToUserAsync(walletId, userId);

        if (!walletBelongsToUser) throw new NotFoundException("Wallet not found or access is denied.");


        var predicate = PredicateBuilder.True<Transaction>();
        predicate = predicate.And(t => t.FromId == walletId || t.ToId == walletId);
        if (filter.Status.HasValue)
        {
            predicate = predicate.And(t => t.Status == filter.Status.Value);
        }

        // Type filtresinin doğru hali:
        if (filter.Type.HasValue)
        {
            predicate = predicate.And(t => t.Type == filter.Type.Value);
        }

        if (filter.MinAmount.HasValue)
        {
            predicate = predicate.And(t => t.OriginalAmount >= filter.MinAmount.Value);
        }

        if (filter.MaxAmount.HasValue)
        {
            predicate = predicate.And(t => t.OriginalAmount <= filter.MaxAmount.Value);
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

        Expression<Func<Transaction, object>>? orderByExpr = filter.OrderBy switch
        {
            TransactionSortField.Amount => t => t.OriginalAmount,
            TransactionSortField.CreatedAt => t => t.CreatedAt,
            null => t => t.CreatedAt,
            _ => t => t.CreatedAt,
        };

        var pagedTransactions = await unitOfWork.Transactions.FindAsync(
            predicate: predicate,
            pageNumber: filter.PageNumber,
            pageSize: filter.PageSize,
            orderBy: orderByExpr,
            isDescending: filter.IsDescending ?? true
        );
        


        var transactionDtos = (pagedTransactions.Items ?? Enumerable.Empty<Transaction>()).Select(t => new TransactionDto
        {
            Id = t.Id,
            FromId = t.FromId,
            ToId = t.ToId,
            Amount = t.OriginalAmount,
            OriginalCurrency = t.OriginalCurrency.ToString(),
            ConvertedAmount = t.ConvertedAmount,
            TargetCurrency = t.TargetCurrency.ToString(),
            ConversionRate = t.ConversionRate,
            CreatedAt = t.CreatedAt,
            Status = t.Status.ToString(),
            Type = t.Type.ToString()

        }).ToList();

        return new PagedResult<TransactionDto>
        {
            Page = pagedTransactions.Page,
            PageSize = pagedTransactions.PageSize,
            TotalCount = pagedTransactions.TotalCount,
            Items = transactionDtos
        };

    }

    public async Task RequestDepositAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto depositRequestDto)
    {
        var wallet = await unitOfWork.Wallets.GetByIdForWriteAsync(walletId);


        if (wallet == null) throw new NotFoundException("Wallet not found.");

        if (wallet.UserId != userId) throw new ForbiddenAccessException("Not your wallet");

        var newTransaction = new Transaction
        {
            FromId = SystemConstants.SystemWalletId,
            ToId = walletId,
            OriginalAmount = depositRequestDto.Amount,
            OriginalCurrency = wallet.Currency,
            ConvertedAmount = depositRequestDto.Amount,
            TargetCurrency = wallet.Currency,
            ConversionRate = 1,
            Status = TransactionStatus.Pending,
            Type = TransactionType.Deposit
        };
        await unitOfWork.Transactions.AddAsync(newTransaction);
        await unitOfWork.SaveChangesAsync();
        await paymentGatewayService.InitiatePayment(newTransaction.Id);
    }
    public async Task RequestWithdrawalAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto withdrawalRequestDto)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var wallet = await unitOfWork.Wallets.GetByIdForWriteAsync(walletId);

            if (wallet == null) throw new NotFoundException("Wallet not found.");

            if (wallet.UserId != userId) throw new ForbiddenAccessException("Not your wallet");


            if (wallet.AvailableBalance < withdrawalRequestDto.Amount) throw new InsufficientBalanceException("Insufficient Balance");
            wallet.AvailableBalance -= withdrawalRequestDto.Amount;

            var newTransaction = new Transaction
            {
                FromId = SystemConstants.SystemWalletId,
                ToId = walletId,
                OriginalAmount = withdrawalRequestDto.Amount,
                OriginalCurrency = wallet.Currency,
                ConvertedAmount = withdrawalRequestDto.Amount,
                TargetCurrency = wallet.Currency,
                ConversionRate = 1,
                Status = TransactionStatus.Pending,
                Type = TransactionType.Withdrawal
            };

            await unitOfWork.Transactions.AddAsync(newTransaction);
            await unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();

            await paymentGatewayService.InitiatePayment(newTransaction.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task FinalizePaymentAsync(Guid transactionId, bool isSuccessful)
    {
        await using var dbTransaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var transaction = await unitOfWork.Transactions.GetByIdForWriteAsync(transactionId);

            if (transaction is null) throw new NotFoundException("Transaction not found.");
            if (transaction.Status != TransactionStatus.Pending) throw new InvalidOperationException("Transaction is not in a pending state.");

            var wallet = await unitOfWork.Wallets.GetByIdForWriteAsync(transaction.ToId);
            if (wallet is null)
            {
                transaction.Status = TransactionStatus.Failed;
                await unitOfWork.SaveChangesAsync();
                throw new NotFoundException("CRITICAL: Wallet associated with the transaction was not found.");
            }

            if (isSuccessful)
            {


                if (transaction.Type == TransactionType.Deposit)
                {
                    wallet.Balance += transaction.OriginalAmount;
                    wallet.AvailableBalance += transaction.OriginalAmount;
                    transaction.Status = TransactionStatus.Completed;
                }
                else if (transaction.Type == TransactionType.Withdrawal)
                {
                    wallet.Balance -= transaction.OriginalAmount;
                    transaction.Status = TransactionStatus.Completed;
                }
                else
                {

                    throw new NotFoundException("CRITICAL: Transaction Type error");
                }
            }
            else if (!isSuccessful)
            {
                transaction.Status = TransactionStatus.Failed;
                if (transaction.Type == TransactionType.Withdrawal)
                {
                    wallet.AvailableBalance += transaction.OriginalAmount;
                }
            }
            await unitOfWork.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
}