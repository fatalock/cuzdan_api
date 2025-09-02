using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken> 
{
    Task<RefreshToken?> GetRefreshTokenByTokenAsync(string Token);

}