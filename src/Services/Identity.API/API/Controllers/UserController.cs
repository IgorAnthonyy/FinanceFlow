using FinanceFlow.SharedKernel.Entities;
using Identity.API.Application.DTOs;
using Identity.API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de usuários e autenticação.
/// </summary>
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserApplicationService _userApplicationService;

    /// <summary>
    /// Construtor do controller de usuário.
    /// </summary>
    /// <param name="userApplicationService">Serviço de aplicação para gestão de usuários.</param>
    public UserController(IUserApplicationService userApplicationService)
    {
        _userApplicationService = userApplicationService;
    }

    /// <summary>
    /// Cria um novo usuário no sistema.
    /// </summary>
    /// <param name="userCreate">Dados para criação do usuário.</param>
    /// <returns>Resultado da operação.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreate userCreate)
    {
        await _userApplicationService.CreateUserAsync(userCreate);
        return Ok();
    }

    /// <summary>
    /// Realiza a autenticação (login) do usuário.
    /// </summary>
    /// <param name="userLogin">Credenciais do usuário.</param>
    /// <returns>Token de autenticação JWT.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        var token = await _userApplicationService.Login(userLogin);
        return Ok(new { token });
    }

    /// <summary>
    /// Atualiza o perfil do usuário autenticado.
    /// </summary>
    /// <param name="userUpdate">Novos dados do perfil do usuário.</param>
    /// <returns>Resultado da operação.</returns>
    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserUpdate userUpdate)
    {
        await _userApplicationService.UpdateUserAsync(userUpdate);
        return Ok();
    }

    /// <summary>
    /// Renova o token de acesso do usuário autenticado.
    /// </summary>
    /// <returns>Novo token de autenticação e detalhes da renovação.</returns>
    [Authorize]
    [HttpPost("renew")]
    public async Task<IActionResult> RenewToken()
    {
        var authResponse = await _userApplicationService.RenewTokenAsync();
        return Ok(authResponse);
    }
}