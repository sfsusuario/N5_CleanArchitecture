using Security.Domain.Entities;
using Security.Domain.Repositories.Query.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Security.Domain.Repositories.Query
{
    /// <summary>
    /// PermissionTypesQueryRepository interface
    /// </summary>
    public interface IPermissionTypesQueryRepository : IQueryRepository<PermissionsType>
    {
        /// <summary>
        /// Get all permission types async
        /// </summary>
        /// <returns>Permission types</returns>
        Task<IReadOnlyList<PermissionsType>> GetAllAsync();
    }
}
