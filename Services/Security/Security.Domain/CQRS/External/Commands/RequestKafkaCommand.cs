using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Domain.CQRS.External.Commands
{
    /// <summary>
    /// Request kafka command data
    /// </summary>
    public class RequestKafkaCommand
    {
        /// <summary>
        /// Guid kafka event
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Operation name
        /// </summary>
        public String? NameOperation { get; set; }
    }
}
