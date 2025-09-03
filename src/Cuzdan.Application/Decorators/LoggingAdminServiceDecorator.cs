using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cuzdan.Application.Decorators;

public partial class LoggingAdminServiceDecorator(
    IAdminService innerAdminService,
    ILogger<LoggingAdminServiceDecorator> logger)
    : LoggingDecoratorBase<IAdminService>(innerAdminService, logger), IAdminService
{

    public async Task<PagedResult<UserDto>> GetAllUsersProfileAsync(UserFilterDto filter)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetAllUsersProfileAsync(filter));

        LogGetAllUsersProfileAsyncSuccess(Logger, nameof(GetAllUsersProfileAsync));
        return result;
    }

    public async Task<UserDto> GetUserProfileAsync(Guid userId)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetUserProfileAsync(userId));

        LogGetUserProfileAsyncSuccess(Logger, nameof(GetUserProfileAsync), userId);
        return result;
    }

    public async Task<PagedResult<WalletDto>> GetAllWalletsAsync(WalletFilterDto filter)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetAllWalletsAsync(filter));

        LogGetAllWalletsAsyncSuccess(Logger, nameof(GetAllWalletsAsync));
        return result;
    }

    public async Task<List<WalletDto>> GetUserWalletsAsync(Guid userId)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetUserWalletsAsync(userId));

        LogGetUserWalletsAsyncSuccess(Logger, nameof(GetUserWalletsAsync), userId);
        return result;
    }

    public async Task<PagedResult<TransactionDto>> GetAllTransactionsAsync(TransactionFilterDto filter)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetAllTransactionsAsync(filter));

        LogGetAllTransactionsAsyncSuccess(Logger, nameof(GetAllTransactionsAsync));
        return result;
    }
    public async Task<PagedResult<TransactionDto>> GetTransactionsByWalletAsync(Guid walletId, TransactionFilterDto filter)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetTransactionsByWalletAsync(walletId, filter));

        LogGetTransactionsByWalletAsyncSuccess(Logger, nameof(GetTransactionsByWalletAsync), walletId);
        return result;
    }
    public async Task<TransactionDto> GetTransactionAsync(Guid transactionId)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetTransactionAsync(transactionId));

        LogGetTransactionAsyncSuccess(Logger, nameof(GetTransactionAsync), transactionId);
        return result;
    }

    private static class LoggingEvents
    {
        public const int LogGetAllUsersProfileAsyncSuccess = 2014;
        public const int LogGetUserProfileAsyncSuccess = 2015;
        public const int LogGetAllWalletsAsyncSuccess = 2016;
        public const int LogGetUserWalletsAsyncSuccess = 2017;
        public const int LogGetAllTransactionsAsyncSuccess = 2018;
        public const int LogGetTransactionsByWalletAsyncSuccess = 2019;
        public const int LogGetTransactionAsyncSuccess = 2020;
    }

    [LoggerMessage(EventId = LoggingEvents.LogGetAllUsersProfileAsyncSuccess, Level = LogLevel.Information, Message = "Successfully fetched all user prifiles in {MethodName} with admin")]
    private static partial void LogGetAllUsersProfileAsyncSuccess(ILogger Logger, string methodName);

    [LoggerMessage(EventId = LoggingEvents.LogGetUserProfileAsyncSuccess, Level = LogLevel.Information, Message = "Successfully fetched User {UserId} profile in {MethodName} with admin")]
    private static partial void LogGetUserProfileAsyncSuccess(ILogger Logger, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogGetAllWalletsAsyncSuccess, Level = LogLevel.Information, Message = "Successfully fetched all wallets in {MethodName} with admin")]
    private static partial void LogGetAllWalletsAsyncSuccess(ILogger Logger, string methodName);

    [LoggerMessage(EventId = LoggingEvents.LogGetUserWalletsAsyncSuccess, Level = LogLevel.Information, Message = "Successfully fetched wallets for User {UserId} in {MethodName} with admin")]
    private static partial void LogGetUserWalletsAsyncSuccess(ILogger Logger, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogGetAllTransactionsAsyncSuccess, Level = LogLevel.Information, Message = "Successfully fetched all transactions in {MethodName} with admin")]
    private static partial void LogGetAllTransactionsAsyncSuccess(ILogger Logger, string methodName);
    
    [LoggerMessage(EventId = LoggingEvents.LogGetTransactionsByWalletAsyncSuccess, Level = LogLevel.Information, Message = "Successfully fetched all transactions for Wallet {WalletId} in {MethodName} with admin")]
    private static partial void LogGetTransactionsByWalletAsyncSuccess(ILogger Logger, string methodName, Guid walletId);

    [LoggerMessage(EventId = LoggingEvents.LogGetTransactionAsyncSuccess, Level = LogLevel.Information, Message = "Successfully fetched Transaction {TransactionId} in {MethodName} with admin")]
    private static partial void LogGetTransactionAsyncSuccess(ILogger Logger, string methodName, Guid transactionId);
}