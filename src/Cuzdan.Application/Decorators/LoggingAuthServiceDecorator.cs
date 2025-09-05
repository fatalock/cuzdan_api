using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cuzdan.Application.Decorators;

public partial class LoggingAuthServiceDecorator(
    IAuthService innerAuthService,
    ILogger<LoggingAuthServiceDecorator> logger)
    : LoggingDecoratorBase<IAuthService>(innerAuthService, logger), IAuthService
{

    public async Task<Result<UserDto>> RegisterAsync(RegisterUserDto registerDto)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.RegisterAsync(registerDto));

        if (result.IsSuccess)
        {
            LogRegisterAsyncSuccess(Logger, nameof(RegisterAsync), result.Value!.Id);
        }
        else
        {
            LogRegisterAsyncFailure(Logger, nameof(RegisterAsync), result.Error!.Description, registerDto.Email);
        }
        return result;
    }

    public async Task<Result<AuthResult>> LoginAsync(LoginUserDto loginDto)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.LoginAsync(loginDto));

        if (result.IsSuccess)
        {
            LogLoginAsyncSuccess(Logger, nameof(LoginAsync), result.Value!.UserId);
        }
        else
        {
            LogLoginAsyncFailure(Logger, nameof(RegisterAsync), result.Error!.Description, loginDto.Email);
        }
        return result;
    }

    public async Task<Result<AuthResult>> RefresAccesshAsync(string refreshToken)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.RefresAccesshAsync(refreshToken));

        if (result.IsSuccess)
        {
            LogRefresAccesshAsyncSuccess(Logger, nameof(RefresAccesshAsync), result.Value!.UserId);
        }
        else
        {
            LogRefresAccesshAsyncFailure(Logger, nameof(RegisterAsync), result.Error!.Description, refreshToken);
        }
        
        return result;
    }


    private static class LoggingEvents
    {
        public const int LogRegisterAsyncSuccess = 2011;
        public const int LogLoginAsyncSuccess = 2012;
        public const int LogRefresAccesshAsyncSuccess = 2013;

        public const int LogRegisterAsyncFailure = 4011;
        public const int LogLoginAsyncFailure = 4012;
        public const int LogRefresAccesshAsyncFailure = 4013;
    }

    [LoggerMessage(EventId = LoggingEvents.LogRegisterAsyncSuccess, Level = LogLevel.Information, Message = "Successfully registered in {MethodName} for User {UserId}")]
    private static partial void LogRegisterAsyncSuccess(ILogger Logger, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogLoginAsyncSuccess, Level = LogLevel.Information, Message = "Successfully login in {MethodName} for User {UserId}")]
    private static partial void LogLoginAsyncSuccess(ILogger Logger, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogRefresAccesshAsyncSuccess, Level = LogLevel.Information, Message = "Successfully refreshed access token in {MethodName} for User {UserId}")]
    private static partial void LogRefresAccesshAsyncSuccess(ILogger Logger, string methodName, Guid userId);


    [LoggerMessage(EventId = LoggingEvents.LogRegisterAsyncFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for Email {Email}. Reason: {ErrorDescription}")]
    private static partial void LogRegisterAsyncFailure(ILogger Logger, string methodName, string errorDescription, string email);

    [LoggerMessage(EventId = LoggingEvents.LogLoginAsyncFailure, Level = LogLevel.Warning, Message = "Failed {MethodName} for Email {Email}. Reason: {ErrorDescription}")]
    private static partial void LogLoginAsyncFailure(ILogger Logger, string methodName, string errorDescription, string email);

    [LoggerMessage(EventId = LoggingEvents.LogRefresAccesshAsyncFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for Refresh Token {refreshToken}. Reason: {ErrorDescription}")]
    private static partial void LogRefresAccesshAsyncFailure(ILogger Logger, string methodName, string errorDescription, string refreshToken);
}