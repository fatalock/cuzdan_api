using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;
public class RefreshTokenRepository(CuzdanContext context) : Repository<RefreshToken>(context), IRefreshTokenRepository
{

    public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string Token)
    {
        return await Context.RefreshTokens.FirstOrDefaultAsync(u => u.Token == Token);
    }

}