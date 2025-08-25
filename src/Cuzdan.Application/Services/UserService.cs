using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Services;

public class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<UserProfileDto> GetUserProfileAsync(Guid Id)
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

    public async Task UpdateUserProfileAsync(Guid userId, UpdateUserDto updateUserDto)
    {
                
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        // 2. Email'in başka bir kullanıcı tarafından kullanılıp kullanılmadığını kontrol et
        var existingUserWithEmail = await _userRepository.GetUserByEmailAsync(updateUserDto.Email);
        if (existingUserWithEmail != null && existingUserWithEmail.Id != userId)
        {
            throw new ConflictException("Email is already in use by another account.");
        }

        // 3. Kullanıcı bilgilerini güncelle
        user.Name = updateUserDto.Name;
        user.Email = updateUserDto.Email;

        // 4. Eğer yeni bir şifre gönderilmişse, hash'leyip güncelle
        if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
        }

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
    
}