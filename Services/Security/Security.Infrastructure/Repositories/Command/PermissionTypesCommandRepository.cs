using Security.Domain.Entities;
using Security.Domain.Repositories.Command;
using Security.Infrastructure.Data;
using Security.Infrastructure.Repository.Command.Base;

namespace Security.Infrastructure.Repository.Command
{
    /// <summary>
    /// PermissionTypesCommandRepository
    /// </summary>
    public class PermissionTypesCommandRepository : CommandRepository<PermissionsType>, IPermissionTypesCommandRepository
    {
        /// <summary>
        /// Permission types command repository constructor
        /// </summary>
        /// <param name="context">Security context repository container</param>
        public PermissionTypesCommandRepository(SecurityContext context) : base(context)
        {

        }
    }
}
