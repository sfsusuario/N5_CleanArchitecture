using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Security.Domain.CQRS.External.Commands;
using Security.Domain.External.Command;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Security.Domain.Entities.Config;
using Security.Infrastructure.Resilience;

namespace Security.Infrastructure.External.Command
{
    /// <summary>
    /// Kafka command external logic
    /// </summary>
    public class KafkaCommandExternal : IKafkaCommandExternal, IDisposable
    {
        /// <summary>
        /// Kafka producer instance. The constructor opens a real connection to the
        /// broker, so this class is registered as a Singleton (see Program.cs) and
        /// disposed once per process instead of once per DI resolution.
        /// </summary>
        private readonly IProducer<Null, string> _producer;

        private readonly string _topic;

        /// <summary>
        /// Retry + circuit breaker pipeline. Kept as an instance field (not per-call) because
        /// the circuit breaker is stateful — it needs to persist across calls to actually trip.
        /// </summary>
        private readonly IAsyncPolicy _resiliencePolicy;

        /// <summary>
        /// KafkaCommandExternal constructor
        /// </summary>
        /// <param name="options">Project configuration</param>
        /// <param name="logger">Logger</param>
        public KafkaCommandExternal(IOptions<ProjectConfiguration> options, ILogger<KafkaCommandExternal> logger) {

            var config = new ProducerConfig
            {
                BootstrapServers = options.Value.KafkaConnection,
            };

            _topic = options.Value.KafkaTopic;
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _resiliencePolicy = ResiliencePolicyFactory.CreateDefault(logger, "Kafka");
        }

        /// <summary>
        /// Request to elastic search
        /// </summary>
        /// <param name="entity">Commmand entity</param>
        /// <returns>Command response</returns>
        public async Task<RequestKafkaCommand> RequestAsync(RequestKafkaCommand entity)
        {
            await _resiliencePolicy.ExecuteAsync(() => this._producer.ProduceAsync(_topic, new Message<Null, string> {
                Value = JsonSerializer.Serialize(entity)
            }));

            return entity;
        }

        public void Dispose()
        {
            _producer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
