using AutoMapper;
using PosGo.Application.UserCases.V1.Queries.Menu;
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
        CreateMap<User, Contract.Services.V1.Account.Response.UpdateProfileResponse>().ReverseMap();

        CreateMap<User, Contract.Services.V1.User.Response.UserResponse>().ReverseMap();
        CreateMap<User, Contract.Services.V1.User.Response.UpdateUserResponse>().ReverseMap();
        CreateMap<PagedResult<User>, PagedResult<Contract.Services.V1.User.Response.UserResponse>>().ReverseMap();

        CreateMap<Restaurant, Contract.Services.V1.Restaurant.Response.RestaurantResponse>().ReverseMap();
        CreateMap<PagedResult<Restaurant>, PagedResult<Contract.Services.V1.Restaurant.Response.RestaurantResponse>>().ReverseMap();
        CreateMap<Restaurant, Contract.Services.V1.Restaurant.Response.MyRestaurantResponse>().ReverseMap();

        CreateMap<Function, GetMenuByUserResponse>().ReverseMap();

        CreateMap<Role, Contract.Services.V1.Role.Response.RoleResponse>().ReverseMap();

        CreateMap<Plan, Contract.Services.V1.Plan.Response.PlanResponse>().ReverseMap();
        CreateMap<PagedResult<Plan>, PagedResult<Contract.Services.V1.Plan.Response.PlanResponse>>().ReverseMap();
    }
}
