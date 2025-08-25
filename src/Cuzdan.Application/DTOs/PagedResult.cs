namespace Cuzdan.Application.DTOs;

public class PagedResult<T>
{
    public PagedResult() { }
    public PagedResult(int page, int pageSize, int totalCount, List<T>? items)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items;
    }

    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<T>? Items { get; set; }
}