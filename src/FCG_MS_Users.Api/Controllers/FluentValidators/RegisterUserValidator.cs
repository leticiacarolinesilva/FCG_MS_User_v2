using FCG_MS_Users.Application.Dtos;
using FluentValidation;
using System.Text.RegularExpressions;

namespace FCG_MS_Users.Api.Controllers.FluentValidators;

public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(100).WithMessage("Nome pode ter no máximo 100 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Must(ContainNumber).WithMessage("Password must contain at least one number")
            .Must(ContainLetter).WithMessage("Password must contain at least one letter")
            .Must(ContainSpecialChar).WithMessage("Password must contain at least one special character");

        RuleFor(x => x.ConfirmationPassword)
            .NotEmpty().WithMessage("Confirmation password is required")
            .Equal(x => x.Password).WithMessage("Confirmation password must match Password");
    }

    private bool ContainNumber(string password) =>
     Regex.IsMatch(password ?? "", @"[0-9]+");

    private bool ContainLetter(string password) =>
        Regex.IsMatch(password ?? "", @"[a-zA-Z]+");

    private bool ContainSpecialChar(string password) =>
        Regex.IsMatch(password ?? "", @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
}
