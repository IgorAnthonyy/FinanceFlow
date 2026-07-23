using FinanceFlow.SharedKernel.Entities;

namespace FinanceFlow.SharedKernel.Repositories;
public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    Task SaveChangesAsync();
}