using Cuzdan.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Cuzdan.Api.Schemas;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Cuzdan.Api.Data;
using System.Security.Cryptography;
using Cuzdan.Api.Interfaces;


namespace Cuzdan.Api.Services;


public class AuthService(CuzdanContext context, IConfiguration configuration) : IAuthService
{
    private readonly CuzdanContext _context = context;
    private readonly IConfiguration _configuration = configuration;

    public async Task<ApiResponse> RegisterUserAsync(RegisterUserDto registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            return new ApiResponse { IsSuccessful = false, ErrorMessage = "Email already exist" };
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

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return new ApiResponse { IsSuccessful = true, SuccessMessage = "User Created", ErrorMessage = "Register succesful" }; ;
    }
    public async Task<ApiResponse<AuthResult>> LoginUserAsync(LoginUserDto loginDto)
    {

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return new ApiResponse<AuthResult> { IsSuccessful = false, ErrorMessage = "Invalid email or password." };
        }

        var accessToken = GenerateAccessToken(user);

        // 2. Create the long-lived Refresh Token
        var refreshToken = GenerateRefreshToken(user.Id);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new ApiResponse<AuthResult> { IsSuccessful = true, SuccessMessage = "Login succesful", Data = new AuthResult{ AccessToken = accessToken, RefreshToken = refreshToken.Token } };
    }

    public async Task<ApiResponse<string>> RefresAccesshAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null)
        {
            return new ApiResponse<string> { IsSuccessful = false, ErrorMessage = "Invalid refresh token." };
        }
        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return new ApiResponse<string> { IsSuccessful = false, ErrorMessage = "Refresh token Expired." };
        }
        if (storedToken.IsRevoked)
        {
            return new ApiResponse<string> { IsSuccessful = false, ErrorMessage = "Refresh token revoked" };
        }
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == storedToken.UserId);
        if (user == null) return new ApiResponse<string> { IsSuccessful = false, ErrorMessage = "User not exist." };
        var accessToken = GenerateAccessToken(user);
        return new ApiResponse<string> { IsSuccessful = true, SuccessMessage = "New access token granted", Data = accessToken};
    }
    private string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("Jwt:Key").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(1), // Token 1 gün geçerli olsun
            SigningCredentials = creds,
            Issuer = _configuration.GetSection("Jwt:Issuer").Value,
            Audience = _configuration.GetSection("Jwt:Audience").Value
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
        private RefreshToken GenerateRefreshToken(Guid userId)
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