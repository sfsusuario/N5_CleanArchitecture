using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;
using Security.Domain.Repositories.Query;
using Security.Infrastructure.Data;
using Security.Infrastructure.Repository.Query.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Security.Infrastructure.Repository.Query
{
    /// <summary>
    /// PermissionTypesQueryRepository
    /// </summary>
    public class PermissionTypesQueryRepository : QueryRepository<PermissionsType>, IPermissionTypesQueryRepository
    {
        /// <summary>
        /// Permission types query repository constructor
        /// </summary>
        /// <param name="context">Security context instance</param>
        public PermissionTypesQueryRepository(SecurityContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Get all permission types
        /// </summary>
        public async Task<IReadOnlyList<PermissionsType>> GetAllAsync()
        {
            return await _context.PermissionTypes.ToListAsync();
        }
    }
}
