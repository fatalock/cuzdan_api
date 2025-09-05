namespace Cuzdan.Api.Controllers;

using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Enums;
using Cuzdan.Domain.Errors;
using Microsoft.AspNetCore.Mvc;


public abstract partial class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (result.Value is bool boolValue && !boolValue)
            {
                return BadRequest(new ProblemDetails { Title = "Operation failed." });
            }
            return Ok(result.Value);
        }

        var statusCode = result.Error!.Type switch
        {
            ErrorType.InsufficientBalance => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = result.Error.Code,
            Detail = result.Error.Description
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent(); 
        }

        var statusCode = result.Error!.Type switch
        {
            ErrorType.InsufficientBalance => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = result.Error.Code,
            Detail = result.Error.Description
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
}