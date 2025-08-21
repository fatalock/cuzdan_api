using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);

}