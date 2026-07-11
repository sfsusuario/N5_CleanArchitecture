using FluentValidation;
using Security.Domain.CQRS.Repository.Commands;

namespace Security.Application.Validators
{
    /// <summary>
    /// Validation rules for ModifyPermissionCommand
    /// </summary>
    public class ModifyPermissionCommandValidator : AbstractValidator<ModifyPermissionCommand>
    {
        public ModifyPermissionCommandValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0);
            RuleFor(c => c.EmployeeForename).NotEmpty();
            RuleFor(c => c.EmployeeSurname).NotEmpty();
            RuleFor(c => c.PermissionType).GreaterThan(0);
            RuleFor(c => c.PermissionDate).NotEqual(default(System.DateTime));
        }
    }
}
