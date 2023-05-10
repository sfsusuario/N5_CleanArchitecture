using Moq;
using Security.Domain.Contracts.Persistence;
using Security.Domain.CQRS.External.Commands;
using Security.Domain.External.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Test.Mocks
{
    /// <summary>
    /// Mock kafka basic
    /// </summary>
    public static class MockKafka
    {
        public static Mock<IKafkaCommandExternal> GetKafka()
        {
            var mockKafka = new Mock<IKafkaCommandExternal>();
            return mockKafka;
        }
    }
}
