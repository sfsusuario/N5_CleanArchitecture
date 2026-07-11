using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;
using Security.Infrastructure.Data;
using Security.Infrastructure.Repository.Query;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Security.Test.Repositories
{
    public class PermissionsQueryRepositoryTests
    {
        private static SecurityContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SecurityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new SecurityContext(options);
        }

        [Fact]
        public async Task GetPermissionsAsync_ReturnsAllSeededPermissions()
        {
            using var context = CreateContext();
            var permissionType = new PermissionsType { Description = "Vacation" };
            context.PermissionTypes.Add(permissionType);
            await context.SaveChangesAsync();

            context.Permissions.AddRange(
                new Permissions { EmployeeForename = "Samael", EmployeeSurname = "Fierro", PermissionType = permissionType.Id, PermissionDate = DateTime.UtcNow },
                new Permissions { EmployeeForename = "Kelly", EmployeeSurname = "Barrera", PermissionType = permissionType.Id, PermissionDate = DateTime.UtcNow });
            await context.SaveChangesAsync();
            var repository = new PermissionsQueryRepository(context);

            var result = await repository.GetPermissionsAsync();

            result.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetPermissionAsync_ReturnsMatchingPermission()
        {
            using var context = CreateContext();
            var permissionType = new PermissionsType { Description = "Vacation" };
            context.PermissionTypes.Add(permissionType);
            await context.SaveChangesAsync();

            var seeded = new Permissions { EmployeeForename = "Samael", EmployeeSurname = "Fierro", PermissionType = permissionType.Id, PermissionDate = DateTime.UtcNow };
            context.Permissions.Add(seeded);
            await context.SaveChangesAsync();

            var repository = new PermissionsQueryRepository(context);

            var result = await repository.GetPermissionAsync(seeded.Id);

            result.ShouldNotBeNull();
            result.EmployeeForename.ShouldBe("Samael");
        }

        [Fact]
        public async Task GetPermissionAsync_UnknownId_ReturnsNull()
        {
            using var context = CreateContext();
            var repository = new PermissionsQueryRepository(context);

            var result = await repository.GetPermissionAsync(999);

            result.ShouldBeNull();
        }
    }
}
