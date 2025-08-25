using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace Cuzdan.Application.DTOs;

public class BaseFilter<T> where T : class
{
    public Expression<Func<T, object>>? OrderBy { get; set; }
    public bool IsDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}