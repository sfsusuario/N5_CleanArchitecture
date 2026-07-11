using AutoMapper;
using Moq;
using Security.Domain.Constants;
using Security.Domain.Contracts.Persistence;
using Security.Application.Handlers.CommandHandler;
using Security.Application.Handlers.QueryHandlers;
using Security.Application.Mapper;
using Security.Domain.CQRS.External.Commands;
using Security.Domain.CQRS.Repository.Queries;
using Security.Domain.Entities;
using Security.Domain.Repositories.Query;
using Security.Test.Mocks;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Security.Domain.External.Command;

namespace Security.Test.Handlers
{
    public class GetPermissionsHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IKafkaCommandExternal> _mockKafka;

        public GetPermissionsHandlerTests()
        {
            _mockUow = MockUnitOfWork.GetUnitOfWork();
            _mockKafka = MockKafka.GetKafka();

            var mapperConfig = new MapperConfiguration(c => 
            {
                c.AddProfile<SecurityMappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetPermissions()
        {
            var handler = new GetPermissionsHandler(_mockUow.Object, _mockKafka.Object);

            var result = await handler.Handle(new GetPermissionsQuery(), CancellationToken.None);

            result.ShouldBeOfType<List<Permissions>>();

            result.Count.ShouldBe(3);
        }

        [Fact]
        public async Task GetPermissions_PublishesGetEventToKafka()
        {
            var handler = new GetPermissionsHandler(_mockUow.Object, _mockKafka.Object);

            await handler.Handle(new GetPermissionsQuery(), CancellationToken.None);

            _mockKafka.Verify(k => k.RequestAsync(
                It.Is<RequestKafkaCommand>(c => c.NameOperation == KafkaPermissionActions.GET)),
                Times.Once);
        }

        [Fact]
        public async Task GetPermissions_PropagatesRepositoryException()
        {
            var mockRepo = MockPermissionsRepository.PermissionsQueryRepository();
            mockRepo.Setup(r => r.GetPermissionsAsync()).ThrowsAsync(new InvalidOperationException("db unavailable"));
            _mockUow.Setup(r => r.PermissionsQueryRepository).Returns(mockRepo.Object);

            var handler = new GetPermissionsHandler(_mockUow.Object, _mockKafka.Object);

            await Should.ThrowAsync<InvalidOperationException>(
                () => handler.Handle(new GetPermissionsQuery(), CancellationToken.None));
        }
    }
}
