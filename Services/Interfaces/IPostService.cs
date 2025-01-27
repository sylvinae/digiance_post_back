using posts_back.DTO;

namespace posts_back.Services.Interfaces;

public interface IPostService
{
    Task<PaginatedResponse<PostDto>> GetPostsAsync(int page, string searchQuery, bool searchTitles);
}