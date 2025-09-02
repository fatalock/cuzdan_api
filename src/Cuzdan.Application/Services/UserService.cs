using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{


    public async Task<UserProfileDto> GetUserProfileAsync(Guid Id)
    {
        var user = await unitOfWork.Users.GetByIdAsync(Id);

        if (user == null) throw new NotFoundException("User not found.");


        return new UserProfileDto
        {
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
        };
    }

    public async Task UpdateUserProfileAsync(Guid userId, UpdateUserDto updateUserDto)
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
    }
    
}