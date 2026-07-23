using AutoMapper;
using Transactions.API.Application.DTOs;
using Transactions.API.Domain.Filters;
using DO = Transactions.API.Domain.Entities;

namespace Transactions.API.Application.Mappings;

public class TransactionsMappingProfile : Profile
{
    public TransactionsMappingProfile()

    {
        CreateMap<TransactionCreate, DO.Transaction>();
        CreateMap<DO.Transaction, TransactionResponse>();
        CreateMap<CategoryCreate, DO.Category>();
        CreateMap<DO.Category, CategoryResponse>();
        CreateMap<TransactionFilterRequest, TransactionFilter>();
    }
}
