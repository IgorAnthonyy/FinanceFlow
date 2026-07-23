using System;
using System.Text.Json;
using System.Threading.Tasks;
using FinanceFlow.Contracts.Events;
using FinanceFlow.SharedKernel.Messaging;
using FinanceFlow.SharedKernel.Messaging.Abstractions;
using Notifications.Worker.Application.Models;
using Notifications.Worker.Domain.Entities;
using Notifications.Worker.Domain.Interfaces;

namespace Notifications.Worker.Application.Services;

public class NotificationApplicationService : IMessageBrokerClientApplicationService
{
    private readonly INotificationsLogRepository _logRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IEmailService _emailService;

    public NotificationApplicationService(
        INotificationsLogRepository logRepository,
        IUserRepository userRepository,
        ITemplateRenderer templateRenderer,
        IEmailService emailService)
    {
        _logRepository = logRepository;
        _userRepository = userRepository;
        _templateRenderer = templateRenderer;
        _emailService = emailService;
    }

    public async Task<bool> ProcessMessage(string routingKey, string message)
    {
        switch (routingKey)
        {
            case AppConstants.RabbitMq.RoutingKeys.UserCreated:
                return await HandleUserCreatedAsync(message);

            case AppConstants.RabbitMq.RoutingKeys.TransactionCreated:
                return await HandleTransactionCreatedAsync(message);

            case AppConstants.RabbitMq.RoutingKeys.UserUpdated:
                return await HandleUserUpdatedAsync(message);

            default:
                return false;
        }
    }

    private async Task<bool> HandleUserCreatedAsync(string message)
    {
        var userCreated = JsonSerializer.Deserialize<UserCreatedIntegrationEvent>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (userCreated == null)
            return false;

        var welcomeModel = new
        {
            Name = userCreated.Name,
            Email = userCreated.Email
        };

        var htmlBody = await _templateRenderer.RenderAsync("Welcome", welcomeModel);
        await _emailService.SendEmailAsync(userCreated.Email, "Bem-vindo ao FinanceFlow!", htmlBody);

        var user = new User(userCreated.Id, userCreated.Name, userCreated.Email);
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var log = new NotificationsLog(userCreated.Id, "UserCreated", userCreated.Email);
        await _logRepository.AddAsync(log);
        await _logRepository.SaveChangesAsync();

        return true;
    }

    private async Task<bool> HandleTransactionCreatedAsync(string message)
    {
        var txCreated = JsonSerializer.Deserialize<TransactionCreatedIntegrationEvent>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (txCreated == null)
            return false;

        var (name, email) = await _userRepository.GetUserInfoAsync(txCreated.UserId);
        if (string.IsNullOrEmpty(email))
            return false;

        var userModel = new UserEmailModel(name, email);
        var txModel = new TransactionEmailModel(txCreated.Amount, txCreated.Type, txCreated.BankAccountId, txCreated.CreatedAt);
        var viewModel = new TransactionNotificationViewModel(userModel, txModel);

        var htmlBody = await _templateRenderer.RenderAsync("TransactionNotification", viewModel);
        var subject = $"Alerta de Transação FinanceFlow: {txCreated.Type} no valor de {txModel.FormattedAmount}";

        await _emailService.SendEmailAsync(email, subject, htmlBody);

        var log = new NotificationsLog(txCreated.UserId, "TransactionCreated", email);
        await _logRepository.AddAsync(log);
        await _logRepository.SaveChangesAsync();

        return true;
    }

    private async Task<bool> HandleUserUpdatedAsync(string message)
    {
        var userUpdated = JsonSerializer.Deserialize<UserUpdatedIntegrationEvent>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (userUpdated == null)
            return false;

        var user = await _userRepository.GetByIdAsync(userUpdated.Id, true);
        if (user == null)
            return false;

        user.Update(userUpdated.Name, userUpdated.Email);
        await _userRepository.SaveChangesAsync();

        var log = new NotificationsLog(userUpdated.Id, "UserUpdated", user.Email);
        await _logRepository.AddAsync(log);
        await _logRepository.SaveChangesAsync();

        return true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
