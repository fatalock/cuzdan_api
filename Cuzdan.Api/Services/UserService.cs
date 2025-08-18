using Cuzdan.Api.Models;
using Microsoft.EntityFrameworkCore;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Data;
using Azure;
using Cuzdan.Api.Interfaces;

namespace Cuzdan.Api.Services;

public class UserService(CuzdanContext context) : IUserService
{
    private readonly CuzdanContext _context = context;

    public async Task<UserProfileDto> GetProfileAsync(Guid Id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        return new UserProfileDto
        {
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
        };
    }
}