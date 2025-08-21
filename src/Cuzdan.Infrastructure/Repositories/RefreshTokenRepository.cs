using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;
public class RefreshTokenRepository(CuzdanContext context) : IRefreshTokenRepository
{
    private readonly CuzdanContext _context = context;


    public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string Token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(u => u.Token == Token);
    }
    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
    }
}