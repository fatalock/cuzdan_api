using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;

public class UserRepository(CuzdanContext context) : Repository<User>(context), IUserRepository
{

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await Context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

}