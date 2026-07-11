using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;
using Security.Infrastructure.Data;
using Security.Infrastructure.Repository;
using Security.Infrastructure.Repository.Command;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Security.Test.Repositories
{
    public class PermissionsCommandRepositoryTests
    {
        private static SecurityContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SecurityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new SecurityContext(options);
        }

        [Fact]
        public async Task RequestAsync_OnlyTracksChanges_DoesNotPersistUntilUnitOfWorkSave()
        {
            using var context = CreateContext();
            var repository = new PermissionsCommandRepository(context);

            await repository.RequestAsync(new Permissions
            {
                EmployeeForename = "Samael",
                EmployeeSurname = "Fierro",
                PermissionType = 1,
                PermissionDate = DateTime.UtcNow
            });

            // Not committed yet: CommandRepository<T> no longer calls SaveChangesAsync itself.
            (await context.Permissions.CountAsync()).ShouldBe(0);
        }

        [Fact]
        public async Task RequestAsync_ThenUnitOfWorkSave_PersistsEntity()
        {
            using var context = CreateContext();
            var unitOfWork = new UnitOfWork(context);

            await unitOfWork.PermissionsCommandRepository.RequestAsync(new Permissions
            {
                EmployeeForename = "Samael",
                EmployeeSurname = "Fierro",
                PermissionType = 1,
                PermissionDate = DateTime.UtcNow
            });
            await unitOfWork.Save();

            (await context.Permissions.CountAsync()).ShouldBe(1);
        }

        [Fact]
        public async Task UpdateAsync_ThenSave_PersistsChanges()
        {
            using var context = CreateContext();
            var seeded = new Permissions { EmployeeForename = "Old", EmployeeSurname = "Name", PermissionType = 1, PermissionDate = DateTime.UtcNow };
            context.Permissions.Add(seeded);
            await context.SaveChangesAsync();
            context.Entry(seeded).State = EntityState.Detached;

            var unitOfWork = new UnitOfWork(context);
            seeded.EmployeeForename = "New";
            await unitOfWork.PermissionsCommandRepository.UpdateAsync(seeded);
            await unitOfWork.Save();

            var updated = await context.Permissions.FirstAsync(p => p.Id == seeded.Id);
            updated.EmployeeForename.ShouldBe("New");
        }
    }
}
