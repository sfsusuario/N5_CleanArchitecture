using System;
using Security.Domain.Entities;

namespace Security.Domain.DTO.Response
{
    public class PermissionResponse
    {
        public long Id { get; set; }
        public string? EmployeeForename { get; set; }
        public string? EmployeeSurname { get; set; }
        public int PermissionType { get; set; }
        public DateTime PermissionDate { get; set; }
        public PermissionsType? PermissionsType { get; set; }
    }
}
