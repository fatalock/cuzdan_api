using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cuzdan.Application.Decorators;

public partial class LoggingTransactionServiceDecorator(
    ITransactionService innerTransactionService,
    ILogger<LoggingTransactionServiceDecorator> logger)
    : LoggingDecoratorBase<ITransactionService>(innerTransactionService, logger), ITransactionService
{

    public async Task<Result> TransferTransactionAsync(CreateTransactionDto transactionDto, Guid userId)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.TransferTransactionAsync(transactionDto, userId),
            userId);

        if (result.IsSuccess)
        {
            LogTransferTransactionAsyncSuccess(Logger, transactionDto.Amount, transactionDto.FromId, transactionDto.ToId, nameof(TransferTransactionAsync), userId);
        }
        else
        {
            LogTransferTransactionAsyncFailure(Logger, nameof(TransferTransactionAsync), result.Error!.Description, userId);
        }
        return result;
    }

    public async Task<Result<PagedResult<TransactionDto>>> GetTransactionsByWalletAsync(
        Guid userId,
        Guid walletId,
        TransactionFilterDto filter
    )
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.GetTransactionsByWalletAsync(userId, walletId, filter),
            userId);

        if (result.IsSuccess)
        {
            LogGetTransactionsByWalletAsyncSuccess(Logger, walletId, nameof(GetTransactionsByWalletAsync), userId);
        }
        else
        {
            LogGetTransactionsByWalletAsyncFailure(Logger, nameof(GetTransactionsByWalletAsync), result.Error!.Description, userId, walletId);
        }
        return result;
    }

    public async Task<Result> RequestDepositAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto depositRequestDto)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.RequestDepositAsync(userId, walletId, depositRequestDto),
            userId);

        if (result.IsSuccess)
        {
            LogRequestDepositAsyncSuccess(Logger, walletId, depositRequestDto.Amount, nameof(RequestDepositAsync), userId);
        }
        else
        {
            LogRequestDepositAsyncFailure(Logger, nameof(RequestDepositAsync), result.Error!.Description, userId, walletId, depositRequestDto.Amount);
        }
        return result;
    }

    public async Task<Result> RequestWithdrawalAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto withdrawalRequestDto)
    {
        var result = await LogAndExecuteAsync(
           () => InnerService.RequestWithdrawalAsync(userId, walletId, withdrawalRequestDto),
           userId);

        if (result.IsSuccess)
        {
            LogRequestWithdrawalAsyncSuccess(Logger, walletId, withdrawalRequestDto.Amount, nameof(RequestWithdrawalAsync), userId);
        }
        else
        {
            LogRequestWithdrawalAsyncFailure(Logger, nameof(RequestWithdrawalAsync), result.Error!.Description, userId, walletId, withdrawalRequestDto.Amount);
        }
        return result;
    }

    public async Task<Result> FinalizePaymentAsync(Guid transactionId, bool isSuccessful)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.FinalizePaymentAsync(transactionId, isSuccessful));

        if (result.IsSuccess)
        {
            LogFinalizePaymentAsyncSuccess(Logger, transactionId, nameof(FinalizePaymentAsync));
        }
        else
        {
            LogFinalizePaymentAsyncFailure(Logger, nameof(FinalizePaymentAsync), result.Error!.Description, transactionId);
        }
        return result;
    }


    private static class LoggingEvents
    {
        public const int LogTransferTransactionAsyncSuccess = 2006;
        public const int LogGetTransactionsByWalletAsyncSuccess = 2007;
        public const int LogRequestDepositAsyncSuccess = 2008;
        public const int LogRequestWithdrawalAsyncSuccess = 2009;
        public const int LogFinalizePaymentAsyncSuccess = 2010;

        public const int LogTransferTransactionAsyncFailure = 4006;
        public const int LogGetTransactionsByWalletAsyncFailure = 4007;
        public const int LogRequestDepositAsyncFailure = 4008;
        public const int LogRequestWithdrawalAsyncFailure = 4009;
        public const int LogFinalizePaymentAsyncFailure = 4010;
    }

    [LoggerMessage(EventId = LoggingEvents.LogTransferTransactionAsyncSuccess, Level = LogLevel.Information, Message = "Successfully transfered amount {Amount} from wallet {FromId} to wallet {ToId} in {MethodName} for User {UserId}")]
    private static partial void LogTransferTransactionAsyncSuccess(ILogger Logger, decimal amount, Guid fromId, Guid toId, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogGetTransactionsByWalletAsyncSuccess, Level = LogLevel.Information, Message = "Successfully fetched transactions by wallet {WalletId} in {MethodName} for User {UserId}")]
    private static partial void LogGetTransactionsByWalletAsyncSuccess(ILogger Logger, Guid walletId, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogRequestDepositAsyncSuccess, Level = LogLevel.Information, Message = "Successfully requested deposit amount {Amount} to wallet {WalletId} in {MethodName} for User {UserId}")]
    private static partial void LogRequestDepositAsyncSuccess(ILogger Logger, Guid walletId, decimal amount, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogRequestWithdrawalAsyncSuccess, Level = LogLevel.Information, Message = "Successfully requested withdrawal amount {Amount} from wallet {WalletId} in {MethodName} for User {UserId}")]
    private static partial void LogRequestWithdrawalAsyncSuccess(ILogger Logger, Guid walletId, decimal amount, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogFinalizePaymentAsyncSuccess, Level = LogLevel.Information, Message = "Successfully finalized transaction {TransactionId} in {MethodName}")]
    private static partial void LogFinalizePaymentAsyncSuccess(ILogger Logger, Guid transactionId, string methodName);

    
    [LoggerMessage(EventId = LoggingEvents.LogTransferTransactionAsyncFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for User {userId}. Reason: {ErrorDescription}")]
    private static partial void LogTransferTransactionAsyncFailure(ILogger Logger, string methodName, string errorDescription, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogGetTransactionsByWalletAsyncFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for User {userId} and Wallet {WalletId}. Reason: {ErrorDescription}")]
    private static partial void LogGetTransactionsByWalletAsyncFailure(ILogger Logger, string methodName, string errorDescription, Guid userId, Guid walletId);

    [LoggerMessage(EventId = LoggingEvents.LogRequestDepositAsyncFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for User {userId} and Wallet {WalletId} and Amount {amount}. Reason: {ErrorDescription}")]
    private static partial void LogRequestDepositAsyncFailure(ILogger Logger, string methodName, string errorDescription, Guid userId, Guid walletId, decimal amount);

    [LoggerMessage(EventId = LoggingEvents.LogRequestWithdrawalAsyncFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for User {userId} and Wallet {WalletId} and Amount {amount}. Reason: {ErrorDescription}")]
    private static partial void LogRequestWithdrawalAsyncFailure(ILogger Logger, string methodName, string errorDescription, Guid userId, Guid walletId, decimal amount);

    [LoggerMessage(EventId = LoggingEvents.LogFinalizePaymentAsyncFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for Transaction {transactionId}. Reason: {ErrorDescription}")]
    private static partial void LogFinalizePaymentAsyncFailure(ILogger Logger, string methodName, string errorDescription, Guid transactionId);

}