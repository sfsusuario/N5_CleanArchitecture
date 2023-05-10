
using AutoMapper;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Security.Domain.Contracts.Persistence;
using Security.Domain.Repositories.Command;
using Security.Domain.Repositories.Query;
using Security.Infrastructure.Repository.Query;
using Security.Infrastructure.Repository.Command;
using Microsoft.Extensions.Configuration;
using Security.Infrastructure.Data;

namespace HR.LeaveManagement.Persistence.Repositories
{
    /// <summary>
    /// UnitOfWork class
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SecurityContext _context;
        private readonly IConfiguration _configuration;
        private IPermissionsQueryRepository _permissionQueryRepository;
        private IPermissionsCommandRepository _permissionCommandRepository;

        public UnitOfWork(SecurityContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Get permission query repository
        /// </summary>
        public IPermissionsQueryRepository PermissionsQueryRepository => 
            _permissionQueryRepository ??= new PermissionsQueryRepository(_configuration, _context);

        /// <summary>
        /// Get permission command repository
        /// </summary>
        /// <returns></returns>
        public IPermissionsCommandRepository PermissionsCommandRepository => 
            _permissionCommandRepository ??= new PermissionsCommandRepository(_context);

        /// <summary>
        /// Dispose unit of work
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Save unit of work
        /// </summary>
        public async Task Save() 
        {
            await _context.SaveChangesAsync();
        }
    }
}
