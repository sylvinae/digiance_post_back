using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using posts_back.DTO;
using posts_back.Services.Interfaces;

namespace posts_back.Controllers.Post;

[ApiController]
[Route("api/[controller]")]
public class PostController(IPostService postService, Channel<object> postChannel, ILogger<PostController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<PostDto>>> GetPosts(
        [FromQuery] int page,
        [FromQuery] string query = "",
        [FromQuery] bool searchTitles = false)

    {
        try
        {
            var result = await postService.GetPostsAsync(page, query, searchTitles);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("stream")]
    public async Task StreamPosts(CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        logger.LogInformation("Stream started. Waiting for new posts...");

        await foreach (var postWithFlag in postChannel.Reader.ReadAllAsync(cancellationToken))
        {
            var jsonData = JsonConvert.SerializeObject(postWithFlag);
            var sseEvent = $"data: {jsonData}\n\n";


            logger.LogInformation("Sending SSE event: {sseEvent}", sseEvent);

            await Response.WriteAsync(sseEvent, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }

        logger.LogInformation("Stream ended or cancelled.");
    }
}