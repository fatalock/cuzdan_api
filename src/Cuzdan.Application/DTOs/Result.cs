#pragma warning disable CA1000
using Cuzdan.Domain.Errors;

namespace Cuzdan.Application.DTOs;
public abstract class ResultBase(bool isSuccess, DomainError? error)
{
    public bool IsSuccess { get; } = isSuccess;
    public DomainError? Error { get; } = error;
}

public class Result : ResultBase
{
    private Result(bool isSuccess, DomainError? error) : base(isSuccess, error) { }

    public static Result Success() => new Result(true, null);
    public static Result Failure(DomainError error) => new Result(false, error);
}

public class Result<T> : ResultBase
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, DomainError? error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new Result<T>(true, value, null);
    public static Result<T> Failure(DomainError error) => new Result<T>(false, default, error);
}