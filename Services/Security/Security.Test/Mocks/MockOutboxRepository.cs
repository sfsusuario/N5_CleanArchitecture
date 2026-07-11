using Moq;
using Security.Domain.Entities;
using Security.Domain.External.Command.Base;
using System.Threading.Tasks;

namespace Security.Test.Mocks
{
    /// <summary>
    /// Mock outbox repository
    /// </summary>
    public static class MockOutboxRepository
    {
        public static Mock<ICommandExternal<OutboxMessage>> GetOutboxRepository()
        {
            var mockRepo = new Mock<ICommandExternal<OutboxMessage>>();

            mockRepo.Setup(r => r.RequestAsync(It.IsAny<OutboxMessage>()))
                .Returns((OutboxMessage message) => Task.FromResult(message));

            return mockRepo;
        }
    }
}
