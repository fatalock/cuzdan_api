using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(Guid Id);


}
