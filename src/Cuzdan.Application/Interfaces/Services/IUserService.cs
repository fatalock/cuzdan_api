using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetUserProfileAsync(Guid Id);
    Task UpdateUserProfileAsync(Guid userId, UpdateUserDto updateUserDto);

}
