using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Errors;

namespace Cuzdan.Application.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{


    public async Task<Result<UserDto>> GetUserProfileAsync(Guid userId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId);

        if (user == null) return Result<UserDto>.Failure(DomainErrors.User.NotFound);


        var result = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
        };
        return Result<UserDto>.Success(result);
    }

    public async Task<Result<UserDto>> UpdateUserProfileAsync(Guid userId, UpdateUserDto updateUserDto)
    {

        var user = await unitOfWork.Users.GetByIdAsync(userId);
        if (user is null) return Result<UserDto>.Failure(DomainErrors.User.NotFound);


        var existingUserWithEmail = await unitOfWork.Users.GetUserByEmailAsync(updateUserDto.Email);
        if (existingUserWithEmail != null && existingUserWithEmail.Id != userId) return Result<UserDto>.Failure(DomainErrors.User.EmailAlreadyExists);


        user.Name = updateUserDto.Name;
        user.Email = updateUserDto.Email;

        if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
        }

        await unitOfWork.SaveChangesAsync(CancellationToken.None);

        var result = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
        };
        return Result<UserDto>.Success(result);

    }
    
}