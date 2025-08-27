using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;
using Cuzdan.Application.Extensions;
using System.Linq.Expressions;
using Cuzdan.Domain.Enums;
using Cuzdan.Domain.Constants;

namespace Cuzdan.Application.Services;


public class TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, IPaymentGatewayService paymentGatewayService, IUnitOfWork unitOfWork) : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly IWalletRepository _walletRepository = walletRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPaymentGatewayService _paymentGatewayService = paymentGatewayService;

    public async Task<ApiResponse> TransferTransactionAsync(TransactionDto transactionDto, Guid UserId)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var senderWallet = await _walletRepository.GetByIdForWriteAsync(transactionDto.FromId);

            var receiverWallet = await _walletRepository.GetByIdForWriteAsync(transactionDto.ToId);

            if (senderWallet == null || receiverWallet == null) throw new NotFoundException("Wallet not found.");

            if (senderWallet.UserId != UserId) throw new ForbiddenAccessException("Not your wallet");


            if (senderWallet.AvailableBalance < transactionDto.Amount) throw new InsufficientBalanceException("Not enough balance");


            if (transactionDto.Amount <= 0 | transactionDto.Amount > 1000000) throw new ArgumentOutOfRangeException("transactionDto.Amount", "Invalid Amount");



            senderWallet.AvailableBalance -= transactionDto.Amount;
            senderWallet.Balance -= transactionDto.Amount;
            
            receiverWallet.AvailableBalance += transactionDto.Amount;
            receiverWallet.Balance += transactionDto.Amount;

            var newTransaction = new Transaction
            {
                FromId = transactionDto.FromId,
                ToId = transactionDto.ToId,
                Amount = transactionDto.Amount,
                Status = TransactionStatus.Completed,
                Type = TransactionType.Transfer
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

        Expression<Func<Transaction, object>>? orderByExpr = filter.OrderBy switch
        {
            TransactionSortField.Amount => t => t.Amount,
            TransactionSortField.CreatedAt => t => t.CreatedAt,
            null => t => t.CreatedAt,
            _ => t => t.CreatedAt,
        };

        var pagedTransactions = await _transactionRepository.FindAsync(
            predicate: predicate,
            pageNumber: filter.PageNumber,
            pageSize: filter.PageSize,
            orderBy: orderByExpr,
            isDescending: filter.IsDescending ?? true
        );



        var transactionDtos = (pagedTransactions.Items ?? Enumerable.Empty<Transaction>()).Select(t => new TransactionDto
        {
            Id = t.Id,
            Amount = t.Amount,
            CreatedAt = t.CreatedAt,
            FromId = t.FromId,
            ToId = t.ToId,
            Status = t.Status
        }).ToList();

        return new PagedResult<TransactionDto>
        {
            Page = pagedTransactions.Page,
            PageSize = pagedTransactions.PageSize,
            TotalCount = pagedTransactions.TotalCount,
            Items = transactionDtos
        };

    }

    public async Task RequestDepositAsync(Guid userId, Guid walletId, decimal amount)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId);


        if (wallet == null) throw new NotFoundException("Wallet not found.");

        if (wallet.UserId != userId) throw new ForbiddenAccessException("Not your wallet");

        if (amount <= 0 | amount > 1000000) throw new ArgumentOutOfRangeException(nameof(amount), "Invalid Amount");

        var newTransaction = new Transaction
        {
            FromId = SystemConstants.SystemWalletId,
            ToId = walletId,
            Amount = amount,
            Status = TransactionStatus.Pending,
            Type = TransactionType.Deposit
        };
        await _transactionRepository.AddAsync(newTransaction);
        await _unitOfWork.SaveChangesAsync();
        await _paymentGatewayService.InitiatePayment(newTransaction.Id);
    }
    public async Task RequestWithdrawalAsync(Guid userId, Guid walletId, decimal amount)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var wallet = await _walletRepository.GetByIdForWriteAsync(walletId);

            if (wallet == null) throw new NotFoundException("Wallet not found.");

            if (wallet.UserId != userId) throw new ForbiddenAccessException("Not your wallet");

            if (amount <= 0 | amount > 1000000) throw new ArgumentOutOfRangeException(nameof(amount), "Invalid Amount");

            if (wallet.AvailableBalance < amount) throw new InsufficientBalanceException("Insufficient Balance");
            wallet.AvailableBalance -= amount;

            var newTransaction = new Transaction
            {
                FromId = SystemConstants.SystemWalletId,
                ToId = walletId,
                Amount = amount,
                Status = TransactionStatus.Pending,
                Type = TransactionType.Withdrawal
            };
            await _transactionRepository.AddAsync(newTransaction);
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();

            await _paymentGatewayService.InitiatePayment(newTransaction.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task FinalizePaymentAsync(Guid transactionId, bool isSuccessful)
    {
        await using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            if (transaction is null) throw new NotFoundException("Transaction not found.");
            if (transaction.Status != TransactionStatus.Pending) throw new InvalidOperationException("Transaction is not in a pending state.");

            var wallet = await _walletRepository.GetByIdForWriteAsync(transaction.ToId);
            if (wallet is null)
            {
                transaction.Status = TransactionStatus.Failed;
                await _unitOfWork.SaveChangesAsync();
                throw new NotFoundException("CRITICAL: Wallet associated with the transaction was not found.");
            }

            if (isSuccessful)
            {


                if (transaction.Type == TransactionType.Deposit)
                {
                    wallet.Balance += transaction.Amount;
                    transaction.Status = TransactionStatus.Completed;
                }
                else if (transaction.Type == TransactionType.Withdrawal)
                {
                    wallet.Balance -= transaction.Amount;
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
                    wallet.AvailableBalance += transaction.Amount;
                }
            }
            await _unitOfWork.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
}