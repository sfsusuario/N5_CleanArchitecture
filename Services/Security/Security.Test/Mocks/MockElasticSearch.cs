using Moq;
using Security.Domain.External.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Test.Mocks
{
    /// <summary>
    /// Mock elastic search basic
    /// </summary>
    public class MockElasticSearch
    {
        public static Mock<IElasticSearchCommandExternal> GetElasticSearch()
        {
            var mockKafka = new Mock<IElasticSearchCommandExternal>();
            return mockKafka;
        }
    }
}
