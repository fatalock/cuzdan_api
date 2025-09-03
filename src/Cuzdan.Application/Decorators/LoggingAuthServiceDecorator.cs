using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cuzdan.Application.Decorators;

public partial class LoggingAuthServiceDecorator(
    IAuthService innerAuthService,
    ILogger<LoggingAuthServiceDecorator> logger)
    : LoggingDecoratorBase<IAuthService>(innerAuthService, logger), IAuthService
{

    public async Task<UserDto> RegisterAsync(RegisterUserDto registerDto)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.RegisterAsync(registerDto));

        LogRegisterAsyncSuccess(Logger, nameof(RegisterAsync), result.Id);
        return result;
    }

    public async Task<AuthResult> LoginAsync(LoginUserDto loginDto)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.LoginAsync(loginDto));

        LogLoginAsyncSuccess(Logger, nameof(LoginAsync), result.UserId);
        return result;
    }

    public async Task<AuthResult> RefresAccesshAsync(string refreshToken)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.RefresAccesshAsync(refreshToken));

        LogRefresAccesshAsyncSuccess(Logger, nameof(RefresAccesshAsync), result.UserId);
        return result;
    }


    private static class LoggingEvents
    {
        public const int LogRegisterAsyncSuccess = 2011;
        public const int LogLoginAsyncSuccess = 2012;
        public const int LogRefresAccesshAsyncSuccess = 2013;
    }

    [LoggerMessage(EventId = LoggingEvents.LogRegisterAsyncSuccess, Level = LogLevel.Information, Message = "Successfully registered in {MethodName} for User {UserId}")]
    private static partial void LogRegisterAsyncSuccess(ILogger Logger, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogLoginAsyncSuccess, Level = LogLevel.Information, Message = "Successfully login in {MethodName} for User {UserId}")]
    private static partial void LogLoginAsyncSuccess(ILogger Logger, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogRefresAccesshAsyncSuccess, Level = LogLevel.Information, Message = "Successfully refreshed access token in {MethodName} for User {UserId}")]
    private static partial void LogRefresAccesshAsyncSuccess(ILogger Logger, string methodName, Guid userId);
}