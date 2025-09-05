using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cuzdan.Application.Decorators;

public abstract partial class LoggingDecoratorBase<TService>(TService innerService, ILogger logger)
{
    protected TService InnerService { get; } = innerService;
    protected ILogger Logger { get; } = logger;
    protected async Task<T> LogAndExecuteAsync<T>(Func<Task<T>> action, Guid userId, [CallerMemberName] string methodName = "")
    {
        LogMethodEntry(Logger, methodName, userId);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            stopwatch.Stop();
            LogMethodExit(Logger, methodName, stopwatch.ElapsedMilliseconds);
        }
    }

    protected async Task LogAndExecuteAsync(Func<Task> action, Guid userId, [CallerMemberName] string methodName = "")
    {
        LogMethodEntry(Logger, methodName, userId);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await action();
        }
        catch (Exception ex)
        {

            throw;
        }
        finally
        {
            stopwatch.Stop();
            LogMethodExit(Logger, methodName, stopwatch.ElapsedMilliseconds);
        }
    }

    protected async Task<T> LogAndExecuteAsync<T>(Func<Task<T>> action, [CallerMemberName] string methodName = "")
    {
        LogMethodEntryWithoutUser(Logger, methodName);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            return await action();
        }
        catch (Exception ex)
        {

            throw;
        }
        finally
        {
            stopwatch.Stop();
            LogMethodExit(Logger, methodName, stopwatch.ElapsedMilliseconds);
        }
    }

    protected async Task LogAndExecuteAsync(Func<Task> action, [CallerMemberName] string methodName = "")
    {
        LogMethodEntryWithoutUser(Logger, methodName);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await action();
        }
        catch (Exception ex)
        {

            throw;
        }
        finally
        {
            stopwatch.Stop();
            LogMethodExit(Logger, methodName, stopwatch.ElapsedMilliseconds);
        }
    }

    protected async Task<T> LogAndExecuteAdminAsync<T>(Func<Task<T>> action, [CallerMemberName] string methodName = "")
    {
        LogMethodEntryAdminUser(Logger, methodName);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            return await action();
        }
        catch (Exception ex)
        {

            throw;
        }
        finally
        {
            stopwatch.Stop();
            LogMethodExit(Logger, methodName, stopwatch.ElapsedMilliseconds);
        }
    }

    protected async Task LogAndExecuteAdminAsync(Func<Task> action, [CallerMemberName] string methodName = "")
    {
        LogMethodEntryAdminUser(Logger, methodName);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await action();
        }
        catch (Exception ex)
        {

            throw;
        }
        finally
        {
            stopwatch.Stop();
            LogMethodExit(Logger, methodName, stopwatch.ElapsedMilliseconds);
        }
    }

    [LoggerMessage(EventId = 3001, Level = LogLevel.Information, Message = "Entering {MethodName} for User {UserId}")]
    private static partial void LogMethodEntry(ILogger Logger, string methodName, Guid userId);

    [LoggerMessage(EventId = 3002, Level = LogLevel.Information, Message = "Finished {MethodName} in {ElapsedMilliseconds}ms")]
    private static partial void LogMethodExit(ILogger Logger, string methodName, long elapsedMilliseconds);

    [LoggerMessage(EventId = 3003, Level = LogLevel.Information, Message = "Entering {MethodName}")]
    private static partial void LogMethodEntryWithoutUser(ILogger logger, string methodName);

    [LoggerMessage(EventId = 3004, Level = LogLevel.Information, Message = "Entering {MethodName} with admin")]
    private static partial void LogMethodEntryAdminUser(ILogger logger, string methodName);


}