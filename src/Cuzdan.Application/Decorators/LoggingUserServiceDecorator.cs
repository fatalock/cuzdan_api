using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cuzdan.Application.Decorators;

public partial class LoggingUserServiceDecorator(
    IUserService innerUserService,
    ILogger<LoggingUserServiceDecorator> logger) : LoggingDecoratorBase<IUserService>(innerUserService, logger), IUserService
{

    public async Task<Result<UserDto>> GetUserProfileAsync(Guid userId)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.GetUserProfileAsync(userId),
            userId);

        if (result.IsSuccess)
        {
            LogGetUserProfileAsyncSuccess(logger, nameof(GetUserProfileAsync), userId);
        }
        else
        {
            LogGetUserProfileAsyncFailure(Logger, nameof(GetUserProfileAsync), result.Error!.Description, userId);
        }
        return result;
    }

    public async Task<Result<UserDto>> UpdateUserProfileAsync(Guid userId, UpdateUserDto updateUserDto)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.UpdateUserProfileAsync(userId, updateUserDto),
            userId);

        if (result.IsSuccess)
        {
            LogUpdateUserProfileAsyncSuccess(logger, nameof(UpdateUserProfileAsync), userId);
        }
        else
        {
            LogUpdateUserProfileAsyncFailure(Logger, nameof(UpdateUserProfileAsync), result.Error!.Description, userId);
        }
        return result;
    }


    private static class LoggingEvents
    {
        public const int LogGetUserProfileAsyncSuccess = 2004;
        public const int LogUpdateUserProfileAsyncSuccess = 2005;

        public const int LogGetUserProfileAsyncFailure = 4004;
        public const int LogUpdateUserProfileAsyncFailure = 4005;
    }

    [LoggerMessage(EventId = LoggingEvents.LogGetUserProfileAsyncSuccess, Level = LogLevel.Information, Message = "Successfully fetched user profile in {MethodName} for User {UserId}")]
    private static partial void LogGetUserProfileAsyncSuccess(ILogger logger, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogUpdateUserProfileAsyncSuccess, Level = LogLevel.Information, Message = "Successfully updated user profile in {MethodName} for User {UserId}")]
    private static partial void LogUpdateUserProfileAsyncSuccess(ILogger logger, string methodName, Guid userId);


    [LoggerMessage(EventId = LoggingEvents.LogGetUserProfileAsyncFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for User {userId}. Reason: {ErrorDescription}")]
    private static partial void LogGetUserProfileAsyncFailure(ILogger Logger, string methodName, string errorDescription, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogUpdateUserProfileAsyncFailure, Level = LogLevel.Information, Message = "Failed {MethodName} for User {userId}. Reason: {ErrorDescription}")]
    private static partial void LogUpdateUserProfileAsyncFailure(ILogger Logger, string methodName, string errorDescription, Guid userId);
    

}