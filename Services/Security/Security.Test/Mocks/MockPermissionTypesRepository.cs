using Moq;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command;
using Security.Domain.Repositories.Query;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Security.Test.Mocks
{
    /// <summary>
    /// Mock permission types repository
    /// </summary>
    public static class MockPermissionTypesRepository
    {
        public static Mock<IPermissionTypesQueryRepository> GetRepository()
        {
            var permissionTypes = new List<PermissionsType>
            {
                new PermissionsType { Id = 1, Description = "Vacation" },
                new PermissionsType { Id = 2, Description = "Sick leave" }
            };

            var mockRepo = new Mock<IPermissionTypesQueryRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(permissionTypes);

            return mockRepo;
        }

        public static Mock<IPermissionTypesCommandRepository> GetCommandRepository()
        {
            var mockRepo = new Mock<IPermissionTypesCommandRepository>();

            mockRepo.Setup(r => r.RequestAsync(It.IsAny<PermissionsType>()))
                .Returns((PermissionsType type) => Task.FromResult(type));

            return mockRepo;
        }
    }
}
