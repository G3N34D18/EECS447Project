using ArenaSync.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public static class DatabaseWarmupService
    {
        public static async Task WarmAsync(
            IServiceProvider serviceProvider,
            ILogger logger,
            TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(timeout);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var strategy = context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    if (!await context.Database.CanConnectAsync(timeoutSource.Token))
                    {
                        throw new InvalidOperationException("Database warmup could not establish a connection.");
                    }

                    await context.Events
                        .AsNoTracking()
                        .Select(e => e.Id)
                        .Take(1)
                        .ToListAsync(timeoutSource.Token);
                });

                logger.LogInformation("Database warmup completed before accepting requests.");
            }
            catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested)
            {
                logger.LogWarning("Database warmup timed out after {Seconds} seconds.", timeout.TotalSeconds);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Database warmup failed; user requests may retry normally.");
            }
        }
    }
}
