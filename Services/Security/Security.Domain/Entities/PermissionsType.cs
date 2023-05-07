using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Security.Domain.Entities.Base;

namespace Security.Domain.Entities
{
    /// <summary>
    /// PermissionsType entity
    /// </summary>
    [Table("PermissionTypes")]
    public class PermissionsType: BaseEntity
    {
        public string? Description { get; set; }
    }
}