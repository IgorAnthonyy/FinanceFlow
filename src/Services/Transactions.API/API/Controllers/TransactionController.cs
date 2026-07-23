using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.API.Application.DTOs;
using Transactions.API.Application.Interfaces;

namespace Transactions.API.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de transações financeiras.
/// </summary>
[Authorize]
[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionApplicationService _transactionApplicationService;

    /// <summary>
    /// Construtor do controller de transações.
    /// </summary>
    /// <param name="transactionApplicationService">Serviço de aplicação para gestão de transações.</param>
    public TransactionController(ITransactionApplicationService transactionApplicationService)
    {
        _transactionApplicationService = transactionApplicationService;
    }

    /// <summary>
    /// Registra uma nova transação financeira.
    /// </summary>
    /// <param name="request">Dados para criação da transação.</param>
    /// <returns>A transação criada.</returns>
    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] TransactionCreate request)
    {
        var result = await _transactionApplicationService.RegisterTransactionAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Obtém todas as transações associadas a um usuário.
    /// </summary>
    /// <param name="userId">Identificador único do usuário.</param>
    /// <returns>Lista de transações do usuário.</returns>
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
    {
        var list = await _transactionApplicationService.GetTransactionsByUserIdAsync(userId);
        return Ok(list);
    }

    /// <summary>
    /// Obtém transações com base em filtros avançados.
    /// </summary>
    /// <param name="request">Filtros para a consulta de transações.</param>
    /// <returns>Lista de transações filtradas.</returns>
    [HttpPost("filters")]
    public async Task<IActionResult> GetFilteredTransactions([FromBody] TransactionFilterRequest request)
    {
        var list = await _transactionApplicationService.GetFilteredTransactionsAsync(request);
        return Ok(list);
    }
}
