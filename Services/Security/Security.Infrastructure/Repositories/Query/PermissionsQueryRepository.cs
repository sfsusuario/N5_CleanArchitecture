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
    /// PermissionsQueryRepository
    /// </summary>
    public class PermissionsQueryRepository : QueryRepository<Permissions>, IPermissionsQueryRepository
    {
        /// <summary>
        /// Permissions query repository constructor
        /// </summary>
        /// <param name="context">Security context instance</param>
        public PermissionsQueryRepository(SecurityContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Get all permissions
        /// </summary>
        public async Task<IReadOnlyList<Permissions>> GetPermissionsAsync()
        {
            // Real async I/O via EF Core; exceptions propagate as-is so callers see the
            // original EF exception instead of a re-wrapped, less informative one.
            return await _context.Permissions
                .Include(u => u.PermissionTypeRef)
                .ToListAsync();
        }

        /// <summary>
        /// Get permission by id
        /// </summary>
        /// <param name="id">Permission identifier</param>
        public async Task<Permissions> GetPermissionAsync(int id)
        {
            return await _context.Permissions
                .Include(u => u.PermissionTypeRef)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}