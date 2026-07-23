using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceFlow.SharedKernel.Applications;
using FinanceFlow.SharedKernel.Entities;
using Wallet.API.Application.DTOs;
using Wallet.API.Application.Interfaces;
using Wallet.API.Domain.Interfaces;

namespace Wallet.API.Application.Services;

public class ReportApplicationService : BaseApplication, IReportApplicationService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IBankAccountRepository _bankAccountRepository;

    public ReportApplicationService(
        IWalletRepository walletRepository,
        IBankAccountRepository bankAccountRepository,
        UserData userData) : base(userData)
    {
        _walletRepository = walletRepository;
        _bankAccountRepository = bankAccountRepository;
    }

    public async Task<WalletReportResponse> GetWalletReportAsync()
    {
        var wallet = await _walletRepository.GetByUserIdAsync(UserData.Id);
        var accounts = (await _bankAccountRepository.GetByUserIdAsync(UserData.Id)).ToList();

        var totalBalance = accounts.Sum(a => a.Balance);
        var distribution = new List<AccountBalanceSummary>();

        foreach (var acc in accounts)
        {
            var percentage = totalBalance > 0 ? (double)(acc.Balance / totalBalance) * 100.0 : 0.0;
            distribution.Add(new AccountBalanceSummary
            {
                BankAccountId = acc.Id,
                BankName = acc.BankName,
                AccountName = acc.AccountName,
                Balance = acc.Balance,
                Percentage = Math.Round(percentage, 2)
            });
        }

        return new WalletReportResponse
        {
            TotalBalance = totalBalance,
            AccountsDistribution = distribution.OrderByDescending(d => d.Balance).ToList()
        };
    }
}
