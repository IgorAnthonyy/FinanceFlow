using AutoMapper;
using FinanceFlow.Contracts.Events;
using FinanceFlow.SharedKernel.Applications;
using FinanceFlow.SharedKernel.Entities;
using FinanceFlow.SharedKernel.Messaging;
using FinanceFlow.SharedKernel.Messaging.Abstractions;
using Transactions.API.Application.DTOs;
using Transactions.API.Application.Interfaces;
using Transactions.API.Domain.Enums;
using Transactions.API.Domain.Filters;
using Transactions.API.Domain.Interfaces;

namespace Transactions.API.Application.Services;

public class TransactionApplicationService : BaseApplication, ITransactionApplicationService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ITransactionService _transactionService;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;

    public TransactionApplicationService(
        ITransactionRepository transactionRepository,
        ITransactionService transactionService,
        IEventBus eventBus,
        IMapper mapper,
        UserData userData) : base(userData)
    {
        _transactionRepository = transactionRepository;
        _transactionService = transactionService;
        _eventBus = eventBus;
        _mapper = mapper;
    }

    public async Task<TransactionResponse> RegisterTransactionAsync(TransactionCreate request)
    {
        var transaction = _transactionService.CreateTransaction(request);

        await _transactionRepository.AddAsync(transaction);
        await _transactionRepository.SaveChangesAsync();

        var transactionEvent = new TransactionCreatedIntegrationEvent(
            transaction.UserId,
            transaction.Amount,
            transaction.Type.GetDescription(),
            transaction.BankAccountId,
            transaction.CreatedAt
        );

        await _eventBus.PublishAsync(transactionEvent, AppConstants.RabbitMq.RoutingKeys.TransactionCreated, AppConstants.RabbitMq.ExchangeName);

        return _mapper.Map<TransactionResponse>(transaction);
    }

    public async Task<IEnumerable<TransactionResponse>> GetTransactionsByUserIdAsync(Guid userId)
    {
        var list = await _transactionRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<TransactionResponse>>(list);
    }

    public async Task<IEnumerable<TransactionResponse>> GetTransactionsByBankAccountIdAsync(Guid bankAccountId)
    {
        var list = await _transactionRepository.GetByBankAccountIdAsync(bankAccountId);
        return _mapper.Map<IEnumerable<TransactionResponse>>(list);
    }

    public async Task<PagedResult<TransactionResponse>> GetFilteredTransactionsAsync(TransactionFilterRequest request)
    {
        var filter = _mapper.Map<TransactionFilter>(request);
        var (items, totalCount) = await _transactionRepository.GetFilteredTransactionsAsync(filter);
        var mappedItems = _mapper.Map<List<TransactionResponse>>(items);
        return new PagedResult<TransactionResponse>(mappedItems, request.PageNumber, request.PageSize, totalCount);
    }
}
