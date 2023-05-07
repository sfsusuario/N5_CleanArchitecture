using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Domain.CQRS.External.Commands
{
    public class RequestKafkaCommand
    {
        public Guid Id { get; set; }

        public String? NameOperation { get; set; }
    }
}
