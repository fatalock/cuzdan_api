namespace Cuzdan.Domain.Errors;

public record DomainError(string Code, string Description, ErrorType Type);

public enum ErrorType
{
    NotFound,
    Conflict,
    Unauthorized,
    Failure,
    InsufficientBalance,
    Forbidden,
}