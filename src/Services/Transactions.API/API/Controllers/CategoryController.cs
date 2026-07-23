using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.API.Application.DTOs;
using Transactions.API.Application.Interfaces;

namespace Transactions.API.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de categorias de transações.
/// </summary>
[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryApplicationService _categoryApplicationService;

    /// <summary>
    /// Construtor do controller de categorias.
    /// </summary>
    /// <param name="categoryApplicationService">Serviço de aplicação para gestão de categorias.</param>
    public CategoryController(ICategoryApplicationService categoryApplicationService)
    {
        _categoryApplicationService = categoryApplicationService;
    }

    /// <summary>
    /// Obtém todas as categorias cadastradas.
    /// </summary>
    /// <returns>Lista de categorias.</returns>
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryApplicationService.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Cadastra uma nova categoria de transação.
    /// </summary>
    /// <param name="request">Dados para criação da categoria.</param>
    /// <returns>A categoria criada.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreate request)
    {
        var result = await _categoryApplicationService.RegisterCategoryAsync(request);
        return Ok(result);
    }
}