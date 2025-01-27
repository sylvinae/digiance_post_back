using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using posts_back.Data;
using posts_back.Data.Entities;
using posts_back.DTO;

namespace posts_back.Helpers;

public static class PostSyncHelper
{
    public static async Task<(int newPosts, int updatedPosts)> SyncPostsAsync(
        HttpClient httpClient, string apiUrl, AppDbContext context)
    {
        var posts = await FetchPostsAsync(httpClient, apiUrl);
        return await SavePostsAsync(context, posts);
    }

    private static async Task<List<PostEntity>> FetchPostsAsync(HttpClient httpClient, string apiUrl)
    {
        var response = await httpClient.GetStringAsync(apiUrl);
        var posts = JsonConvert.DeserializeObject<List<PostDto>>(response);

        return posts?.Select(p => new PostEntity
        {
            Id = p.Id,
            UserId = p.UserId,
            Title = p.Title,
            Body = p.Body,
            Hash = HashGenerator.HashPost(p)
        }).ToList() ?? [];
    }

    private static async Task<(int newPosts, int updatedPosts)> SavePostsAsync(
        AppDbContext context, List<PostEntity> posts)
    {
        const int batchSize = 500;
        var newPosts = new List<PostDto>();
        var updatedPosts = new List<PostDto>();

        for (var i = 0; i < posts.Count; i += batchSize)
        {
            var batch = posts.Skip(i).Take(batchSize).ToList();
            var batchIds = batch.Select(p => p.Id).ToList();
            var existingPosts = await context.Posts
                .Where(p => batchIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var (toAdd, toUpdate) = ProcessPosts(batch, existingPosts);

            if (toAdd.Count != 0)
            {
                await context.Posts.AddRangeAsync(toAdd);
                newPosts.AddRange(toAdd.Select(MapToPostDto));
            }

            if (toUpdate.Count != 0)
            {
                context.Posts.UpdateRange(toUpdate);
                updatedPosts.AddRange(toUpdate.Select(MapToPostDto));
            }

            await context.SaveChangesAsync();
        }

        return (newPosts.Count, updatedPosts.Count);
    }

    private static (List<PostEntity> toAdd, List<PostEntity> toUpdate) ProcessPosts(
        List<PostEntity> incoming,
        Dictionary<int, PostEntity> existing)
    {
        var toAdd = new List<PostEntity>();
        var toUpdate = new List<PostEntity>();

        foreach (var post in incoming)
            if (existing.TryGetValue(post.Id, out var existingPost))
            {
                if (post.Hash == existingPost.Hash) continue;
                existingPost.Title = post.Title;
                existingPost.Body = post.Body;
                existingPost.UserId = post.UserId;
                existingPost.Hash = post.Hash;
                toUpdate.Add(existingPost);
            }
            else
            {
                toAdd.Add(post);
            }

        return (toAdd, toUpdate);
    }

    private static PostDto MapToPostDto(PostEntity entity)
    {
        return new PostDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Title = entity.Title,
            Body = entity.Body
        };
    }
}