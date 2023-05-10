using Moq;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command;
using Security.Domain.Repositories.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Test.Mocks
{
    /// <summary>
    /// Mock permissions repository
    /// </summary>
    public static class MockPermissionsRepository
    {
        public static Mock<IPermissionsQueryRepository> PermissionsQueryRepository()
        {
            var leaveTypes = new List<Permissions>
            {
                new Permissions
                {
                    Id = 1,
                    EmployeeForename = "Samael",
                    EmployeeSurname = "Fierro",
                },
                new Permissions
                {
                    Id = 2,
                    EmployeeForename = "Kelly",
                    EmployeeSurname = "Barrera",
                },
                new Permissions
                {
                    Id = 3,
                    EmployeeForename = "Jonás",
                    EmployeeSurname = "Mendez",
                }
            };

            var mockRepo = new Mock<IPermissionsQueryRepository>();

            mockRepo.Setup(r => r.GetPermissionsAsync()).ReturnsAsync(leaveTypes);
            mockRepo.Setup(r => r.GetPermissionAsync(It.IsAny<long>())).ReturnsAsync(leaveTypes[0]);

            return mockRepo;
        }
        public static Mock<IPermissionsCommandRepository> PermissionsCommandRepository()
        {
            var leaveType = new Permissions
            {
                Id = 1,
                EmployeeForename = "Samael",
                EmployeeSurname = "Fierro",
                PermissionType = 1,
                PermissionTypeRef = new PermissionsType()
                {
                    Description= "description",
                    Id = 1
                }
            };

            var mockRepo = new Mock<IPermissionsCommandRepository>();

            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Permissions>())).Returns((Permissions leaveType) =>
            {
                return Task.FromResult<Permissions>(leaveType);
            });

            mockRepo.Setup(r => r.RequestAsync(It.IsAny<Permissions>())).Returns((Permissions leaveType) =>
            {
                return Task.FromResult<Permissions>(leaveType);
            });

            return mockRepo;
        }
    }
}
