using Moq;
using Security.Domain.Contracts.Persistence;
using Security.Application.Handlers.QueryHandlers;
using Security.Domain.CQRS.Repository.Queries;
using Security.Domain.Entities;
using Security.Test.Mocks;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Security.Test.Handlers
{
    public class GetPermissionTypesHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;

        public GetPermissionTypesHandlerTests()
        {
            _mockUow = MockUnitOfWork.GetUnitOfWork();
        }

        [Fact]
        public async Task GetPermissionTypes_ReturnsAllTypes()
        {
            var handler = new GetPermissionTypesHandler(_mockUow.Object);

            var result = await handler.Handle(new GetPermissionTypesQuery(), CancellationToken.None);

            result.ShouldBeOfType<List<PermissionsType>>();
            result.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetPermissionTypes_PropagatesRepositoryException()
        {
            var mockRepo = MockPermissionTypesRepository.GetRepository();
            mockRepo.Setup(r => r.GetAllAsync()).ThrowsAsync(new InvalidOperationException("db unavailable"));
            _mockUow.Setup(r => r.PermissionTypesQueryRepository).Returns(mockRepo.Object);

            var handler = new GetPermissionTypesHandler(_mockUow.Object);

            await Should.ThrowAsync<InvalidOperationException>(
                () => handler.Handle(new GetPermissionTypesQuery(), CancellationToken.None));
        }
    }
}
