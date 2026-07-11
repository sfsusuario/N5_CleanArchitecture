
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Security.Domain.Contracts.Persistence;
using Security.Domain.Entities;
using Security.Domain.External.Command.Base;
using Security.Domain.Repositories.Command;
using Security.Domain.Repositories.Query;
using Security.Infrastructure.Repository.Query;
using Security.Infrastructure.Repository.Command;
using Security.Infrastructure.Repository.Command.Base;
using Security.Infrastructure.Data;

namespace Security.Infrastructure.Repository
{
    /// <summary>
    /// UnitOfWork class
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SecurityContext _context;
        private IPermissionsQueryRepository _permissionQueryRepository;
        private IPermissionsCommandRepository _permissionCommandRepository;
        private IPermissionTypesQueryRepository _permissionTypesQueryRepository;
        private ICommandExternal<OutboxMessage> _outboxMessages;
        private IDbContextTransaction _transaction;

        public UnitOfWork(SecurityContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get permission query repository
        /// </summary>
        public IPermissionsQueryRepository PermissionsQueryRepository =>
            _permissionQueryRepository ??= new PermissionsQueryRepository(_context);

        /// <summary>
        /// Get permission command repository
        /// </summary>
        /// <returns></returns>
        public IPermissionsCommandRepository PermissionsCommandRepository =>
            _permissionCommandRepository ??= new PermissionsCommandRepository(_context);

        /// <summary>
        /// Get permission types query repository
        /// </summary>
        public IPermissionTypesQueryRepository PermissionTypesQueryRepository =>
            _permissionTypesQueryRepository ??= new PermissionTypesQueryRepository(_context);

        /// <summary>
        /// Outbox repository. Reuses the generic CommandRepository&lt;T&gt; already used by
        /// the other command repositories — an outbox row is just another tracked entity.
        /// </summary>
        public ICommandExternal<OutboxMessage> OutboxMessages =>
            _outboxMessages ??= new CommandRepository<OutboxMessage>(_context);

        /// <summary>
        /// Dispose unit of work
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Commits all pending changes tracked by the context. Repositories only track changes
        /// (Add/Modify/Remove); persistence happens here, so a handler that performs several
        /// repository calls can still commit them atomically — either in one Save() call, or
        /// across several Save() calls wrapped in BeginTransactionAsync/CommitTransactionAsync.
        /// </summary>
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction is null) return;
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction is null) return;
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
