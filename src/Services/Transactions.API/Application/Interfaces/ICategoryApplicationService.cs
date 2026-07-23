using System.Collections.Generic;
using System.Threading.Tasks;
using Transactions.API.Application.DTOs;

namespace Transactions.API.Application.Interfaces;

public interface ICategoryApplicationService
{
    Task<CategoryResponse> RegisterCategoryAsync(CategoryCreate request);
    Task<IEnumerable<CategoryResponse>> GetCategoriesAsync();
}
