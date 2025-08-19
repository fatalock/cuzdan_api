using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Cuzdan.Api.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
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

            _ => HttpStatusCode.InternalServerError                         // 500
        };

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
}