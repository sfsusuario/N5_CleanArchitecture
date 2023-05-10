using MediatR;
using Security.Domain.Entities;
using System;

namespace Security.Domain.CQRS.Repository.Queries
{
    /// <summary>
    /// GetPermissionQuery class
    /// </summary>
    public class GetPermissionQuery : IRequest<Permissions>
    {
        /// <summary>
        /// Permision identifier
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Get permssion query
        /// </summary>
        /// <param name="Id">Identifier</param>
        public GetPermissionQuery(long Id)
        {
            this.Id = Id;
        }

    }
}
