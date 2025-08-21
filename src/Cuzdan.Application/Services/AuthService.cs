using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Cuzdan.Application.DTOs;
using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Services;


public class AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task RegisterAsync(RegisterUserDto registerDto)
    {
        if (await _userRepository.GetUserByEmailAsync(registerDto.Email) != null)
        {
            throw new ConflictException("Email already exist");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var newUser = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            Role = "User",
        };

        await _userRepository.AddUserAsync(newUser);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<ApiResponse<AuthResult>> LoginAsync(LoginUserDto loginDto)
    {

        var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);

        var refreshToken = GenerateRefreshToken(user.Id);

        await _refreshTokenRepository.AddRefreshTokenAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new ApiResponse<AuthResult> { IsSuccessful = true, SuccessMessage = "Login succesful", Data = new AuthResult{ AccessToken = accessToken, RefreshToken = refreshToken.Token } };
    }

    public async Task<ApiResponse<string>> RefresAccesshAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetRefreshTokenByTokenAsync(refreshToken);

        if (storedToken == null)
        {
            throw new InvalidTokenException("Invalid refresh token.");
        }
        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new TokenExpiredException("Refresh token Expired.");
        }
        if (storedToken.IsRevoked)
        {
            throw new RevokedTokenException("Refresh token revoked");
        }
        var user = await _userRepository.GetUserByIdAsync(storedToken.UserId);
        if (user == null)
        {
            throw new NotFoundException("User not exist.");
        }    
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
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