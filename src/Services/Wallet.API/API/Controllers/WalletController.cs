using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.API.Application.Interfaces;

namespace Wallet.API.Controllers;

/// <summary>
/// Controller responsável pelas operações de carteira.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletApplicationService _walletApplicationService;

    /// <summary>
    /// Construtor do controller de carteira.
    /// </summary>
    /// <param name="walletApplicationService">Serviço de aplicação para gestão de carteira.</param>
    public WalletController(IWalletApplicationService walletApplicationService)
    {
        _walletApplicationService = walletApplicationService;
    }

    /// <summary>
    /// Obtém os dados da carteira do usuário informado.
    /// </summary>
    /// <param name="userId">Identificador único do usuário.</param>
    /// <returns>Os dados da carteira do usuário.</returns>
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
    {
        var wallet = await _walletApplicationService.GetWalletByUserIdAsync(userId);
        if (wallet == null)
            return NotFound("Wallet not found for this user.");

        return Ok(wallet);
    }
}
