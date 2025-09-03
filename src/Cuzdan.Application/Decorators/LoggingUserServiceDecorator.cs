using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cuzdan.Application.Decorators;

public partial class LoggingUserServiceDecorator(
    IUserService innerUserService,
    ILogger<LoggingUserServiceDecorator> logger) : LoggingDecoratorBase<IUserService>(innerUserService, logger), IUserService
{

    public async Task<UserDto> GetUserProfileAsync(Guid userId)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.GetUserProfileAsync(userId),
            userId);

        LogGetUserProfileAsyncSuccess(logger, nameof(GetUserProfileAsync), userId);
        return result;
    }

    public async Task<UserDto> UpdateUserProfileAsync(Guid userId, UpdateUserDto updateUserDto)
    {
        var result = await LogAndExecuteAsync(
            () => InnerService.UpdateUserProfileAsync(userId, updateUserDto),
            userId);

        LogUpdateUserProfileAsyncSuccess(logger, nameof(UpdateUserProfileAsync), userId);
        return result;
    }


    private static class LoggingEvents
    {
        public const int LogGetUserProfileAsyncSuccess = 2004;
        public const int LogUpdateUserProfileAsyncSuccess = 2005;
    }

    [LoggerMessage(EventId = LoggingEvents.LogGetUserProfileAsyncSuccess, Level = LogLevel.Information, Message = "Successfully fetched user profile in {MethodName} for User {UserId}")]
    private static partial void LogGetUserProfileAsyncSuccess(ILogger logger, string methodName, Guid userId);

    [LoggerMessage(EventId = LoggingEvents.LogUpdateUserProfileAsyncSuccess, Level = LogLevel.Information, Message = "Successfully updated user profile in {MethodName} for User {UserId}")]
    private static partial void LogUpdateUserProfileAsyncSuccess(ILogger logger, string methodName, Guid userId);

}