using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Security.Domain.Entities;
using Security.Domain.Repositories.Query;
using Security.Infrastructure.Data;
using Security.Infrastructure.Repository.Query.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        /// <param name="configuration">Configuration</param>
        /// <param name="context">Security context instance</param>
        public PermissionsQueryRepository(IConfiguration configuration, SecurityContext context) 
            : base(configuration, context)
        {

        }

        /// <summary>
        /// Get all permissions
        /// </summary>
        public async Task<IReadOnlyList<Permissions>> GetPermissionsAsync()
        {
            var me = this;
            try
            {
                return await Task.Factory.StartNew<IReadOnlyList<Permissions>>(() => {
                    return me._context
                        .Permissions
                        .Include(u=>u.PermissionTypeRef)
                        .ToList();
                });
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        
        /// <summary>
        /// Get permission by id
        /// </summary>
        /// <param name="id">Permission identifier</param>
        public async Task<Permissions> GetPermissionAsync(long id)
        {
            var me = this;
            try
            {
                return await Task.Factory.StartNew<Permissions>(() => {
                    return me._context
                        .Permissions
                        .Include(u => u.PermissionTypeRef)
                        .FirstOrDefault( e => e.Id == id);
                });
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}