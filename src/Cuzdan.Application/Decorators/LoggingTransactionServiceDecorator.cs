using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cuzdan.Application.Decorators;

public partial class LoggingTransactionServiceDecorator(
    ITransactionService innerTransactionService,
    ILogger<LoggingTransactionServiceDecorator> logger)
    : LoggingDecoratorBase<ITransactionService>(innerTransactionService, logger), ITransactionService
{

    public async Task TransferTransactionAsync(CreateTransactionDto transactionDto, Guid userId)
    {
        await LogAndExecuteAsync(
            () => InnerService.TransferTransactionAsync(transactionDto, userId),
            userId);

        LogTransferTransactionAsyncSuccess(Logger, transactionDto.Amount, transactionDto.FromId, transactionDto.ToId, nameof(TransferTransactionAsync), userId);
        return;
    }

    public async Task<PagedResult<TransactionDto>> GetTransactionsByWalletAsync(
        Guid userId,
        Guid walletId,
        TransactionFilterDto filter
    )
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.GetTransactionsByWalletAsync(userId, walletId, filter),
            userId);

        LogGetTransactionsByWalletAsyncSuccess(Logger, walletId, nameof(GetTransactionsByWalletAsync), userId);
        return result;
    }

    public async Task RequestDepositAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto depositRequestDto)
    {
        await LogAndExecuteAsync(
            () => InnerService.RequestDepositAsync(userId, walletId, depositRequestDto),
            userId);

        LogRequestDepositAsyncSuccess(Logger, walletId, depositRequestDto.Amount, nameof(RequestDepositAsync), userId);
        return;
    }

    public async Task RequestWithdrawalAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto withdrawalRequestDto)
    {
        await LogAndExecuteAsync(
           () => InnerService.RequestWithdrawalAsync(userId, walletId, withdrawalRequestDto),
           userId);

        LogRequestWithdrawalAsyncSuccess(Logger, walletId, withdrawalRequestDto.Amount, nameof(RequestWithdrawalAsync), userId);
        return;
    }

    public async Task FinalizePaymentAsync(Guid transactionId, bool isSuccessful)
    {
        await LogAndExecuteAsync(
            () => InnerService.FinalizePaymentAsync(transactionId, isSuccessful));

        LogFinalizePaymentAsyncSuccess(Logger,transactionId ,nameof(FinalizePaymentAsync));
        return;
    }


    private static class LoggingEvents
    {
        public const int LogTransferTransactionAsyncSuccess = 2006;
        public const int LogGetTransactionsByWalletAsyncSuccess = 2007;
        public const int LogRequestDepositAsyncSuccess = 2008;
        public const int LogRequestWithdrawalAsyncSuccess = 2009;
        public const int LogFinalizePaymentAsyncSuccess = 2010;
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

}