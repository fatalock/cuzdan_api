using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{


    public async Task<UserDto> GetUserProfileAsync(Guid userId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId);

        if (user == null) throw new NotFoundException("User not found.");


        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
        };
    }

    public async Task<UserDto> UpdateUserProfileAsync(Guid userId, UpdateUserDto updateUserDto)
    {

        var user = await unitOfWork.Users.GetByIdAsync(userId);
        if (user is null) throw new NotFoundException("User not found.");


        var existingUserWithEmail = await unitOfWork.Users.GetUserByEmailAsync(updateUserDto.Email);
        if (existingUserWithEmail != null && existingUserWithEmail.Id != userId) throw new ConflictException("Email is already in use by another account.");


        user.Name = updateUserDto.Name;
        user.Email = updateUserDto.Email;

        if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
        }

        await unitOfWork.SaveChangesAsync(CancellationToken.None);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
        };
    }
    
}