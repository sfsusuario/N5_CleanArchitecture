using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// Permissions save repository
        /// </summary>
        public Task Save();
    }
}