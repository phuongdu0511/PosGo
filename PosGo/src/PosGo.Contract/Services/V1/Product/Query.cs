using PosGo.Contract.Abstractions.Shared;
using static PosGo.Contract.Services.V1.Product.Response;

namespace PosGo.Contract.Services.V1.Product;

public static class Query
{
    public record GetProductsQuery() : IQuery<List<ProductResponse>>;
    public record GetProductByIdQuery(Guid Id) : IQuery<ProductResponse>;
}
