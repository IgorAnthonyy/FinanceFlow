using System.Collections.Generic;

namespace Wallet.API.Application.DTOs;

public class WalletReportResponse
{
    public decimal TotalBalance { get; set; }
    public List<AccountBalanceSummary> AccountsDistribution { get; set; } = new();
}
