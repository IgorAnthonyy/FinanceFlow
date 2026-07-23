using FluentValidation;
using Wallet.API.Application.DTOs;

namespace Wallet.API.Application.Validations;

public class BankAccountCreateValidation : AbstractValidator<BankAccountCreate>
{
    public BankAccountCreateValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId é obrigatório.");

        RuleFor(x => x.BankName)
            .NotEmpty().WithMessage("O nome do banco é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do banco deve ter no máximo 100 caracteres.");

        RuleFor(x => x.AccountName)
            .NotEmpty().WithMessage("O nome da conta é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da conta deve ter no máximo 100 caracteres.");
    }
}
