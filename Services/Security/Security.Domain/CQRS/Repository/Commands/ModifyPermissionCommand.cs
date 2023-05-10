using MediatR;
using Security.Domain.DTO.Response;
using System;

namespace Security.Domain.CQRS.Repository.Commands
{
    /// <summary>
    /// ModifyPermissionCommand class
    /// </summary>
    public class ModifyPermissionCommand : IRequest<PermissionResponse>
    {
        /// <summary>
        /// Employee identifier
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Employee forename
        /// </summary>
        public string? EmployeeForename { get; set; }

        /// <summary>
        /// Employee surname
        /// </summary>
        public string? EmployeeSurname { get; set; }

        /// <summary>
        /// Employee permission type
        /// </summary>
        public int PermissionType { get; set; }

        /// <summary>
        /// Employee permission date
        /// </summary>
        public DateTime PermissionDate { get; set; }
    }
}
