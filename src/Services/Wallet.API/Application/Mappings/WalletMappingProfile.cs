using AutoMapper;
using Wallet.API.Application.DTOs;
using DO = Wallet.API.Domain.Entities;

namespace Wallet.API.Application.Mappings;

public class WalletMappingProfile : Profile
{
    public WalletMappingProfile()
    {
        CreateMap<BankAccountCreate, DO.BankAccount>();
        CreateMap<DO.BankAccount, BankAccountResponse>();
        CreateMap<DO.Wallet, WalletResponse>();
    }
}
