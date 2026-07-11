using Moq;
using Security.Domain.Contracts.Persistence;
using Security.Application.Handlers.CommandHandler;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.Entities;
using Security.Test.Mocks;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Security.Test.Handlers
{
    public class CreatePermissionTypeHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;

        public CreatePermissionTypeHandlerTests()
        {
            _mockUow = MockUnitOfWork.GetUnitOfWork();
        }

        [Fact]
        public async Task CreatePermissionType_ReturnsCreatedType()
        {
            var handler = new CreatePermissionTypeHandler(_mockUow.Object);

            var result = await handler.Handle(new CreatePermissionTypeCommand { Description = "Maternity leave" }, CancellationToken.None);

            result.ShouldBeOfType<PermissionsType>();
            result.Description.ShouldBe("Maternity leave");
        }

        [Fact]
        public async Task CreatePermissionType_PersistsViaUnitOfWorkSave()
        {
            var handler = new CreatePermissionTypeHandler(_mockUow.Object);

            await handler.Handle(new CreatePermissionTypeCommand { Description = "Maternity leave" }, CancellationToken.None);

            _mockUow.Verify(u => u.PermissionTypesCommandRepository.RequestAsync(
                It.Is<PermissionsType>(t => t.Description == "Maternity leave")),
                Times.Once);
            _mockUow.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public async Task CreatePermissionType_PropagatesRepositoryException()
        {
            var mockRepo = MockPermissionTypesRepository.GetCommandRepository();
            mockRepo.Setup(r => r.RequestAsync(It.IsAny<PermissionsType>()))
                .ThrowsAsync(new InvalidOperationException("db unavailable"));
            _mockUow.Setup(r => r.PermissionTypesCommandRepository).Returns(mockRepo.Object);

            var handler = new CreatePermissionTypeHandler(_mockUow.Object);

            await Should.ThrowAsync<InvalidOperationException>(
                () => handler.Handle(new CreatePermissionTypeCommand { Description = "X" }, CancellationToken.None));
        }
    }
}
