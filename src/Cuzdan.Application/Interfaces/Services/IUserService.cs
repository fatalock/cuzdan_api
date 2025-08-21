using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(Guid Id);


}
