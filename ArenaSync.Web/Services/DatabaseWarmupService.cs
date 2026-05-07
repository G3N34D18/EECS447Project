using ArenaSync.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public sealed class DatabaseWarmupService : IHostedService
    {
        private static readonly TimeSpan WarmupTimeout = TimeSpan.FromSeconds(90);

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseWarmupService> _logger;

        public DatabaseWarmupService(
            IServiceProvider serviceProvider,
            ILogger<DatabaseWarmupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(WarmupTimeout);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var strategy = context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    if (!await context.Database.CanConnectAsync(timeout.Token))
                    {
                        _logger.LogWarning("Database warmup could not establish a connection.");
                        return;
                    }

                    await context.Events
                        .AsNoTracking()
                        .Select(e => e.Id)
                        .Take(1)
                        .ToListAsync(timeout.Token);
                });

                _logger.LogInformation("Database warmup completed.");
            }
            catch (OperationCanceledException) when (timeout.IsCancellationRequested)
            {
                _logger.LogWarning("Database warmup timed out after {Seconds} seconds.", WarmupTimeout.TotalSeconds);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database warmup failed; the first user request may retry normally.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
