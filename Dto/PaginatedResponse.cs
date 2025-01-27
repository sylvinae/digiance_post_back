namespace posts_back.DTO;

public class PaginatedResponse<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<T> Items { get; set; } = [];
}