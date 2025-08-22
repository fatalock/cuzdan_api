using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserProfileDto> GetProfileAsync(Guid Id)
    {
        var user = await _userRepository.GetByIdAsync(Id);

        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        return new UserProfileDto
        {
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
        };
    }
}