using PosGo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Abstractions.Entities;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PosGo.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder) 
    {
        // Global filter for soft delete pattern
        var softDeleteEntities = typeof(ISoftDeletableEntity).Assembly.GetTypes()
                .Where(type => typeof(ISoftDeletableEntity)
                                .IsAssignableFrom(type)
                                && type.IsClass
                                && !type.IsAbstract);

        foreach (var softDeleteEntity in softDeleteEntities)
        {
            builder.Entity(softDeleteEntity).HasQueryFilter(GenerateQueryFilterLambda(softDeleteEntity));
        }

        builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }

    private LambdaExpression? GenerateQueryFilterLambda(Type type)
    {
        // parameter: w =>
        var parameter = Expression.Parameter(type, "w");
        // falseConstantValue: false
        var falseConstantValue = Expression.Constant(false);
        // propertyAccess: w.IsDeleted
        var propertyAccess = Expression.PropertyOrField(parameter, nameof(ISoftDeletableEntity.IsDeleted));
        // equalExpression: w.IsDeleted == false
        var equalExpression = Expression.Equal(propertyAccess, falseConstantValue);
        // lambda: w => w.IsDeleted == false
        var lambda = Expression.Lambda(equalExpression, parameter);

        return lambda; // w => w.IsDeleted == false
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<RestaurantGroup> RestaurantGroups { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<RestaurantLanguage> RestaurantLanguages { get; set; }
    public DbSet<CodeSet> CodeSets { get; set; }
    public DbSet<CodeItem> CodeItems { get; set; }
    public DbSet<CodeItemTranslation> CodeItemTranslations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RestaurantUser> RestaurantUsers { get; set; }
    public DbSet<TableArea> TableAreas { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<UnitTranslation> UnitTranslations { get; set; }
    public DbSet<DishCategory> DishCategories { get; set; }
    public DbSet<DishCategoryTranslation> DishCategoryTranslations { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<DishTranslation> DishTranslations { get; set; }
    public DbSet<DishVariant> DishVariants { get; set; }
    public DbSet<DishVariantTranslation> DishVariantTranslations { get; set; }
    public DbSet<DishVariantOption> DishVariantOptions { get; set; }
    public DbSet<DishVariantOptionTranslation> DishVariantOptionTranslations { get; set; }
    public DbSet<DishSku> DishSkus { get; set; }
    public DbSet<DishSkuVariantOption> DishSkuVariantOptions { get; set; }
    public DbSet<DishAttributeGroup> DishAttributeGroups { get; set; }
    public DbSet<DishAttributeGroupTranslation> DishAttributeGroupTranslations { get; set; }
    public DbSet<DishAttributeItem> DishAttributeItems { get; set; }
    public DbSet<DishAttributeItemTranslation> DishAttributeItemTranslations { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderItemAttribute> OrderItemAttributes { get; set; }
    public DbSet<RestaurantOpeningHour> RestaurantOpeningHours { get; set; }
    public DbSet<Function> Functions { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<RestaurantPlan> RestaurantPlans { get; set; }
    public DbSet<PlanFunction> PlanFunctions { get; set; }
}
