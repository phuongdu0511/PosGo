using System.Security.Claims;
using PosGo.Domain.Entities;

namespace PosGo.Application.Abstractions;

public interface IJwtTokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
    Task<string> GenerateAccessTokenForRestaurantAsync(User user, string scope, Guid restaurantId);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
