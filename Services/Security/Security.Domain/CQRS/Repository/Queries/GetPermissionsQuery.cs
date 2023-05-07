using MediatR;
using Security.Domain.Entities;
using System.Collections.Generic;

namespace Security.Domain.CQRS.Repository.Queries
{
    /// <summary>
    /// GetPermissionQuery class
    /// </summary>
    public record GetPermissionsQuery : IRequest<List<Permissions>>
    {

    }
}
