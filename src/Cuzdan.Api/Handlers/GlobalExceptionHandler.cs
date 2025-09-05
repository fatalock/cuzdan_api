using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Application.Exceptions;
using FluentValidation;

namespace Cuzdan.Api.Handlers;

public partial class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            NotFoundException _ => HttpStatusCode.NotFound,                 // 404
            InsufficientBalanceException _ => HttpStatusCode.BadRequest,    // 400
            ConflictException _ => HttpStatusCode.Conflict,                 // 409
            UnauthorizedAccessException _ => HttpStatusCode.Unauthorized,   // 401
            ForbiddenAccessException _ => HttpStatusCode.Forbidden,         // 403
            ArgumentOutOfRangeException => HttpStatusCode.BadRequest,       // 400
            ArgumentNullException => HttpStatusCode.BadRequest,             // 400
            ArgumentException => HttpStatusCode.BadRequest,                 // 400
            InvalidTokenException _ => HttpStatusCode.Unauthorized,
            TokenExpiredException _ => HttpStatusCode.Unauthorized,
            RevokedTokenException _ => HttpStatusCode.Unauthorized,
            ValidationException _ => HttpStatusCode.BadRequest,

            _ => HttpStatusCode.InternalServerError                         // 500
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            LogUnhandledException(logger, exception, httpContext.Request.Path);
        }
        else
        {
            LogClientErrorWarning(logger, exception.Message, exception);
        }

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = "An error occurred.",
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.StatusCode = (int)statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "An unhandled exception occurred. Path: {Path}")]
    private static partial void LogUnhandledException(ILogger logger, Exception ex, string path);
    
    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "A client-side error occurred: {ErrorMessage}")]
    private static partial void LogClientErrorWarning(ILogger logger, string errorMessage, Exception ex);
}