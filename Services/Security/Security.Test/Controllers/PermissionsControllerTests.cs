using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.CQRS.Repository.Queries;
using Security.Domain.DTO.Response;
using Security.Domain.Entities;
using Security.Presentation.Controllers;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Security.Test.Controllers
{
    public class PermissionsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly PermissionsController _controller;

        public PermissionsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new PermissionsController(_mockMediator.Object, Mock.Of<ILogger<PermissionsController>>());
        }

        [Fact]
        public async Task RequestPermission_NullCommand_ReturnsBadRequest()
        {
            var result = await _controller.RequestPermission(null!);

            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task RequestPermission_Success_ReturnsOkWithResponse()
        {
            var response = new PermissionResponse { Id = 1 };
            _mockMediator.Setup(m => m.Send(It.IsAny<RequestPermissionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.RequestPermission(new RequestPermissionCommand());

            var okResult = result.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(response);
        }

        [Fact]
        public async Task RequestPermission_MediatorThrows_ReturnsBadRequestWithMessage()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<RequestPermissionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ApplicationException("mapper failed"));

            var result = await _controller.RequestPermission(new RequestPermissionCommand());

            var badRequest = result.ShouldBeOfType<BadRequestObjectResult>();
            badRequest.Value.ShouldBe("mapper failed");
        }

        [Fact]
        public async Task GetPermissions_Success_ReturnsOkWithList()
        {
            var permissions = new List<Permissions> { new Permissions { Id = 1 } };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetPermissionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(permissions);

            var result = await _controller.GetPermissions();

            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(permissions);
        }

        [Fact]
        public async Task GetPermissions_MediatorThrows_ReturnsBadRequest()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetPermissionsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("db unavailable"));

            var result = await _controller.GetPermissions();

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ModifyPermission_NullCommand_ReturnsBadRequest()
        {
            var result = await _controller.ModifyPermission(1, null!);

            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ModifyPermission_IdMismatch_ReturnsBadRequest()
        {
            var result = await _controller.ModifyPermission(1, new ModifyPermissionCommand { Id = 2 });

            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ModifyPermission_Success_ReturnsOkWithResponse()
        {
            var response = new PermissionResponse { Id = 1 };
            _mockMediator.Setup(m => m.Send(It.IsAny<ModifyPermissionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.ModifyPermission(1, new ModifyPermissionCommand { Id = 1 });

            var okResult = result.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(response);
        }

        [Fact]
        public void Test_ReturnsOkWithLlamado()
        {
            var result = _controller.Test();

            var okResult = result.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe("Llamado");
        }
    }
}
