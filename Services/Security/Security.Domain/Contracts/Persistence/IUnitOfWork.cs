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
        public IPermissionsQueryRepository PermissionsQueryRepository { get; }
        public IPermissionsCommandRepository PermissionsCommandRepository { get; }
        public Task Save();
    }
}