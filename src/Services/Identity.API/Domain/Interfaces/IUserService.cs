using Identity.API.Application.DTOs;
using Identity.API.Domain.Entities;

namespace Identity.API.Domain.Interfaces;

public interface IUserService
{
    Task<User> CreateUser(UserCreate userCreate);
    Task<User> ValidateCredentials(UserLogin userLogin);
    Task<User> UpdateUser(Guid userId, string name, string email);
}