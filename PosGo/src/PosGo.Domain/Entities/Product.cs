using PosGo.Domain.Abstractions.Aggregates;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class Product : AggregateRoot<Guid>, IAuditableEntity, ISoftDeletableEntity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }

    public static Product CreateProduct(Guid id, string name, decimal price, string description)
    {
        var product = new Product(id, name, price, description);

        product.RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductCreated(Guid.NewGuid(), product.Id, 
            product.Name, product.Price, product.Description));

        return product;
    }

    public Product(Guid id, string name, decimal price, string description)
    {
        //if (!NameValidation(name))
        //    throw new ArgumentNullException();
        Id = id;
        Name = name;
        Price = price;
        Description = description;
    }

    public void Update(string name, decimal price, string description)
    {
        //if (!NameValidation(name))
        //    throw new ArgumentNullException();

        Name = name;
        Price = price;
        Description = description;

        RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductUpdated(Guid.NewGuid(), Id, name, price, description));
    }
    public void Delete()
        => RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductDeleted(Guid.NewGuid(), Id));

    private bool NameValidation(string name)
        => name.Contains("ABCD-");
}
