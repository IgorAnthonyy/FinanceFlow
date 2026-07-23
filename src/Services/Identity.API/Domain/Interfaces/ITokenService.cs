using Identity.API.Application.DTOs;
using Identity.API.Domain.Entities;

namespace Identity.API.Domain.Interfaces;

public interface ITokenService
{
    AuthResponse Generate(User user);
}