using Carter;
using PosGo.Presentation.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CommandV1 = PosGo.Contract.Services.V1.Product;
using PosGo.Contract.Extensions;

namespace PosGo.Presentation.APIs.Products;

public class ProductApi : ApiEndpoint, ICarterModule
{
    private const string BaseUrl = "/api/minimal/v{version:apiVersion}/products";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group1 = app.NewVersionedApi("products")
            .MapGroup(BaseUrl).HasApiVersion(1).RequireAuthorization();

        group1.MapPost(string.Empty, CreateProductsV1);
        group1.MapGet(string.Empty, GetProductsV1);
        group1.MapGet("{productId}", GetProductsByIdV1);
        group1.MapDelete("{productId}", DeleteProductsV1);
        group1.MapPut("{productId}", UpdateProductsV1);
    }

    public static async Task<IResult> CreateProductsV1(ISender sender, [FromBody] CommandV1.Command.CreateProductCommand CreateProduct)
    {
        var result = await sender.Send(CreateProduct);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Results.Ok(result);
    }

    public static async Task<IResult> GetProductsV1(ISender sender, string? serchTerm = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new CommandV1.Query.GetProductsQuery(serchTerm, sortColumn,
            SortOrderExtension.ConvertStringToSortOrder(sortOrder),
            SortOrderExtension.ConvertStringToSortOrderV2(sortColumnAndOrder),
            pageIndex,
            pageSize));
        return Results.Ok(result);
    }

    public static async Task<IResult> GetProductsByIdV1(ISender sender, Guid productId)
    {
        var result = await sender.Send(new CommandV1.Query.GetProductByIdQuery(productId));
        return Results.Ok(result);
    }

    public static async Task<IResult> DeleteProductsV1(ISender sender, Guid productId)
    {
        var result = await sender.Send(new CommandV1.Command.DeleteProductCommand(productId));
        return Results.Ok(result);
    }

    public static async Task<IResult> UpdateProductsV1(ISender sender, Guid productId, [FromBody] CommandV1.Command.UpdateProductCommand updateProduct)
    {
        var updateProductCommand = new CommandV1.Command.UpdateProductCommand(productId, updateProduct.Name, updateProduct.Price, updateProduct.Description);
        var result = await sender.Send(updateProductCommand);
        return Results.Ok(result);
    }
}
