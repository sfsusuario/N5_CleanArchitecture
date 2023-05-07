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
        public long Id { get; set; }
        public string? EmployeeForename { get; set; }
        public string? EmployeeSurname { get; set; }
        public int PermissionType { get; set; }
        public DateTime PermissionDate { get; set; }
    }
}
