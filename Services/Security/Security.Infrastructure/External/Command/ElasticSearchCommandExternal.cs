using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using Security.Domain.CQRS.External.Commands;
using Security.Domain.Entities.Config;
using Security.Domain.External.Command;

namespace Security.Infrastructure.External.Command
{
    public class ElasticSearchCommandExternal : IElasticSearchCommandExternal
    {
        private readonly ElasticClient _client;
        public ElasticSearchCommandExternal(IOptions<ProjectConfiguration> options) {

            // Elastic search
            var pool = new SingleNodeConnectionPool(new Uri(options.Value.ElasticSearchConnection));
            var settings = new ConnectionSettings(pool)
                .DefaultIndex("books");
            _client = new ElasticClient(settings);
        }

        public async Task<RequestElasticSearchCommand> RequestAsync(RequestElasticSearchCommand entity)
        {
            _client.IndexDocument(entity);
            return entity;
        }
    }
}
