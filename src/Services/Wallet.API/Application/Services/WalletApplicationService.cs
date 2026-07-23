using System.Text.Json;
using AutoMapper;
using FinanceFlow.Contracts.Events;
using FinanceFlow.SharedKernel.Applications;
using FinanceFlow.SharedKernel.Entities;
using FinanceFlow.SharedKernel.Exceptions;
using FinanceFlow.SharedKernel.Messaging;
using FinanceFlow.SharedKernel.Messaging.Abstractions;
using Wallet.API.Application.DTOs;
using Wallet.API.Application.Interfaces;
using Wallet.API.Domain.Enums;
using Wallet.API.Domain.Interfaces;
using DO = Wallet.API.Domain.Entities;

namespace Wallet.API.Application.Services;

public class WalletApplicationService : BaseApplication, IWalletApplicationService, IMessageBrokerClientApplicationService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IWalletService _walletService;
    private readonly IMapper _mapper;

    public WalletApplicationService(
        IWalletRepository walletRepository,
        IBankAccountRepository bankAccountRepository,
        IWalletService walletService,
        IMapper mapper,
        UserData userData) : base(userData)
    {
        _walletRepository = walletRepository;
        _bankAccountRepository = bankAccountRepository;
        _walletService = walletService;
        _mapper = mapper;
    }

    public async Task CreateWalletAsync(Guid userId)
    {
        var existing = await _walletRepository.GetByUserIdAsync(userId);
        if (existing != null)
            return;

        var wallet = new DO.Wallet(userId);
        await _walletRepository.AddAsync(wallet);
        await _walletRepository.SaveChangesAsync();
    }

    public async Task<BankAccountResponse> CreateBankAccountAsync(BankAccountCreate request)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId);
        if (wallet == null)
            throw new ConflictException("Carteira não encontrada para o usuário especificado.");

        var account = _mapper.Map<DO.BankAccount>(request);
        account.WalletId = wallet.Id;

        account.PrepareInsert();

        await _bankAccountRepository.AddAsync(account);
        await _bankAccountRepository.SaveChangesAsync();

        var accounts = await _bankAccountRepository.GetByUserIdAsync(request.UserId);
        _walletService.RecalculateWallet(wallet, accounts);
        await _walletRepository.SaveChangesAsync();

        return _mapper.Map<BankAccountResponse>(account);
    }

    public async Task<WalletResponse> GetWalletByUserIdAsync(Guid userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
            return null;

        return _mapper.Map<WalletResponse>(wallet);
    }

    public async Task<IEnumerable<BankAccountResponse>> GetBankAccountsByUserIdAsync(Guid userId)
    {
        var accounts = await _bankAccountRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<BankAccountResponse>>(accounts);
    }

    public async Task ProcessTransactionEventAsync(TransactionCreatedIntegrationEvent transactionCreatedIntegrationEvent)
    {
        if (!EnumExtensions.TryParseFromDescription<TransactionType>(transactionCreatedIntegrationEvent.Type, out var transactionType))
            return;

        var account = await _bankAccountRepository.GetByIdAsync(transactionCreatedIntegrationEvent.BankAccountId);
        if (account == null)
            return;

        var wallet = await _walletRepository.GetByUserIdAsync(transactionCreatedIntegrationEvent.UserId);
        if (wallet == null)
            return;

        _walletService.ProcessTransaction(wallet, account, transactionCreatedIntegrationEvent.Amount, transactionType);


        var accounts = await _bankAccountRepository.GetByUserIdAsync(transactionCreatedIntegrationEvent.UserId);
        _walletService.RecalculateWallet(wallet, accounts);

        await _bankAccountRepository.SaveChangesAsync();
        await _walletRepository.SaveChangesAsync();
    }

    public async Task<bool> ProcessMessage(string routingKey, string message)
    {
        switch (routingKey)
        {
            case AppConstants.RabbitMq.RoutingKeys.UserCreated:
                var userCreated = JsonSerializer.Deserialize<UserCreatedIntegrationEvent>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (userCreated != null)
                    await CreateWalletAsync(userCreated.Id);

                break;

            case AppConstants.RabbitMq.RoutingKeys.TransactionCreated:
                var txCreated = JsonSerializer.Deserialize<TransactionCreatedIntegrationEvent>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (txCreated != null)
                    await ProcessTransactionEventAsync(txCreated);

                break;

            default:
                return false;
        }
        return true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
