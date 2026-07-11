using FluentValidation;
using Security.Domain.CQRS.Repository.Commands;

namespace Security.Application.Validators
{
    /// <summary>
    /// Validation rules for RequestPermissionCommand
    /// </summary>
    public class RequestPermissionCommandValidator : AbstractValidator<RequestPermissionCommand>
    {
        public RequestPermissionCommandValidator()
        {
            RuleFor(c => c.EmployeeForename).NotEmpty();
            RuleFor(c => c.EmployeeSurname).NotEmpty();
            RuleFor(c => c.PermissionType).GreaterThan(0);
            RuleFor(c => c.PermissionDate).NotEqual(default(System.DateTime));
        }
    }
}
