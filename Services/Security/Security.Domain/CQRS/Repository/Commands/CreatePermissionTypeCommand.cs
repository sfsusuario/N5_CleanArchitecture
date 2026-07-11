using MediatR;
using Security.Domain.Entities;

namespace Security.Domain.CQRS.Repository.Commands
{
    /// <summary>
    /// CreatePermissionTypeCommand class
    /// </summary>
    public class CreatePermissionTypeCommand : IRequest<PermissionsType>
    {
        /// <summary>
        /// Permission type description
        /// </summary>
        public string? Description { get; set; }
    }
}
