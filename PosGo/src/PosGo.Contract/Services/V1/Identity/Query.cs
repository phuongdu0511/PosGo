using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Identity;

public static class Query
{
    public record Login(string UserName, string Password) : IQuery<Response.Authenticated>;

    public record Token(string? AccessToken, string? RefreshToken) : IQuery<Response.Authenticated>;
    public record SwitchRestaurant(Guid RestaurantId) : IQuery<Response.Authenticated>;
}
