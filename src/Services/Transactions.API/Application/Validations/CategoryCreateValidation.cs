using FluentValidation;
using Transactions.API.Application.DTOs;

namespace Transactions.API.Application.Validations;

public class CategoryCreateValidation : AbstractValidator<CategoryCreate>
{
    public CategoryCreateValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.");

        RuleFor(x => x.Icon)
            .NotEmpty().WithMessage("O ícone é obrigatório.")
            .MaximumLength(50).WithMessage("O ícone deve ter no máximo 50 caracteres.");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("A cor é obrigatória.")
            .MaximumLength(50).WithMessage("A cor deve ter no máximo 50 caracteres.");
    }
}
