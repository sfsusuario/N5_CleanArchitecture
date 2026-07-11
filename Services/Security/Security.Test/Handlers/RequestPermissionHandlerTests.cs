using AutoMapper;
using Moq;
using Security.Domain.Constants;
using Security.Domain.Contracts.Persistence;
using Security.Application.Handlers.CommandHandler;
using Security.Application.Mapper;
using Security.Domain.CQRS.External.Commands;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command;
using Security.Test.Mocks;
using Shouldly;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.DTO.Response;

namespace Security.Test.Handlers
{
    public class RequestPermissionHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Permissions _leaveTypeDto;
        private readonly RequestPermissionHandler _handler;

        public RequestPermissionHandlerTests()
        {
            _mockUow = MockUnitOfWork.GetUnitOfWork();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<SecurityMappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
            _handler = new RequestPermissionHandler(_mockUow.Object);

            _leaveTypeDto = new Permissions
            {
                Id = 1,
                EmployeeForename = "Samael",
                EmployeeSurname = "Fierro",
            };
        }

        [Fact]
        public async Task Valid_LeaveType_Added()
        {
            var result = await _handler.Handle(new RequestPermissionCommand(), CancellationToken.None);

            result.ShouldBeOfType<PermissionResponse>();
        }

        [Fact]
        public async Task RequestPermission_WritesKafkaAndElasticsearchOutboxRowsAndCommits()
        {
            await _handler.Handle(new RequestPermissionCommand(), CancellationToken.None);

            _mockUow.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _mockUow.Verify(u => u.OutboxMessages.RequestAsync(
                It.Is<OutboxMessage>(m => m.Channel == OutboxChannels.Kafka
                    && JsonSerializer.Deserialize<RequestKafkaCommand>(m.Payload, (JsonSerializerOptions)null).NameOperation == KafkaPermissionActions.REQUEST)),
                Times.Once);
            _mockUow.Verify(u => u.OutboxMessages.RequestAsync(
                It.Is<OutboxMessage>(m => m.Channel == OutboxChannels.Elasticsearch)),
                Times.Once);
            _mockUow.Verify(u => u.CommitTransactionAsync(), Times.Once);
            _mockUow.Verify(u => u.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task RequestPermission_WrapsRepositoryExceptionInApplicationExceptionAndRollsBack()
        {
            var mockCommandRepo = MockPermissionsRepository.PermissionsCommandRepository();
            mockCommandRepo.Setup(r => r.RequestAsync(It.IsAny<Permissions>()))
                .ThrowsAsync(new InvalidOperationException("db unavailable"));
            _mockUow.Setup(r => r.PermissionsCommandRepository).Returns(mockCommandRepo.Object);

            var handler = new RequestPermissionHandler(_mockUow.Object);

            await Should.ThrowAsync<ApplicationException>(
                () => handler.Handle(new RequestPermissionCommand(), CancellationToken.None));

            _mockUow.Verify(u => u.RollbackTransactionAsync(), Times.Once);
            _mockUow.Verify(u => u.CommitTransactionAsync(), Times.Never);
        }
    }
}
