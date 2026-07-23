using FinanceFlow.SharedKernel.Repositories;
using Identity.API.Domain.Entities;

namespace Identity.API.Domain.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    public Task<User> GetByEmailAsync(string email);
}