using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;

namespace PosGo.Persistence.Constants;

internal static class TableNames
{
    internal const string Product = nameof(Product);
    internal const string RestaurantGroups = nameof(RestaurantGroups);
    internal const string Languages = nameof(Languages); 
    internal const string Restaurants = nameof(Restaurants); 
    internal const string RestaurantLanguages = nameof(RestaurantLanguages); 
    internal const string CodeSets = nameof(CodeSets); 
    internal const string CodeItems = nameof(CodeItems); 
    internal const string CodeItemTranslations = nameof(CodeItemTranslations); 
    internal const string Users = nameof(Users); 
    internal const string Roles = nameof(Roles); 
    internal const string UserSystemRoles = nameof(UserSystemRoles); 
    internal const string RestaurantUsers = nameof(RestaurantUsers); 
    internal const string TableAreas = nameof(TableAreas); 
    internal const string Tables = nameof(Tables); 
    internal const string Units = nameof(Units); 
    internal const string UnitTranslations = nameof(UnitTranslations); 
    internal const string DishCategories = nameof(DishCategories); 
    internal const string DishCategoryTranslations = nameof(DishCategoryTranslations); 
    internal const string Dishes = nameof(Dishes); 
    internal const string DishTranslations = nameof(DishTranslations); 
    internal const string DishVariants = nameof(DishVariants); 
    internal const string DishVariantTranslations = nameof(DishVariantTranslations); 
    internal const string DishVariantOptions = nameof(DishVariantOptions); 
    internal const string DishVariantOptionTranslations = nameof(DishVariantOptionTranslations); 
    internal const string DishSkus = nameof(DishSkus); 
    internal const string DishSkuVariantOptions = nameof(DishSkuVariantOptions); 
    internal const string DishAttributeGroups = nameof(DishAttributeGroups); 
    internal const string DishAttributeGroupTranslations = nameof(DishAttributeGroupTranslations); 
    internal const string DishAttributeItems = nameof(DishAttributeItems); 
    internal const string DishAttributeItemTranslations = nameof(DishAttributeItemTranslations); 
    internal const string Orders = nameof(Orders); 
    internal const string OrderItems = nameof(OrderItems); 
    internal const string OrderItemAttributes = nameof(OrderItemAttributes); 
    internal const string RestaurantOpeningHours = nameof(RestaurantOpeningHours); 
}
