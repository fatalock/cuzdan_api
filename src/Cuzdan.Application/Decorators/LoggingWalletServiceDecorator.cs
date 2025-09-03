namespace Cuzdan.Application.Decorators;

using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.Extensions.Logging;


public partial class LoggingWalletServiceDecorator(
    IWalletService innerWalletService,
    ILogger<LoggingWalletServiceDecorator> logger) : LoggingDecoratorBase<IWalletService>(innerWalletService, logger), IWalletService
{

    public async Task<WalletDto> CreateWalletAsync(CreateWalletDto createWalletDto, Guid userId)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.CreateWalletAsync(createWalletDto, userId),
            userId);

        LogCreateWalletAsyncSuccess(logger, result.Id, nameof(CreateWalletAsync), userId);
        return result;
    }

    public async Task<List<WalletDto>> GetWalletsAsync(Guid userId)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.GetWalletsAsync(userId),
            userId);

        LogGetWalletsAsyncSuccess(logger, nameof(GetWalletsAsync), userId);
        return result;
    }

    public async Task<List<UserBalanceByCurrencyResponseDto>> GetTotalBalancePerCurrencyAsync(Guid userId)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.GetTotalBalancePerCurrencyAsync(userId),
            userId);

        LogGetTotalBalancePerCurrencyAsyncSuccess(logger, nameof(GetTotalBalancePerCurrencyAsync), userId);
        return result;
    }

    private static class LoggingEvents
    {
        public const int WalletCreated = 2001;
        public const int WalletFetchSuccess = 2002;
        public const int otalBalancePerCurrencyFetchSuccess = 2003;
    }

    [LoggerMessage(EventId = LoggingEvents.WalletCreated, Level = LogLevel.Information, Message = "Successfully created wallet {WalletId} in {MethodName} for User {UserId}")]
    private static partial void LogCreateWalletAsyncSuccess(ILogger logger, Guid walletId,  string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.WalletFetchSuccess, Level = LogLevel.Information, Message = "Successfully fetched data in {MethodName} for User {UserId}")]
    private static partial void LogGetWalletsAsyncSuccess(ILogger logger, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.otalBalancePerCurrencyFetchSuccess, Level = LogLevel.Information, Message = "Successfully fetched data in {MethodName} for User {UserId}")]
    private static partial void LogGetTotalBalancePerCurrencyAsyncSuccess(ILogger logger, string methodName, Guid userId);
}