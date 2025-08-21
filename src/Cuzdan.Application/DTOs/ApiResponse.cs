namespace Cuzdan.Application.DTOs;

public class ApiResponse<T>
{
    public bool IsSuccessful { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public T? Data { get; set; }
}
public class ApiResponse : ApiResponse<object>
{
}