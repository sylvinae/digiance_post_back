using System.Threading.Channels;
using posts_back.Data;
using posts_back.Helpers;

namespace posts_back.BackgroundServices;

public class PostBackgroundService(
    IHttpClientFactory httpClientFactory,
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<PostBackgroundService> logger,
    Channel<object> postChannel)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var httpClient = httpClientFactory.CreateClient();

                var apiUrl = configuration["ExternalApis:JsonPlaceholder"];
                if (string.IsNullOrEmpty(apiUrl))
                {
                    logger.LogError("Missing API URL configuration");
                    continue;
                }

                logger.LogInformation("Starting sync...");
                var (newPosts, updatedPosts) = await PostSyncHelper.SyncPostsAsync(httpClient, apiUrl, context);

                if (newPosts > 0)
                    await postChannel.Writer.WriteAsync(new { operation = "added", count = newPosts }, stoppingToken);
                if (updatedPosts > 0)
                    await postChannel.Writer.WriteAsync(new { operation = "updated", count = updatedPosts },
                        stoppingToken);


                logger.LogInformation("Sync completed. {NewPosts} added, {UpdatedPosts} updated",
                    newPosts, updatedPosts);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during post synchronization");
            }
            finally
            {
                // 10 sec for testing purposes
                // await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
    }
}