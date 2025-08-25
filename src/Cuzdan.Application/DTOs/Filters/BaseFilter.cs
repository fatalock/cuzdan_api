using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace Cuzdan.Application.DTOs;

public class BaseFilter<T> where T : class
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}