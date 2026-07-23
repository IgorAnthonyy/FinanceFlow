using FinanceFlow.SharedKernel.Exceptions;
using Identity.API.Application.DTOs;
using Identity.API.Domain.Entities;
using Identity.API.Domain.Interfaces;

namespace Identity.API.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> CreateUser(UserCreate userCreate)
    {
        var existingUser = await _userRepository.GetByEmailAsync(userCreate.Email);
        if (existingUser != null)
            throw new ConflictException("Já existe um usuário cadastrado com este e-mail.");

        var hashedPassword = _passwordHasher.Hash(userCreate.Password);
        var user = new User(userCreate.Name, userCreate.Email, hashedPassword);

        return user;
    }

    public async Task<User> ValidateCredentials(UserLogin userLogin)
    {
        var user = await _userRepository.GetByEmailAsync(userLogin.Email);

        if (user is null || !_passwordHasher.Verify(userLogin.Password, user.PasswordHash))
            throw new UnauthorizedException("E-mail ou senha inválidos.");

        return user;
    }

    public async Task<User> UpdateUser(Guid userId, string name, string email)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (!email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _userRepository.GetByEmailAsync(email);
            if (existing != null)
                throw new InvalidOperationException("E-mail já está em uso.");
        }

        user.UpdateInfo(name, email);
        return user;
    }
}