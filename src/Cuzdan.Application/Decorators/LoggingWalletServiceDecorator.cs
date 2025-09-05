namespace Cuzdan.Application.Decorators;

using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.Extensions.Logging;


public partial class LoggingWalletServiceDecorator(
    IWalletService innerWalletService,
    ILogger<LoggingWalletServiceDecorator> logger) : LoggingDecoratorBase<IWalletService>(innerWalletService, logger), IWalletService
{

    public async Task<Result<WalletDto>> CreateWalletAsync(CreateWalletDto createWalletDto, Guid userId)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.CreateWalletAsync(createWalletDto, userId),
            userId);

        if (result.IsSuccess)
        {
            LogCreateWalletAsyncSuccess(logger, result.Value!.Id, nameof(CreateWalletAsync), userId);
        }
        else
        {
            WalletCreatedFailure(Logger, nameof(CreateWalletAsync), result.Error!.Description, userId);
        }
        return result;
    }

    public async Task<Result<List<WalletDto>>> GetWalletsAsync(Guid userId)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.GetWalletsAsync(userId),
            userId);

        if (result.IsSuccess)
        {
            LogGetWalletsAsyncSuccess(logger, nameof(GetWalletsAsync), userId);
        }
        else
        {
            WalletFetchSuccessFailure(Logger, nameof(GetWalletsAsync), result.Error!.Description, userId);
        }
        return result;
    }

    public async Task<Result<List<UserBalanceByCurrencyResponseDto>>> GetTotalBalancePerCurrencyAsync(Guid userId)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.GetTotalBalancePerCurrencyAsync(userId),
            userId);

        if (result.IsSuccess)
        {
            LogGetTotalBalancePerCurrencyAsyncSuccess(logger, nameof(GetTotalBalancePerCurrencyAsync), userId);
        }
        else
        {
            TotalBalancePerCurrencyFetchSuccessFailure(Logger, nameof(GetTotalBalancePerCurrencyAsync), result.Error!.Description, userId);
        }
        return result;
    }

    private static class LoggingEvents
    {
        public const int WalletCreated = 2001;
        public const int WalletFetchSuccess = 2002;
        public const int TotalBalancePerCurrencyFetchSuccess = 2003;

        public const int WalletCreatedFailure = 4001;
        public const int WalletFetchSuccessFailure = 4002;
        public const int TotalBalancePerCurrencyFetchSuccessFailure = 4003;
    }

    [LoggerMessage(EventId = LoggingEvents.WalletCreated, Level = LogLevel.Information, Message = "Successfully created wallet {WalletId} in {MethodName} for User {UserId}")]
    private static partial void LogCreateWalletAsyncSuccess(ILogger logger, Guid walletId, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.WalletFetchSuccess, Level = LogLevel.Information, Message = "Successfully fetched data in {MethodName} for User {UserId}")]
    private static partial void LogGetWalletsAsyncSuccess(ILogger logger, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.TotalBalancePerCurrencyFetchSuccess, Level = LogLevel.Information, Message = "Successfully fetched data in {MethodName} for User {UserId}")]
    private static partial void LogGetTotalBalancePerCurrencyAsyncSuccess(ILogger logger, string methodName, Guid userId);


    [LoggerMessage(EventId = LoggingEvents.WalletCreatedFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for User {userId}. Reason: {ErrorDescription}")]
    private static partial void WalletCreatedFailure(ILogger Logger, string methodName, string errorDescription, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.WalletFetchSuccessFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for User {userId}. Reason: {ErrorDescription}")]
    private static partial void WalletFetchSuccessFailure(ILogger Logger, string methodName, string errorDescription, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.TotalBalancePerCurrencyFetchSuccessFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for User {userId}. Reason: {ErrorDescription}")]
    private static partial void TotalBalancePerCurrencyFetchSuccessFailure(ILogger Logger, string methodName, string errorDescription, Guid userId);
    
}