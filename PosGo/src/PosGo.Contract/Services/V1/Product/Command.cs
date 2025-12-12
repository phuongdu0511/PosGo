using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Product;

public static class Command
{
    public record CreateProductCommand(string Name, decimal Price, string Description, string sku) : ICommand;

    public record UpdateProductCommand(Guid Id, string Name, decimal Price, string Description) : ICommand;

    public record DeleteProductCommand(Guid Id) : ICommand;
}
