using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.API.Application.DTOs;
using Wallet.API.Application.Interfaces;

namespace Wallet.API.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de contas bancárias.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BankAccountController : ControllerBase
{
    private readonly IWalletApplicationService _walletApplicationService;

    /// <summary>
    /// Construtor do controller de conta bancária.
    /// </summary>
    /// <param name="walletApplicationService">Serviço de aplicação para gestão de carteira e contas.</param>
    public BankAccountController(IWalletApplicationService walletApplicationService)
    {
        _walletApplicationService = walletApplicationService;
    }

    /// <summary>
    /// Cria uma nova conta bancária.
    /// </summary>
    /// <param name="request">Dados para criação da conta bancária.</param>
    /// <returns>A conta bancária criada.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BankAccountCreate request)
    {
        var result = await _walletApplicationService.CreateBankAccountAsync(request);
        return CreatedAtAction(nameof(GetByUserId), new { userId = result.UserId }, result);
    }

    /// <summary>
    /// Obtém as contas bancárias associadas a um determinado usuário.
    /// </summary>
    /// <param name="userId">Identificador único do usuário.</param>
    /// <returns>Lista de contas bancárias do usuário.</returns>
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
    {
        var accounts = await _walletApplicationService.GetBankAccountsByUserIdAsync(userId);
        return Ok(accounts);
    }
}
