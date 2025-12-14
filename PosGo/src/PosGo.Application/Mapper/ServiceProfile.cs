using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Domain.Entities;

namespace PosGo.Application.Mapper;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<Product, Contract.Services.V1.Product.Response.ProductResponse>().ReverseMap();
        CreateMap<PagedResult<Product>, PagedResult<Contract.Services.V1.Product.Response.ProductResponse>>().ReverseMap();

        CreateMap<User, Contract.Services.V1.Account.Response.AccountResponse>().ReverseMap();
        CreateMap<PagedResult<User>, PagedResult<Contract.Services.V1.Account.Response.AccountResponse>>().ReverseMap();
    }
}
