using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.API.Application.Interfaces;

namespace Wallet.API.Controllers;

/// <summary>
/// Controller responsável pela geração de relatórios de carteira.
/// </summary>
[Authorize]
[ApiController]
[Route("api/reports")]
public class ReportController : ControllerBase
{
    private readonly IReportApplicationService _reportApplicationService;

    /// <summary>
    /// Construtor do controller de relatórios.
    /// </summary>
    /// <param name="reportApplicationService">Serviço de aplicação para geração de relatórios.</param>
    public ReportController(IReportApplicationService reportApplicationService)
    {
        _reportApplicationService = reportApplicationService;
    }

    /// <summary>
    /// Obtém o relatório resumido da carteira do usuário autenticado.
    /// </summary>
    /// <returns>Dados do relatório da carteira.</returns>
    [HttpGet("wallet")]
    public async Task<IActionResult> GetWalletReport()
    {
        var report = await _reportApplicationService.GetWalletReportAsync();
        return Ok(report);
    }
}
