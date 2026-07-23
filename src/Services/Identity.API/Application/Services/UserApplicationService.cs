using AutoMapper;
using FinanceFlow.Contracts.Events;
using FinanceFlow.SharedKernel.Entities;
using FinanceFlow.SharedKernel.Exceptions;
using FinanceFlow.SharedKernel.Messaging;
using FinanceFlow.SharedKernel.Messaging.Abstractions;
using Identity.API.Application.DTOs;
using Identity.API.Application.Interfaces;
using Identity.API.Domain.Interfaces;
using Identity.API.Domain.Services;

namespace Identity.API.Application.Services;

public class UserApplicationService : IUserApplicationService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly UserData UserData;


    public UserApplicationService(
        IUserRepository userRepository,
        IUserService userService,
        ITokenService tokenService,
        IEventBus eventBus,
        IMapper mapper,
        UserData userData)
    {
        _userRepository = userRepository;
        _userService = userService;
        _tokenService = tokenService;
        _eventBus = eventBus;
        _mapper = mapper;
        UserData = userData;
    }

    public async Task CreateUserAsync(UserCreate userCreate)
    {
        var user = await _userService.CreateUser(userCreate);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var userCreatedEvent = _mapper.Map<UserCreatedIntegrationEvent>(user);
        await _eventBus.PublishAsync(userCreatedEvent, AppConstants.RabbitMq.RoutingKeys.UserCreated, AppConstants.RabbitMq.ExchangeName);
    }

    public async Task<AuthResponse> Login(UserLogin userLogin)
    {
        var user = await _userService.ValidateCredentials(userLogin);
        return _tokenService.Generate(user);
    }

    public async Task UpdateUserAsync(UserUpdate userUpdate)
    {
        var user = await _userService.UpdateUser(UserData.Id, userUpdate.Name, userUpdate.Email);

        await _userRepository.SaveChangesAsync();

        var userUpdatedEvent = new UserUpdatedIntegrationEvent(user.Id, user.Name, user.Email);
        await _eventBus.PublishAsync(userUpdatedEvent, AppConstants.RabbitMq.RoutingKeys.UserUpdated, AppConstants.RabbitMq.ExchangeName);
    }

    public async Task<AuthResponse> RenewTokenAsync()
    {
        var user = await _userRepository.GetByIdAsync(UserData.Id);
        if (user == null)
            throw new UnauthorizedException("Usuário não encontrado.");

        return _tokenService.Generate(user);
    }
}