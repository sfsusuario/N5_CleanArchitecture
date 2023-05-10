using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Security.Domain.CQRS.External.Commands;
using Security.Domain.Entities;
using Security.Domain.External.Command;
using Security.Domain.Repositories.Command.Base;
using static Confluent.Kafka.ConfigPropertyNames;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Security.Domain.Entities.Config;

namespace Security.Infrastructure.External.Command
{
    /// <summary>
    /// Kafka command external logic
    /// </summary>
    public class KafkaCommandExternal : IKafkaCommandExternal
    {
        /// <summary>
        /// Kadka instance
        /// </summary>
        private IProducer<Null, string> _producer;

        /// <summary>
        /// KafkaCommandExternal constructor
        /// </summary>
        /// <param name="options">Project configuration</param>
        public KafkaCommandExternal(IOptions<ProjectConfiguration> options) {

            var config = new ProducerConfig
            {
                BootstrapServers = options.Value.KafkaConnection,
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        /// <summary>
        /// Request to elastic search
        /// </summary>
        /// <param name="entity">Commmand entity</param>
        /// <returns>Command response</returns>
        public async Task<RequestKafkaCommand> RequestAsync(RequestKafkaCommand entity)
        {
            var result = await this._producer.ProduceAsync("my_custom_topic", new Message<Null, string> { 
                Value = JsonSerializer.Serialize(entity)
            });

            return entity;
        }
    }
}
