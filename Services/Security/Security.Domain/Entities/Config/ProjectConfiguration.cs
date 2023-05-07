using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Domain.Entities.Config
{
    public class ProjectConfiguration
    {
        public string? KafkaConnection { get; set; }

        public string? ElasticSearchConnection { get; set;}
    }
}
