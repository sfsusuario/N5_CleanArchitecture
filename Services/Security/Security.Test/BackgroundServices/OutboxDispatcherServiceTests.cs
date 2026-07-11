using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Security.Domain.Constants;
using Security.Domain.CQRS.External.Commands;
using Security.Domain.Entities;
using Security.Domain.External.Command;
using Security.Infrastructure.BackgroundServices;
using Security.Infrastructure.Data;
using Shouldly;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Security.Test.BackgroundServices
{
    public class OutboxDispatcherServiceTests
    {
        private static (OutboxDispatcherService Service, IServiceScopeFactory ScopeFactory, Mock<IKafkaCommandExternal> Kafka, Mock<IElasticSearchCommandExternal> ElasticSearch)
            CreateService(string databaseName)
        {
            var mockKafka = new Mock<IKafkaCommandExternal>();
            var mockElasticSearch = new Mock<IElasticSearchCommandExternal>();

            var services = new ServiceCollection();
            services.AddDbContext<SecurityContext>(options => options.UseInMemoryDatabase(databaseName));
            services.AddSingleton(mockKafka.Object);
            services.AddSingleton(mockElasticSearch.Object);
            var provider = services.BuildServiceProvider();

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            var service = new OutboxDispatcherService(scopeFactory, NullLogger<OutboxDispatcherService>.Instance);

            return (service, scopeFactory, mockKafka, mockElasticSearch);
        }

        [Fact]
        public async Task ProcessPendingMessagesAsync_DispatchesPendingRowsAndMarksThemProcessed()
        {
            var (service, scopeFactory, mockKafka, mockElasticSearch) = CreateService(Guid.NewGuid().ToString());

            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SecurityContext>();
                context.OutboxMessages.AddRange(
                    new OutboxMessage
                    {
                        Channel = OutboxChannels.Kafka,
                        Payload = JsonSerializer.Serialize(new RequestKafkaCommand { Id = Guid.NewGuid(), NameOperation = KafkaPermissionActions.REQUEST }),
                        CreatedAt = DateTime.UtcNow
                    },
                    new OutboxMessage
                    {
                        Channel = OutboxChannels.Elasticsearch,
                        Payload = JsonSerializer.Serialize(new RequestElasticSearchCommand { Id = 1 }),
                        CreatedAt = DateTime.UtcNow
                    },
                    new OutboxMessage
                    {
                        Channel = OutboxChannels.Kafka,
                        Payload = JsonSerializer.Serialize(new RequestKafkaCommand { Id = Guid.NewGuid(), NameOperation = KafkaPermissionActions.GET }),
                        CreatedAt = DateTime.UtcNow,
                        ProcessedAt = DateTime.UtcNow.AddMinutes(-1) // already processed
                    });
                await context.SaveChangesAsync();
            }

            await service.ProcessPendingMessagesAsync(CancellationToken.None);

            mockKafka.Verify(k => k.RequestAsync(It.IsAny<RequestKafkaCommand>()), Times.Once);
            mockElasticSearch.Verify(e => e.RequestAsync(It.IsAny<RequestElasticSearchCommand>()), Times.Once);

            using var verifyScope = scopeFactory.CreateScope();
            var verifyContext = verifyScope.ServiceProvider.GetRequiredService<SecurityContext>();
            var stillPending = await verifyContext.OutboxMessages.Where(m => m.ProcessedAt == null).CountAsync();
            stillPending.ShouldBe(0);
        }

        [Fact]
        public async Task ProcessPendingMessagesAsync_FailedDelivery_LeavesMessagePendingWithRetryInfo()
        {
            var (service, scopeFactory, mockKafka, _) = CreateService(Guid.NewGuid().ToString());
            mockKafka.Setup(k => k.RequestAsync(It.IsAny<RequestKafkaCommand>()))
                .ThrowsAsync(new InvalidOperationException("broker unreachable"));

            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SecurityContext>();
                context.OutboxMessages.Add(new OutboxMessage
                {
                    Channel = OutboxChannels.Kafka,
                    Payload = JsonSerializer.Serialize(new RequestKafkaCommand { Id = Guid.NewGuid(), NameOperation = KafkaPermissionActions.REQUEST }),
                    CreatedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }

            await service.ProcessPendingMessagesAsync(CancellationToken.None);

            using var verifyScope = scopeFactory.CreateScope();
            var verifyContext = verifyScope.ServiceProvider.GetRequiredService<SecurityContext>();
            var message = await verifyContext.OutboxMessages.SingleAsync();

            message.ProcessedAt.ShouldBeNull();
            message.RetryCount.ShouldBe(1);
            message.LastError.ShouldBe("broker unreachable");
        }
    }
}
