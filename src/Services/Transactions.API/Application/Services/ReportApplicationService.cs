using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinanceFlow.SharedKernel.Applications;
using FinanceFlow.SharedKernel.Entities;
using FinanceFlow.SharedKernel.Extensions;
using Transactions.API.Application.DTOs;
using Transactions.API.Application.Interfaces;
using Transactions.API.Domain.Entities;
using Transactions.API.Domain.Enums;
using Transactions.API.Domain.Interfaces;

namespace Transactions.API.Application.Services;

public class ReportApplicationService : BaseApplication, IReportApplicationService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public ReportApplicationService(
        ITransactionRepository transactionRepository,
        IMapper mapper,
        UserData userData) : base(userData)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(PeriodFilterDto filter = null)
    {
        return await _transactionRepository.GetMonthlySummaryAsync(UserData.Id, filter);
    }

    public async Task<List<CategoryExpenseSummary>> GetExpensesByCategoryAsync(PeriodFilterDto filter = null)
    {
        return await _transactionRepository.GetExpensesByCategoryAsync(UserData.Id, filter);
    }

    public async Task<List<MonthlyEvolutionSummary>> GetMonthlyEvolutionAsync(PeriodFilterDto filter = null)
    {
        return await _transactionRepository.GetMonthlyEvolutionAsync(UserData.Id, filter);
    }

    public async Task<List<MonthlyEvolutionSummary>> GetDailyEvolutionAsync(PeriodFilterDto filter = null)
    {
        return await _transactionRepository.GetDailyEvolutionAsync(UserData.Id, filter);
    }

    public async Task<List<TransactionResponse>> GetLatestTransactionsAsync(PeriodFilterDto filter = null)
    {
        return await _transactionRepository.GetLatestTransactionsAsync(UserData.Id, 4, filter);
    }
}
