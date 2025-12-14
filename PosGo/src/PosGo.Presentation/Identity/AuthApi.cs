using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PosGo.Presentation.Abstractions;

namespace PosGo.Presentation.Identity;

public class AuthApi : ApiEndpoint, ICarterModule
{
    private const string BaseUrl = "/api/minimal/v{version:apiVersion}/auth";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group1 = app.NewVersionedApi("Authentication")
            .MapGroup(BaseUrl).HasApiVersion(1).RequireAuthorization();

        group1.MapPost("login", AuthenticationV1).AllowAnonymous();
    }

    public static async Task<IResult> AuthenticationV1(ISender sender, [FromBody] Contract.Services.V1.Identity.Query.Login login)
    {
        var result = await sender.Send(login);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Results.Ok(result);
    }
}
