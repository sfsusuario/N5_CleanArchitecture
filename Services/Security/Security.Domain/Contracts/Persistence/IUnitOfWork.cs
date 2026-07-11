using System;
using System.Threading.Tasks;
using Security.Domain.Entities;
using Security.Domain.External.Command.Base;
using Security.Domain.Repositories.Command;
using Security.Domain.Repositories.Query;

namespace Security.Domain.Contracts.Persistence
{
    /// <summary>
    /// UnitOfWork interface contract
    /// </summary>
    public interface IUnitOfWork: IDisposable
    {
        /// <summary>
        /// Permissions query repository
        /// </summary>
        public IPermissionsQueryRepository PermissionsQueryRepository { get; }

        /// <summary>
        /// Permissions command repository
        /// </summary>
        public IPermissionsCommandRepository PermissionsCommandRepository { get; }

        /// <summary>
        /// Permission types query repository (lookup table for RequestPermission/ModifyPermission)
        /// </summary>
        public IPermissionTypesQueryRepository PermissionTypesQueryRepository { get; }

        /// <summary>
        /// Permission types command repository (create new lookup entries)
        /// </summary>
        public IPermissionTypesCommandRepository PermissionTypesCommandRepository { get; }

        /// <summary>
        /// Outbox repository. Handlers write to this instead of calling Kafka/Elasticsearch
        /// directly, so the notification is committed atomically with the business change.
        /// </summary>
        public ICommandExternal<OutboxMessage> OutboxMessages { get; }

        /// <summary>
        /// Permissions save repository
        /// </summary>
        public Task Save();

        /// <summary>
        /// Starts an explicit DB transaction, needed when a handler must call Save() more than
        /// once (e.g. to obtain a database-generated Id before writing an outbox row that
        /// references it) while still committing everything atomically.
        /// </summary>
        public Task BeginTransactionAsync();

        /// <summary>
        /// Commits the transaction started by <see cref="BeginTransactionAsync"/>.
        /// </summary>
        public Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the transaction started by <see cref="BeginTransactionAsync"/>.
        /// </summary>
        public Task RollbackTransactionAsync();
    }
}