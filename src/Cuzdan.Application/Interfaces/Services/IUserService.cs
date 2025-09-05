using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IUserService
{
    Task<Result<UserDto>> GetUserProfileAsync(Guid userId);
    Task<Result<UserDto>> UpdateUserProfileAsync(Guid userId, UpdateUserDto updateUserDto);

}
