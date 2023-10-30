using FluentValidation;

namespace MultiTenantOpenProject.Contracts.Account.Validators;

public class RegisterContractValidator : AbstractValidator<RegisterContract>
{
    public RegisterContractValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty()
                .WithMessage("Email address is required.")
            .EmailAddress()
                .WithMessage("A valid email address is required.");

        RuleFor(p => p.Password).NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
            .MaximumLength(100).WithMessage("Your password length must not exceed 100.");
        //     .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
        //     .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
        //     .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
        //     .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
    }
}
