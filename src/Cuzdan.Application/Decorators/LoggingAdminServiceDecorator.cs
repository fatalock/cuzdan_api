using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cuzdan.Application.Decorators;

public partial class LoggingAdminServiceDecorator(
    IAdminService innerAdminService,
    ILogger<LoggingAdminServiceDecorator> logger)
    : LoggingDecoratorBase<IAdminService>(innerAdminService, logger), IAdminService
{

    public async Task<Result<PagedResult<UserDto>>> GetAllUsersProfileAsync(UserFilterDto filter)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetAllUsersProfileAsync(filter));

        if (result.IsSuccess)
        {
            LogGetAllUsersProfileAsyncSuccess(Logger, nameof(GetAllUsersProfileAsync));
        }
        else
        {
            LogGetAllUsersProfileAsyncFailure(Logger, nameof(GetAllUsersProfileAsync), result.Error!.Description);
        }
        return result;
    }

    public async Task<Result<UserDto>> GetUserProfileAsync(Guid userId)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetUserProfileAsync(userId));

        LogGetUserProfileAsyncSuccess(Logger, nameof(GetUserProfileAsync), userId);
        if (result.IsSuccess)
        {
            LogGetAllUsersProfileAsyncSuccess(Logger, nameof(GetAllUsersProfileAsync));
        }
        else
        {
            LogGetAllUsersProfileAsyncFailure(Logger, nameof(GetAllUsersProfileAsync), result.Error!.Description);
        }
        return result;
    }

    public async Task<Result<PagedResult<WalletDto>>> GetAllWalletsAsync(WalletFilterDto filter)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetAllWalletsAsync(filter));

        if (result.IsSuccess)
        {
            LogGetAllWalletsAsyncSuccess(Logger, nameof(GetAllWalletsAsync));
        }
        else
        {
            LogGetAllWalletsAsyncFailure(Logger, nameof(GetAllWalletsAsync), result.Error!.Description);
        }
        
        return result;
    }

    public async Task<Result<List<WalletDto>>> GetUserWalletsAsync(Guid userId)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetUserWalletsAsync(userId));

        if (result.IsSuccess)
        {
            LogGetUserWalletsAsyncSuccess(Logger, nameof(GetUserWalletsAsync), userId);
        }
        else
        {
            LogGetUserWalletsAsyncFailure(Logger, nameof(GetUserWalletsAsync), result.Error!.Description);
        }
        
        return result;
    }

    public async Task<Result<PagedResult<TransactionDto>>> GetAllTransactionsAsync(TransactionFilterDto filter)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetAllTransactionsAsync(filter));

        if (result.IsSuccess)
        {
            LogGetAllTransactionsAsyncSuccess(Logger, nameof(GetAllTransactionsAsync));
        }
        else
        {
            LogGetAllTransactionsAsyncFailure(Logger, nameof(GetAllTransactionsAsync), result.Error!.Description);
        }
        
        return result;
    }
    public async Task<Result<PagedResult<TransactionDto>>> GetTransactionsByWalletAsync(Guid walletId, TransactionFilterDto filter)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetTransactionsByWalletAsync(walletId, filter));


        if (result.IsSuccess)
        {
            LogGetTransactionsByWalletAsyncSuccess(Logger, nameof(GetTransactionsByWalletAsync), walletId);
        }
        else
        {
            LogGetTransactionsByWalletAsyncFailure(Logger, nameof(GetTransactionsByWalletAsync), result.Error!.Description);
        }
        return result;
    }
    public async Task<Result<TransactionDto>> GetTransactionAsync(Guid transactionId)
    {
        var result = await LogAndExecuteAdminAsync(
            () => InnerService.GetTransactionAsync(transactionId));

        if (result.IsSuccess)
        {
            LogGetTransactionAsyncSuccess(Logger, nameof(GetTransactionAsync), transactionId);
        }
        else
        {
            LogGetTransactionAsyncFailure(Logger, nameof(GetTransactionAsync), result.Error!.Description);
        }
        
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
        public const int LogGetAllUsersProfileAsyncFailure = 4014;
        public const int LogGetUserProfileAsyncFailure = 4015;
        public const int LogGetAllWalletsAsyncFailure = 4016;
        public const int LogGetUserWalletsAsyncFailure = 4017;
        public const int LogGetAllTransactionsAsyncFailure = 4018;
        public const int LogGetTransactionsByWalletAsyncFailure = 4019;
        public const int LogGetTransactionAsyncFailure = 4020;
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
    

    [LoggerMessage(EventId = LoggingEvents.LogGetAllUsersProfileAsyncFailure, Level = LogLevel.Warning, Message = "Failed {MethodName} with admin . Reason: {ErrorDescription}")]
    private static partial void LogGetAllUsersProfileAsyncFailure(ILogger Logger, string methodName, string errorDescription);

    [LoggerMessage(EventId = LoggingEvents.LogGetUserProfileAsyncFailure, Level = LogLevel.Warning, Message = "Failed {MethodName} with admin . Reason: {ErrorDescription}")]
    private static partial void LogGetUserProfileAsyncFailure(ILogger Logger, string methodName, string errorDescription);

    [LoggerMessage(EventId = LoggingEvents.LogGetAllWalletsAsyncFailure, Level = LogLevel.Warning, Message = "Failed {MethodName} with admin . Reason: {ErrorDescription}")]
    private static partial void LogGetAllWalletsAsyncFailure(ILogger Logger, string methodName, string errorDescription);

    [LoggerMessage(EventId = LoggingEvents.LogGetUserWalletsAsyncFailure, Level = LogLevel.Warning, Message = "Failed {MethodName} with admin . Reason: {ErrorDescription}")]
    private static partial void LogGetUserWalletsAsyncFailure(ILogger Logger, string methodName, string errorDescription);

    [LoggerMessage(EventId = LoggingEvents.LogGetAllTransactionsAsyncFailure, Level = LogLevel.Warning, Message = "Failed {MethodName} with admin . Reason: {ErrorDescription}")]
    private static partial void LogGetAllTransactionsAsyncFailure(ILogger Logger, string methodName, string errorDescription);

    [LoggerMessage(EventId = LoggingEvents.LogGetTransactionsByWalletAsyncFailure, Level = LogLevel.Warning, Message = "Failed {MethodName} with admin . Reason: {ErrorDescription}")]
    private static partial void LogGetTransactionsByWalletAsyncFailure(ILogger Logger, string methodName, string errorDescription);

    [LoggerMessage(EventId = LoggingEvents.LogGetTransactionAsyncFailure, Level = LogLevel.Warning, Message = "Failed {MethodName} with admin . Reason: {ErrorDescription}")]
    private static partial void LogGetTransactionAsyncFailure(ILogger Logger, string methodName, string errorDescription);

}