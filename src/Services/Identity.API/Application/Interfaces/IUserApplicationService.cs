using Identity.API.Application.DTOs;

namespace Identity.API.Application.Interfaces;

public interface IUserApplicationService
{
    public Task CreateUserAsync(UserCreate userCreate);
    public Task<AuthResponse> Login(UserLogin userLogin);
    public Task UpdateUserAsync(UserUpdate userUpdate);
    public Task<AuthResponse> RenewTokenAsync();
}