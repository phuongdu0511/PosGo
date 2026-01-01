using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;

namespace PosGo.Persistence.Constants;

public static class TableNames
{
    public const string Product = nameof(Product);
    public const string RestaurantGroups = nameof(RestaurantGroups);
    public const string Languages = nameof(Languages); 
    public const string Restaurants = nameof(Restaurants); 
    public const string RestaurantLanguages = nameof(RestaurantLanguages); 
    public const string CodeSets = nameof(CodeSets); 
    public const string CodeItems = nameof(CodeItems); 
    public const string CodeItemTranslations = nameof(CodeItemTranslations); 
    public const string Users = nameof(Users); 
    public const string Roles = nameof(Roles); 
    public const string UserSystemRoles = nameof(UserSystemRoles); 
    public const string RestaurantUsers = nameof(RestaurantUsers); 
    public const string TableAreas = nameof(TableAreas); 
    public const string Tables = nameof(Tables); 
    public const string Units = nameof(Units); 
    public const string UnitTranslations = nameof(UnitTranslations); 
    public const string DishCategories = nameof(DishCategories); 
    public const string DishCategoryTranslations = nameof(DishCategoryTranslations); 
    public const string Dishes = nameof(Dishes); 
    public const string DishTranslations = nameof(DishTranslations); 
    public const string DishVariants = nameof(DishVariants); 
    public const string DishVariantTranslations = nameof(DishVariantTranslations); 
    public const string DishVariantOptions = nameof(DishVariantOptions); 
    public const string DishVariantOptionTranslations = nameof(DishVariantOptionTranslations); 
    public const string DishSkus = nameof(DishSkus); 
    public const string DishSkuVariantOptions = nameof(DishSkuVariantOptions); 
    public const string DishAttributeGroups = nameof(DishAttributeGroups); 
    public const string DishAttributeGroupTranslations = nameof(DishAttributeGroupTranslations); 
    public const string DishAttributeItems = nameof(DishAttributeItems); 
    public const string DishAttributeItemTranslations = nameof(DishAttributeItemTranslations); 
    public const string Orders = nameof(Orders); 
    public const string OrderItems = nameof(OrderItems); 
    public const string OrderItemAttributes = nameof(OrderItemAttributes); 
    public const string RestaurantOpeningHours = nameof(RestaurantOpeningHours);
    public const string Functions = nameof(Functions);
    public const string UserRoles = nameof(UserRoles);
    public const string UserClaims = nameof(UserClaims); // IdentityUserClaim
    public const string RoleClaims = nameof(RoleClaims); // IdentityRoleClaim
    public const string UserLogins = nameof(UserLogins); // IdentityRoleClaim
    public const string UserTokens = nameof(UserTokens); // IdentityUserToken
    public const string Plans = nameof(Plans);
    public const string RestaurantPlans = nameof(RestaurantPlans);
    public const string PlanFunctions = nameof(PlanFunctions);
}
