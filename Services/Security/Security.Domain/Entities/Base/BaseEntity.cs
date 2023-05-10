using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Security.Domain.Entities.Base
{
    /// <summary>
    /// Common base entity
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Identifier base entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }
    }
}