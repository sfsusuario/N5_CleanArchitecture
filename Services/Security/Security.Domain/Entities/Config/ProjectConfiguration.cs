using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Domain.Entities.Config
{
    /// <summary>
    /// Project configuration schema
    /// </summary>
    public class ProjectConfiguration
    {
        /// <summary>
        /// URL Kafka connection
        /// </summary>
        public string? KafkaConnection { get; set; }

        /// <summary>
        /// URL ElasticSearch connection
        /// </summary>
        public string? ElasticSearchConnection { get; set;}

        /// <summary>
        /// Kafka topic used to publish permission events. Must match the topic
        /// provisioned by KAFKA_CREATE_TOPICS in docker-compose.yaml.
        /// </summary>
        public string KafkaTopic { get; set; } = "mytopic";
    }
}
