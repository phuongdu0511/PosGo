using Microsoft.AspNetCore.Http;
using PosGo.Contract.Abstractions.Shared.CommonServices;
using System.Security.Claims;

namespace PosGo.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
    }

    public Guid? UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public IReadOnlyList<string> Roles =>
        _httpContextAccessor.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();
}
