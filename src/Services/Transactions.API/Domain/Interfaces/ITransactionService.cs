using Transactions.API.Application.DTOs;
using Transactions.API.Domain.Entities;
using Transactions.API.Domain.Enums;

namespace Transactions.API.Domain.Interfaces;

public interface ITransactionService
{
    Transaction CreateTransaction(TransactionCreate transaction);
}
