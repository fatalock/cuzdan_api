using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;
public class UserRepository(CuzdanContext context) : IUserRepository
{
    private readonly CuzdanContext _context = context;

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
        public async Task<User?> GetUserByIdAsync(Guid Id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == Id);
    }


    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}