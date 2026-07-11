using MediatR;
using Security.Domain.Entities;
using System.Collections.Generic;

namespace Security.Domain.CQRS.Repository.Queries
{
    /// <summary>
    /// GetPermissionTypesQuery class
    /// </summary>
    public record GetPermissionTypesQuery : IRequest<List<PermissionsType>>
    {

    }
}
