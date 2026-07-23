using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.API.Application.Interfaces;
using Transactions.API.Application.DTOs;

namespace Transactions.API.Controllers;

/// <summary>
/// Controller responsável pela geração de relatórios e métricas de transações para o dashboard.
/// </summary>
[Authorize]
[ApiController]
[Route("api/reports")]
public class ReportController : ControllerBase
{
    private readonly IReportApplicationService _reportApplicationService;

    /// <summary>
    /// Construtor do controller de relatórios de transações.
    /// </summary>
    /// <param name="reportApplicationService">Serviço de aplicação para geração de relatórios de transações.</param>
    public ReportController(IReportApplicationService reportApplicationService)
    {
        _reportApplicationService = reportApplicationService;
    }

    /// <summary>
    /// Obtém o resumo mensal das transações.
    /// </summary>
    /// <param name="filter">Filtro de período.</param>
    /// <returns>Resumo mensal consolidado.</returns>
    [HttpGet("dashboard/summary")]
    public async Task<IActionResult> GetMonthlySummary([FromQuery] PeriodFilterDto filter)
    {
        var summary = await _reportApplicationService.GetMonthlySummaryAsync(filter);
        return Ok(summary);
    }

    /// <summary>
    /// Obtém o agrupamento de despesas por categoria.
    /// </summary>
    /// <param name="filter">Filtro de período.</param>
    /// <returns>Despesas agrupadas por categoria.</returns>
    [HttpGet("dashboard/expenses-by-category")]
    public async Task<IActionResult> GetExpensesByCategory([FromQuery] PeriodFilterDto filter)
    {
        var expenses = await _reportApplicationService.GetExpensesByCategoryAsync(filter);
        return Ok(expenses);
    }

    /// <summary>
    /// Obtém a evolução mensal de receitas e despesas.
    /// </summary>
    /// <param name="filter">Filtro de período.</param>
    /// <returns>Evolução mensal.</returns>
    [HttpGet("dashboard/monthly-evolution")]
    public async Task<IActionResult> GetMonthlyEvolution([FromQuery] PeriodFilterDto filter)
    {
        var evolution = await _reportApplicationService.GetMonthlyEvolutionAsync(filter);
        return Ok(evolution);
    }

    /// <summary>
    /// Obtém a evolução diária de receitas e despesas.
    /// </summary>
    /// <param name="filter">Filtro de período.</param>
    /// <returns>Evolução diária.</returns>
    [HttpGet("dashboard/daily-evolution")]
    public async Task<IActionResult> GetDailyEvolution([FromQuery] PeriodFilterDto filter)
    {
        var evolution = await _reportApplicationService.GetDailyEvolutionAsync(filter);
        return Ok(evolution);
    }

    /// <summary>
    /// Obtém as últimas transações realizadas no período.
    /// </summary>
    /// <param name="filter">Filtro de período.</param>
    /// <returns>Lista das últimas transações.</returns>
    [HttpGet("dashboard/latest-transactions")]
    public async Task<IActionResult> GetLatestTransactions([FromQuery] PeriodFilterDto filter)
    {
        var latest = await _reportApplicationService.GetLatestTransactionsAsync(filter);
        return Ok(latest);
    }
}
