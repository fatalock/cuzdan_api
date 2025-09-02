using System.Security.Cryptography;
using Cuzdan.Application.DTOs;
using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.Services;


public class AuthService(IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork) : IAuthService
{
    
    public async Task RegisterAsync(RegisterUserDto registerDto)
    {
        if (await unitOfWork.Users.GetUserByEmailAsync(registerDto.Email) != null) throw new ConflictException("Email already exist");

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
    }
    public async Task<ApiResponse<AuthResult>> LoginAsync(LoginUserDto loginDto)
    {

        var user = await unitOfWork.Users.GetUserByEmailAsync(loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Invalid email or password.");


        var accessToken = jwtTokenGenerator.GenerateAccessToken(user);

        var refreshToken = GenerateRefreshToken(user.Id);

        await unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await unitOfWork.SaveChangesAsync();

        return new ApiResponse<AuthResult> { IsSuccessful = true, SuccessMessage = "Login succesful", Data = new AuthResult{ AccessToken = accessToken, RefreshToken = refreshToken.Token } };
    }

    public async Task<ApiResponse<string>> RefresAccesshAsync(string refreshToken)
    {
        var storedToken = await unitOfWork.RefreshTokens.GetRefreshTokenByTokenAsync(refreshToken);

        if (storedToken == null) throw new InvalidTokenException("Invalid refresh token.");

        if (storedToken.ExpiresAt < DateTime.UtcNow) throw new TokenExpiredException("Refresh token Expired.");

        if (storedToken.IsRevoked) throw new RevokedTokenException("Refresh token revoked");

        var user = await unitOfWork.Users.GetByIdAsync(storedToken.UserId);

        if (user == null) throw new NotFoundException("User not exist.");
  
        var accessToken = jwtTokenGenerator.GenerateAccessToken(user);
        return new ApiResponse<string> { IsSuccessful = true, SuccessMessage = "New access token granted", Data = accessToken};
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