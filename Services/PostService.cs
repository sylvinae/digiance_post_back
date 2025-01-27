using Microsoft.EntityFrameworkCore;
using posts_back.Data;
using posts_back.Data.Entities;
using posts_back.DTO;
using posts_back.Services.Interfaces;

namespace posts_back.Services;

public class PostService(AppDbContext context) : IPostService
{
    public async Task<PaginatedResponse<PostDto>> GetPostsAsync(int page, string searchQuery, bool searchTitles)
    {
        ValidatePaginationParameters(ref page);

        var query = context.Posts.AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            var keywords = searchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (searchTitles)
                query = query.Where(p =>
                    keywords.All(keyword =>
                        p.Title.Contains(keyword))
                );
            else
                query = query.Where(p =>
                    keywords.All(keyword =>
                        p.Title.Contains(keyword) ||
                        p.Body.Contains(keyword))
                );
        }

        query = query.OrderBy(p => p.Id);
        return await PaginatePostsAsync(query, page);
    }

    private static async Task<PaginatedResponse<PostDto>> PaginatePostsAsync(
        IQueryable<PostEntity> query, int page)
    {
        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * 10)
            .Take(10)
            .Select(p => new PostDto
            {
                Id = p.Id,
                UserId = p.UserId,
                Title = p.Title,
                Body = p.Body
            })
            .ToListAsync();

        return new PaginatedResponse<PostDto>
        {
            Page = page,
            PageSize = 10,
            TotalItems = totalItems,
            TotalPages = totalItems / 10,
            Items = items
        };
    }

    private static void ValidatePaginationParameters(ref int page)
    {
        page = Math.Max(1, page);
    }
}