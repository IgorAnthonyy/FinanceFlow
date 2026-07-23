using System.Threading.Tasks;
using Wallet.API.Application.DTOs;

namespace Wallet.API.Application.Interfaces;

public interface IReportApplicationService
{
    Task<WalletReportResponse> GetWalletReportAsync();
}
