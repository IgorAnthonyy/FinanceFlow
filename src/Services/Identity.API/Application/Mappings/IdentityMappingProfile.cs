using AutoMapper;
using DTO = Identity.API.Application.DTOs;
using Identity.API.Domain.Entities;
using FinanceFlow.Contracts.Events;

namespace Identity.API.Application.Mappings;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        CreateMap<User, DTO.UserCreate>().ReverseMap();

        CreateMap<User, UserCreatedIntegrationEvent>()
        .ReverseMap()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}
