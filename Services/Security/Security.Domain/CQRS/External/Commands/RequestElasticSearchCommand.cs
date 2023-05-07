using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Domain.CQRS.External.Commands
{
    public class RequestElasticSearchCommand
    {
        public long Id { get; set; }
        public string? EmployeeForename { get; set; }
        public string? EmployeeSurname { get; set; }
        public int PermissionType { get; set; }
        public DateTime PermissionDate { get; set; }
    }
}
