using FinanceFlow.SharedKernel.Entities;
using FinanceFlow.SharedKernel.Services;
using Transactions.API.Application.DTOs;
using Transactions.API.Domain.Entities;
using Transactions.API.Domain.Enums;
using Transactions.API.Domain.Interfaces;

namespace Transactions.API.Domain.Services;

public class TransactionService : BaseService, ITransactionService
{
    public TransactionService(UserData userData) : base(userData)
    {
    }

    public Transaction CreateTransaction(TransactionCreate transaction)
    {
        if (transaction.Amount <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.", nameof(transaction.Amount));

        if (transaction.Type == TransactionType.Expense && transaction.BankAccount.Balance < transaction.Amount)
            throw new ArgumentException("Saldo insuficiente.", nameof(transaction.BankAccount.Balance));

        var entity = new Transaction(
            transaction.UserId,
            transaction.BankAccount.BankAccountId,
            transaction.Title,
            transaction.Description,
            transaction.Amount,
            transaction.Type,
            transaction.CategoryId
        );

        entity.PrepareInsert();

        return entity;
    }
}
