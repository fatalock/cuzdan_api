using Cuzdan.Api.Schemas;

namespace Cuzdan.Api.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(Guid Id);


}
