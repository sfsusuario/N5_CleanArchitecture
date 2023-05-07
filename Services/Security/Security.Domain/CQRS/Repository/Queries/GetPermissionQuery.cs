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
        public long Id { get; private set; }

        public GetPermissionQuery(long Id)
        {
            this.Id = Id;
        }

    }
}
