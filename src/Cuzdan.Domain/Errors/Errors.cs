namespace Cuzdan.Domain.Errors;

public static class DomainErrors
{
    public static class User
    {
        public static readonly DomainError EmailAlreadyExists = new(
            "User.EmailAlreadyExists",
            "A user with the same email already exists.",
            ErrorType.Conflict);

        public static readonly DomainError NotFound = new(
            "User.NotFound",
            "The user was not found.",
            ErrorType.NotFound);
    }

    public static class Auth
    {
        public static readonly DomainError InvalidToken = new(
            "Auth.InvalidToken",
            "The provided refresh token is invalid.",
            ErrorType.Unauthorized);

        public static readonly DomainError TokenExpired = new(
            "Auth.TokenExpired",
            "The refresh token has expired.",
            ErrorType.Unauthorized);

        public static readonly DomainError TokenRevoked = new(
            "Auth.TokenRevoked",
            "The refresh token has been revoked.",
            ErrorType.Unauthorized);

        public static readonly DomainError InvalidCredentials = new(
            "Auth.InvalidCredentials",
            "Invalid email or password.",
            ErrorType.Unauthorized);
    }

    public static class Wallet
    {
        public static readonly DomainError InsufficientBalance = new(
            "Wallet.InsufficientBalance",
            "There is not enough balance in the wallet for this operation.",
            ErrorType.InsufficientBalance);
        public static readonly DomainError NotFound = new(
            "Transaction.NotFound",
            "The Transaction was not found.",
            ErrorType.NotFound);
         public static readonly DomainError AccessDenied = new(
            "Wallet.AccessDenied",
            "User does not have permission to access this resource.",
            ErrorType.Forbidden);
    }
    public static class Transaction
    {
        public static readonly DomainError NotFound = new(
            "Transaction.NotFound",
            "The Transaction was not found.",
            ErrorType.NotFound);
        public static readonly DomainError NotPending = new(
            "Transaction.NotPending", 
            "The Transaction was not pending.", 
            ErrorType.NotFound);
    }
}