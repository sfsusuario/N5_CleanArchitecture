using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Security.Domain.Constants;
using Security.Domain.CQRS.External.Commands;
using Security.Domain.External.Command;
using Security.Infrastructure.Data;

namespace Security.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Polls the OutboxMessages table and delivers pending rows to Kafka/Elasticsearch.
    /// This is the read side of the Outbox pattern: handlers only write rows (atomically with
    /// their business change); this service is the sole caller of the external clients for
    /// state-changing operations, so a Kafka/Elasticsearch outage never fails an HTTP request —
    /// the message just stays pending and is retried on the next poll.
    /// </summary>
    public class OutboxDispatcherService : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
        private const int BatchSize = 20;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxDispatcherService> _logger;

        public OutboxDispatcherService(IServiceScopeFactory scopeFactory, ILogger<OutboxDispatcherService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingMessagesAsync(stoppingToken);
                }
                catch (Exception exp)
                {
                    // A poll cycle failing (e.g. DB unreachable) must not kill the loop —
                    // the next cycle will simply try again.
                    _logger.LogError(exp, "Outbox dispatch cycle failed");
                }

                try
                {
                    await Task.Delay(PollInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected on graceful shutdown.
                }
            }
        }

        /// <summary>
        /// Runs a single dispatch cycle. Public so tests can call it directly instead of
        /// driving the infinite ExecuteAsync loop.
        /// </summary>
        public async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SecurityContext>();
            var kafka = scope.ServiceProvider.GetRequiredService<IKafkaCommandExternal>();
            var elasticSearch = scope.ServiceProvider.GetRequiredService<IElasticSearchCommandExternal>();

            var pending = await context.OutboxMessages
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.CreatedAt)
                .Take(BatchSize)
                .ToListAsync(cancellationToken);

            foreach (var message in pending)
            {
                try
                {
                    if (message.Channel == OutboxChannels.Kafka)
                    {
                        var payload = JsonSerializer.Deserialize<RequestKafkaCommand>(message.Payload)
                            ?? throw new InvalidOperationException("Empty Kafka outbox payload");
                        await kafka.RequestAsync(payload);
                    }
                    else if (message.Channel == OutboxChannels.Elasticsearch)
                    {
                        var payload = JsonSerializer.Deserialize<RequestElasticSearchCommand>(message.Payload)
                            ?? throw new InvalidOperationException("Empty Elasticsearch outbox payload");
                        await elasticSearch.RequestAsync(payload);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unknown outbox channel '{message.Channel}'");
                    }

                    message.ProcessedAt = DateTime.UtcNow;
                }
                catch (Exception exp)
                {
                    // Left unprocessed on purpose: the next poll cycle retries it. The Kafka/
                    // Elasticsearch clients already retried transient failures internally
                    // (see ResiliencePolicyFactory) before this catch is ever reached.
                    message.RetryCount++;
                    message.LastError = exp.Message;
                    _logger.LogWarning(exp,
                        "Failed to dispatch outbox message {Id} on channel {Channel} (attempt {RetryCount})",
                        message.Id, message.Channel, message.RetryCount);
                }
            }

            if (pending.Count > 0)
            {
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
