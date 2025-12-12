using PosGo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PosGo.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder) =>
        builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

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
    public DbSet<UserSystemRole> UserSystemRoles { get; set; }
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
}
