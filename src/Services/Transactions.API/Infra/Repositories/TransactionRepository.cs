using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceFlow.SharedKernel.Extensions;
using Microsoft.EntityFrameworkCore;
using Transactions.API.Application.DTOs;
using Transactions.API.Domain.Entities;
using Transactions.API.Domain.Enums;
using Transactions.API.Domain.Filters;
using Transactions.API.Domain.Interfaces;
using Transactions.API.Infra.Data;

namespace Transactions.API.Infra.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly TransactionsDbContext _context;

    public TransactionRepository(TransactionsDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction> GetByIdAsync(Guid id)
    {
        return await _context.Transactions.FindAsync(id);
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId)
    {
        return await BaseQuery()
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByBankAccountIdAsync(Guid bankAccountId)
    {
        return await BaseQuery()
            .Where(t => t.BankAccountId == bankAccountId)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetFilteredTransactionsAsync(TransactionFilter filter)
    {
        var query = BaseQuery().Where(filter.ApplyFilter());
        
        var totalCount = await query.CountAsync();
        
        var page = filter.PageNumber > 0 ? filter.PageNumber : 1;
        var size = filter.PageSize > 0 ? filter.PageSize : 10;
        
        var items = await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
            
        return (items, totalCount);
    }

    public async Task AddAsync(Transaction entity)
    {
        await _context.Transactions.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(Guid userId, PeriodFilterDto filter = null)
    {
        var now = DateTime.UtcNow.ToBrasiliaTime();
        var start = filter?.StartDate ?? new DateTime(now.Year, now.Month, 1);
        var end = filter?.EndDate?.Date.AddDays(1).AddTicks(-1) ?? now;

        var totals = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.TransactionDate >= start && t.TransactionDate <= end)
            .GroupBy(t => t.Type)
            .Select(g => new { Type = g.Key, Sum = g.Sum(x => x.Amount) })
            .ToListAsync();

        var income = totals.FirstOrDefault(t => t.Type == TransactionType.Income)?.Sum ?? 0;
        var expense = totals.FirstOrDefault(t => t.Type == TransactionType.Expense)?.Sum ?? 0;

        return new MonthlySummaryDto { Income = income, Expense = expense };
    }

    public async Task<List<CategoryExpenseSummary>> GetExpensesByCategoryAsync(Guid userId, PeriodFilterDto filter = null)
    {
        var now = DateTime.UtcNow.ToBrasiliaTime();
        var start = filter?.StartDate ?? new DateTime(now.Year, now.Month, 1);
        var end = filter?.EndDate?.Date.AddDays(1).AddTicks(-1) ?? now;

        var totalExpense = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense && t.TransactionDate >= start && t.TransactionDate <= end)
            .SumAsync(t => t.Amount);

        if (totalExpense == 0)
            return new List<CategoryExpenseSummary>();

        var list = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense && t.TransactionDate >= start && t.TransactionDate <= end)
            .GroupBy(t => new { t.CategoryId, t.Category.Name, t.Category.Color, t.Category.Icon })
            .Select(g => new CategoryExpenseSummary
            {
                CategoryName = g.Key.Name,
                Amount = g.Sum(x => x.Amount),
                Percentage = (double)Math.Round((g.Sum(x => x.Amount) / totalExpense) * 100, 2),
                Color = g.Key.Color,
                Icon = g.Key.Icon
            })
            .ToListAsync();

        return list;
    }

    public async Task<List<MonthlyEvolutionSummary>> GetMonthlyEvolutionAsync(Guid userId, PeriodFilterDto filter = null)
    {
        var now = DateTime.UtcNow;
        var start = filter?.StartDate ?? new DateTime(now.Year, now.Month, 1).AddMonths(-5);
        var end = filter?.EndDate ?? now;

        start = new DateTime(start.Year, start.Month, 1);
        end = new DateTime(end.Year, end.Month, 1).AddMonths(1).AddDays(-1);

        var rawData = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.TransactionDate >= start && t.TransactionDate <= end)
            .GroupBy(t => new { Year = t.TransactionDate.Year, Month = t.TransactionDate.Month, Type = t.Type })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Type = g.Key.Type,
                Sum = g.Sum(x => x.Amount)
            })
            .ToListAsync();

        var evolutionList = new List<MonthlyEvolutionSummary>();

        var current = start;
        while (current <= end)
        {
            var monthData = rawData.Where(r => r.Year == current.Year && r.Month == current.Month).ToList();

            var income = monthData.FirstOrDefault(r => r.Type == TransactionType.Income)?.Sum ?? 0;
            var expense = monthData.FirstOrDefault(r => r.Type == TransactionType.Expense)?.Sum ?? 0;

            evolutionList.Add(new MonthlyEvolutionSummary
            {
                MonthName = current.ToString("MM/yy"),
                Income = income,
                Expense = expense
            });

            current = current.AddMonths(1);
        }

        return evolutionList;
    }

    public async Task<List<MonthlyEvolutionSummary>> GetDailyEvolutionAsync(Guid userId, PeriodFilterDto filter = null)
    {
        var now = DateTime.UtcNow;
        var start = filter?.StartDate ?? now.AddMonths(-2);
        var end = filter?.EndDate ?? now;

        start = start.Date;
        end = end.Date.AddDays(1).AddTicks(-1);

        var rawData = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.TransactionDate >= start && t.TransactionDate <= end)
            .GroupBy(t => new { Date = t.TransactionDate.Date, Type = t.Type })
            .Select(g => new
            {
                Date = g.Key.Date,
                Type = g.Key.Type,
                Sum = g.Sum(x => x.Amount)
            })
            .ToListAsync();

        var evolutionList = new List<MonthlyEvolutionSummary>();

        var current = start;
        while (current <= end)
        {
            var dayData = rawData.Where(r => r.Date == current).ToList();

            var income = dayData.FirstOrDefault(r => r.Type == TransactionType.Income)?.Sum ?? 0;
            var expense = dayData.FirstOrDefault(r => r.Type == TransactionType.Expense)?.Sum ?? 0;

            evolutionList.Add(new MonthlyEvolutionSummary
            {
                MonthName = current.ToString("dd/MM"),
                Income = income,
                Expense = expense
            });

            current = current.AddDays(1);
        }

        return evolutionList;
    }

    public async Task<List<TransactionResponse>> GetLatestTransactionsAsync(Guid userId, int count, PeriodFilterDto filter = null)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (filter?.StartDate != null) query = query.Where(t => t.TransactionDate >= filter.StartDate.Value);
        if (filter?.EndDate != null) query = query.Where(t => t.TransactionDate <= filter.EndDate.Value.Date.AddDays(1).AddTicks(-1));

        return await query
            .OrderByDescending(t => t.TransactionDate)
            .Take(count)
            .Select(t => new TransactionResponse
            {
                Id = t.Id,
                UserId = t.UserId,
                BankAccountId = t.BankAccountId,
                Title = t.Title,
                Description = t.Description,
                Amount = t.Amount,
                Type = t.Type,
                CategoryId = t.CategoryId,
                TransactionDate = t.TransactionDate,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();
    }

    private IQueryable<Transaction> BaseQuery(bool tracking = false)
    {
        return tracking ? _context.Transactions : _context.Transactions.AsNoTracking();
    }
}
