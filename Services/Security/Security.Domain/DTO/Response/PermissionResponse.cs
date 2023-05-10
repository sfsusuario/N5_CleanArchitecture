using System;
using Security.Domain.Entities;

namespace Security.Domain.DTO.Response
{
    public class PermissionResponse
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

        /// <summary>
        /// Employee permission type relationship
        /// </summary>
        public PermissionsType? PermissionsType { get; set; }
    }
}
