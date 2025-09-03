using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserProfileAsync(Guid userId);
    Task<UserDto> UpdateUserProfileAsync(Guid userId, UpdateUserDto updateUserDto);

}
