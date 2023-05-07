using Moq;
using Security.Domain.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Test.Mocks
{
    public static class MockUnitOfWork
    {
        public static Mock<IUnitOfWork> GetUnitOfWork()
        {
            var mockUow = new Mock<IUnitOfWork>();
            var mockPermissionsQuery = MockPermissionsRepository.PermissionsQueryRepository();
            var mockPermissionsCommand = MockPermissionsRepository.PermissionsCommandRepository();

            mockUow.Setup(r => r.PermissionsQueryRepository).Returns(mockPermissionsQuery.Object);
            mockUow.Setup(r => r.PermissionsCommandRepository).Returns(mockPermissionsCommand.Object);

            return mockUow;
        }
    }
}
