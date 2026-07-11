using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Security.Domain.CQRS.Repository.Queries;
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
    public class PermissionTypesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly PermissionTypesController _controller;

        public PermissionTypesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new PermissionTypesController(_mockMediator.Object, Mock.Of<ILogger<PermissionTypesController>>());
        }

        [Fact]
        public async Task Get_Success_ReturnsOkWithList()
        {
            var types = new List<PermissionsType> { new PermissionsType { Id = 1, Description = "Vacation" } };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetPermissionTypesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(types);

            var result = await _controller.Get();

            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(types);
        }

        [Fact]
        public async Task Get_MediatorThrows_ReturnsBadRequest()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetPermissionTypesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("db unavailable"));

            var result = await _controller.Get();

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }
    }
}
