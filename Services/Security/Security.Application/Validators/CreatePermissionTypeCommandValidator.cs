using FluentValidation;
using Security.Domain.CQRS.Repository.Commands;

namespace Security.Application.Validators
{
    /// <summary>
    /// Validation rules for CreatePermissionTypeCommand
    /// </summary>
    public class CreatePermissionTypeCommandValidator : AbstractValidator<CreatePermissionTypeCommand>
    {
        public CreatePermissionTypeCommandValidator()
        {
            RuleFor(c => c.Description).NotEmpty().MaximumLength(200);
        }
    }
}
