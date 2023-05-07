using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Security.Domain.Entities.Base;

namespace Security.Domain.Entities
{
    /// <summary>
    /// Permissions entity
    /// </summary>
    [Table("Permissions")]
    public class Permissions: BaseEntity
    {
        public string? EmployeeForename { get; set; }
        public string? EmployeeSurname { get; set; }

        public int PermissionType { get; set; }
        public DateTime PermissionDate { get; set; }

        // Relationship
        [ForeignKey("PermissionType")]
        public PermissionsType? PermissionTypeRef { get; set; }
    }
}