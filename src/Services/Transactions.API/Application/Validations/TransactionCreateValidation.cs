using FluentValidation;
using Transactions.API.Application.DTOs;

namespace Transactions.API.Application.Validations;

public class TransactionCreateValidation : AbstractValidator<TransactionCreate>
{
    public TransactionCreateValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId é obrigatório.");

        RuleFor(x => x.BankAccount.BankAccountId)
            .NotEmpty().WithMessage("BankAccountId é obrigatório.");

        RuleFor(x => x.BankAccount.Balance)
            .GreaterThan(0).WithMessage("O saldo deve ser maior que zero.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId é obrigatório.");

        RuleFor(x => x.TransactionDate)
            .NotEmpty().WithMessage("A data da transação é obrigatória.");
    }
}
