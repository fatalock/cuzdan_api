using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Cuzdan.Api.Filters;
public partial class AsyncValidationFilter : IAsyncActionFilter
{
    private readonly ILogger<AsyncValidationFilter> _logger;

    public AsyncValidationFilter(ILogger<AsyncValidationFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var validationErrors = context.ModelState
                .Where(e => e.Value is { Errors.Count: > 0 })
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            
            LogValidationError(_logger, "Request validation failed", validationErrors);
            
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            };
            context.Result = new BadRequestObjectResult(problemDetails);

            return;
        }

        await next();
    }

    [LoggerMessage(
        EventId = 5005,
        Level = LogLevel.Warning,
        Message = "{ValidationSummary} : {ValidationErrors}")]
    private static partial void LogValidationError(ILogger logger, string validationSummary, object validationErrors);
}