using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface IUserRepository : IRepository<User> 
{
    Task<User?> GetUserByEmailAsync(string email);

}