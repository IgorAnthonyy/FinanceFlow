using System;
using Transactions.API.Domain.Enums;

namespace Transactions.API.Application.DTOs;

public class TransactionFilterRequest
{
    public Guid UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public TransactionType? Type { get; set; }
    public Guid? BankAccountId { get; set; }
    public Guid? CategoryId { get; set; }
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
