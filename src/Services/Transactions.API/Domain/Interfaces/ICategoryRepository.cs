using FinanceFlow.SharedKernel.Repositories;
using Transactions.API.Domain.Entities;

namespace Transactions.API.Domain.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IEnumerable<Category>> GetCategoriesAsync(Guid userId);
    Task<Category> GetByNameAsync(string name);
}
