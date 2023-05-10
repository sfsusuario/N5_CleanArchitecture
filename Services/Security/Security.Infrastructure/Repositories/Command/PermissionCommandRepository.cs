using Security.Domain.Entities;
using Security.Domain.Repositories.Command;
using Security.Infrastructure.Data;
using Security.Infrastructure.Repository.Command.Base;

namespace Security.Infrastructure.Repository.Command
{
    /// <summary>
    /// PermissionCommandRepository
    /// </summary>
    public class PermissionsCommandRepository : CommandRepository<Permissions>, IPermissionsCommandRepository
    {
        /// <summary>
        /// Permissions repositort constructor
        /// </summary>
        /// <param name="context">Security context repository container</param>
        public PermissionsCommandRepository(SecurityContext context) : base(context)
        {

        }
    }
}