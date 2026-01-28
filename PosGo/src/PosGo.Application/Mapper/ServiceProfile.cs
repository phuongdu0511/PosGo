using AutoMapper;
using PosGo.Application.UserCases.V1.Queries.Menu;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Domain.Entities;
using static PosGo.Contract.Services.V1.DishCategory.Response;

namespace PosGo.Application.Mapper;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
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

        CreateMap<User, Contract.Services.V1.Employee.Response.StaffResponse>().ReverseMap();
        CreateMap<PagedResult<User>, PagedResult<Contract.Services.V1.Employee.Response.StaffResponse>>().ReverseMap();

        CreateMap<TableArea, Contract.Services.V1.Table.Response.TableAreaResponse>().ReverseMap();
        CreateMap<PagedResult<TableArea>, PagedResult<Contract.Services.V1.Table.Response.TableAreaResponse>>().ReverseMap();

        CreateMap<Domain.Entities.Table, Contract.Services.V1.Table.Response.TableResponse>().ReverseMap();
        CreateMap<PagedResult<Domain.Entities.Table>, PagedResult<Contract.Services.V1.Table.Response.TableResponse>>().ReverseMap();

        CreateMap<Language, Contract.Services.V1.Language.Response.LanguageResponse>().ReverseMap();
        CreateMap<PagedResult<Language>, PagedResult<Contract.Services.V1.Language.Response.LanguageResponse>>().ReverseMap();
        CreateMap<Language, Contract.Services.V1.Language.Response.LanguageDetailResponse>().ReverseMap();

        CreateMap<Domain.Entities.Dish, Contract.Services.V1.Dish.Response.DishResponse>()
            .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));

        CreateMap<Domain.Entities.Dish, Contract.Services.V1.Dish.Response.DishDetailResponse>()
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => 
                src.Category != null && src.Category.Translations.Any() 
                    ? src.Category.Translations.First().Name 
                    : null))
            .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => 
                src.Unit != null && src.Unit.Translations.Any() 
                    ? src.Unit.Translations.First().Name 
                    : null))
            .ForMember(dest => dest.DishTypeName, opt => opt.MapFrom(src => 
                src.DishType != null && src.DishType.Translations.Any() 
                    ? src.DishType.Translations.First().Name 
                    : null))
            .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));

        CreateMap<PagedResult<Domain.Entities.Dish>, PagedResult<Contract.Services.V1.Dish.Response.DishResponse>>().ReverseMap();

        CreateMap<DishTranslation, Contract.Services.V1.Dish.Response.DishTranslationResponse>();

        CreateMap<DishVariant, Contract.Services.V1.Dish.Response.DishVariantResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => 
                src.Translations.Any() ? src.Translations.First().Name : src.Code));

        CreateMap<DishVariantOption, Contract.Services.V1.Dish.Response.DishVariantOptionResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => 
                src.Translations.Any() ? src.Translations.First().Name : src.Code));

        CreateMap<DishSku, Contract.Services.V1.Dish.Response.DishSkuResponse>();

        // DishCategory mappings
        CreateMap<DishCategory, Contract.Services.V1.DishCategory.Response.DishCategoryResponse>()
            .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));

        CreateMap<PagedResult<DishCategory>, PagedResult<Contract.Services.V1.DishCategory.Response.DishCategoryResponse>>().ReverseMap();

        CreateMap<DishCategoryTranslation, Contract.Services.V1.DishCategory.Response.DishCategoryTranslationResponse>()
            .ForCtorParam(nameof(DishCategoryTranslationResponse.Language),
        opt => opt.MapFrom(src => src.Language.Code));

        // DishVariant mappings
        CreateMap<DishVariant, Contract.Services.V1.DishVariant.Response.DishVariantResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => 
                src.Translations.Any() ? src.Translations.First().Name : src.Code));

        CreateMap<DishVariantOption, Contract.Services.V1.DishVariant.Response.VariantOptionResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => 
                src.Translations.Any() ? src.Translations.First().Name : src.Code));

        CreateMap<DishVariantTranslation, Contract.Services.V1.DishVariant.Response.DishVariantTranslationResponse>()
            .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.Code))
            .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.Name));

        CreateMap<DishVariantOptionTranslation, Contract.Services.V1.DishVariant.Response.DishVariantOptionTranslationResponse>()
            .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.Code))
            .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.Name));

        // DishSku mappings
        CreateMap<DishSku, Contract.Services.V1.DishSku.Response.DishSkuResponse>()
            .ForMember(dest => dest.DishName, opt => opt.MapFrom(src => 
                src.Dish.Translations.Any() ? src.Dish.Translations.First().Name : ""));

        // DishCombo mappings
        CreateMap<DishComboItem, Contract.Services.V1.DishCombo.Response.ComboItemResponse>();
    }
}
