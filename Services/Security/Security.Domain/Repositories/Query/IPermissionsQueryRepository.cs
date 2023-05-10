using Security.Domain.Entities;
using Security.Domain.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Security.Domain.Repositories.Query
{
    /// <summary>
    /// PermissionsQueryRepository interface
    /// </summary>
    public interface IPermissionsQueryRepository : IQueryRepository<Permissions>
    {
        /// <summary>
        /// Get permissions async
        /// </summary>
        /// <returns>Permissions</returns>
        Task<IReadOnlyList<Permissions>> GetPermissionsAsync();

        /// <summary>
        /// Get permission async
        /// </summary>
        /// <param name="id">Permission identifier</param>
        /// <returns>Permission</returns>
        Task<Permissions> GetPermissionAsync(Int64 id);
    }
}