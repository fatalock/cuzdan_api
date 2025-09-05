using System.Security.Cryptography;
using Cuzdan.Application.DTOs;
using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Domain.Enums;
using Cuzdan.Domain.Errors;

namespace Cuzdan.Application.Services;


public class AuthService(IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork) : IAuthService
{

    public async Task<Result<UserDto>> RegisterAsync(RegisterUserDto registerDto)
    {
        if (await unitOfWork.Users.GetUserByEmailAsync(registerDto.Email) != null) return Result<UserDto>.Failure(DomainErrors.User.EmailAlreadyExists);
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var newUser = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            Role = UserRole.User,
        };

        await unitOfWork.Users.AddAsync(newUser);
        await unitOfWork.SaveChangesAsync();
        var result = new UserDto
        {
            Id = newUser.Id,
            Name = newUser.Name,
            Email = newUser.Email,
            CreatedAt = newUser.CreatedAt,
            Role = newUser.Role,
        };
        return Result<UserDto>.Success(result);

    }
    public async Task<Result<AuthResult>> LoginAsync(LoginUserDto loginDto)
    {

        var user = await unitOfWork.Users.GetUserByEmailAsync(loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash)) return Result<AuthResult>.Failure(DomainErrors.Auth.InvalidCredentials);


        var accessToken = jwtTokenGenerator.GenerateAccessToken(user);

        var refreshToken = GenerateRefreshToken(user.Id);

        await unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await unitOfWork.SaveChangesAsync();

        var result = new AuthResult { AccessToken = accessToken, RefreshToken = refreshToken.Token, UserId = user.Id };
        return Result<AuthResult>.Success(result);
    }

    public async Task<Result<AuthResult>> RefresAccesshAsync(string refreshToken)
    {
        var storedToken = await unitOfWork.RefreshTokens.GetRefreshTokenByTokenAsync(refreshToken);

        if (storedToken == null)
        {
            return Result<AuthResult>.Failure(DomainErrors.Auth.InvalidToken);
        }
        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result<AuthResult>.Failure(DomainErrors.Auth.TokenExpired);
        }
        if (storedToken.IsRevoked)
        {
            return Result<AuthResult>.Failure(DomainErrors.Auth.TokenRevoked);
        }
        
        var user = await unitOfWork.Users.GetByIdAsync(storedToken.UserId);
        if (user == null)
        {
            return Result<AuthResult>.Failure(DomainErrors.User.NotFound);
        }

        var accessToken = jwtTokenGenerator.GenerateAccessToken(user);
        var result = new AuthResult { AccessToken = accessToken, RefreshToken = refreshToken, UserId = user.Id };
        return Result<AuthResult>.Success(result);

    }
    
    private static RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return new RefreshToken
        {
            UserId = userId,
            Token = Convert.ToBase64String(randomNumber),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        };
    }
}