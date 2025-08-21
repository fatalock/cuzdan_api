namespace Cuzdan.Application.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}

public class InsufficientBalanceException(string message) : Exception(message)
{
}

public class ConflictException(string message) : Exception(message)
{
}

public class ForbiddenAccessException(string message) : Exception(message)
{
}

public class InvalidTokenException(string message) : Exception(message)
{
}

public class TokenExpiredException(string message) : Exception(message)
{
}

public class RevokedTokenException(string message) : Exception(message)
{
}