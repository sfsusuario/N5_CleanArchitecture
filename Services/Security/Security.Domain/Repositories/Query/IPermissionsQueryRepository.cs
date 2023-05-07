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
        Task<IReadOnlyList<Permissions>> GetPermissionsAsync();
        Task<Permissions> GetPermissionAsync(Int64 id);
    }
}