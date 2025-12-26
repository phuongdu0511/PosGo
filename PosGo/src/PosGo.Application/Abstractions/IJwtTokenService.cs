using System.Security.Claims;
using PosGo.Domain.Entities;

namespace PosGo.Application.Abstractions;

public interface IJwtTokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
