namespace Cuzdan.Application.DTOs;

public class BaseFilter<T> where T : class
{
    public bool? IsDescending { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}