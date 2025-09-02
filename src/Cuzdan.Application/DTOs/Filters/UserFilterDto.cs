using Cuzdan.Domain.Entities;
using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.DTOs;



public class UserFilterDto : BaseFilter<User>
{

    public UserSortField? OrderBy { get; set; }


    public string? Name { get; set; }

    public string? Email { get; set; }


    public UserRole? Role { get; set; }


    public DateTime? StartDate { get; set; }


    public DateTime? EndDate { get; set; }
}