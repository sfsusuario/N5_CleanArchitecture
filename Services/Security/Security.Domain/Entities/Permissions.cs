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

        /// <summary>
        /// Employee permission type relationship
        /// </summary>
        [ForeignKey("PermissionType")]
        public PermissionsType? PermissionTypeRef { get; set; }
    }
}