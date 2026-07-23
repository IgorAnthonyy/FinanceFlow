using System;
using System.Linq.Expressions;
using LinqKit;
using Transactions.API.Domain.Entities;
using Transactions.API.Domain.Enums;

namespace Transactions.API.Domain.Filters;

public class TransactionFilter
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

    public Expression<Func<Transaction, bool>> ApplyFilter()
    {
        var query = PredicateBuilder.New<Transaction>(x => x.UserId == UserId);

        if(StartDate.HasValue)
            query = query.And(t => t.TransactionDate >= StartDate.Value);

        if(EndDate.HasValue)
            query = query.And(t => t.TransactionDate <= EndDate.Value.Date.AddDays(1).AddTicks(-1));

        if(MinAmount.HasValue)
            query = query.And(t => t.Amount >= MinAmount.Value);

        if(MaxAmount.HasValue)
            query = query.And(t => t.Amount <= MaxAmount.Value);

        if(Type.HasValue)
            query = query.And(t => t.Type == Type.Value);

        if(BankAccountId.HasValue)
            query = query.And(t => t.BankAccountId == BankAccountId.Value);

        if(CategoryId.HasValue)
            query = query.And(t => t.CategoryId == CategoryId.Value);

        if(!string.IsNullOrEmpty(SearchTerm))
            query = query.And(t => t.Title.ToLower().Contains(SearchTerm.Trim().ToLower()) || (t.Description != null && t.Description.ToLower().Contains(SearchTerm.Trim().ToLower())));

        return query;
    }
}
