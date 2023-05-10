using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Domain.CQRS.External.Commands
{
    /// <summary>
    /// Request elastic search command data
    /// </summary>
    public class RequestElasticSearchCommand
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
