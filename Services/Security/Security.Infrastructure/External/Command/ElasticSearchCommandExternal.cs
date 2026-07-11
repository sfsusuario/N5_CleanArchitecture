using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Polly;
using Security.Domain.CQRS.External.Commands;
using Security.Domain.Entities.Config;
using Security.Domain.External.Command;
using Security.Infrastructure.Resilience;

namespace Security.Infrastructure.External.Command
{
    /// <summary>
    /// Elastic search command external logic
    /// </summary>
    public class ElasticSearchCommandExternal : IElasticSearchCommandExternal
    {
        /// <summary>
        /// Elastic search instance
        /// </summary>
        private readonly ElasticClient _client;

        /// <summary>
        /// Retry + circuit breaker pipeline. Kept as an instance field (not per-call) because
        /// the circuit breaker is stateful — it needs to persist across calls to actually trip.
        /// </summary>
        private readonly IAsyncPolicy _resiliencePolicy;

        /// <summary>
        /// ElasticSearchCommandExternal constructor. Pings Elasticsearch and creates the
        /// index if missing, so this class is registered as a Singleton (see Program.cs) —
        /// that network round trip must happen once per process, not once per request.
        /// </summary>
        /// <param name="options">Project configuration</param>
        /// <param name="logger">Logger</param>
        public ElasticSearchCommandExternal(IOptions<ProjectConfiguration> options, ILogger<ElasticSearchCommandExternal> logger) {

            // Elastic search
            var pool = new SingleNodeConnectionPool(new Uri(options.Value.ElasticSearchConnection));
            var settings = new ConnectionSettings(pool).DefaultIndex("permissions");
            _client = new ElasticClient(settings);
            if (!_client.Indices.Exists("permissions").Exists)
            {
                _client.Indices.Create("permissions");
            }

            _resiliencePolicy = ResiliencePolicyFactory.CreateDefault(logger, "Elasticsearch");
        }

        /// <summary>
        /// Request to elastic search
        /// </summary>
        /// <param name="entity">Commmand entity</param>
        /// <returns>Command response</returns>
        public async Task<RequestElasticSearchCommand> RequestAsync(RequestElasticSearchCommand entity)
        {
            await _resiliencePolicy.ExecuteAsync(() => _client.IndexDocumentAsync(entity));
            return entity;
        }
    }
}
