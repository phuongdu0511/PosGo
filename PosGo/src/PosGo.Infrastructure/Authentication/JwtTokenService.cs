using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PosGo.Application.Abstractions;
using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Constants;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Infrastructure.DependencyInjection.Options;

namespace PosGo.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOption jwtOption = new JwtOption();
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IRepositoryBase<Domain.Entities.RestaurantUser, int> _restaurantUserRepository;

    public JwtTokenService(
        IConfiguration configuration, 
        UserManager<User> userManager, 
        RoleManager<Role> roleManager, 
        IRepositoryBase<RestaurantUser, int> restaurantUserRepository)
    {
        configuration.GetSection(nameof(JwtOption)).Bind(jwtOption);
        _userManager = userManager;
        _roleManager = roleManager;
        _restaurantUserRepository = restaurantUserRepository;
    }
    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        // 1. Lấy các mapping RestaurantUser đang Active của user
        var restaurantUsers = await _restaurantUserRepository
            .FindAll(x => x.UserId == user.Id && x.Status == ERestaurantUserStatus.Active)
            .ToListAsync();

        var restaurantIds = restaurantUsers
            .Select(x => x.RestaurantId)
            .Distinct()
            .ToList();

        // 2. Nếu user chỉ thuộc đúng 1 nhà hàng -> auto set RestaurantId
        Guid? activeRestaurantId = null;
        if (restaurantIds.Count == 1)
        {
            activeRestaurantId = restaurantIds[0];
        }

        var ActionDes = EnumHelper<ActionType>.GetNameAndDescription().Values;
        var roles = await GetRolesByUserAsync(user);
        //var roles = await GetRolesByUserBinaryAsync(user);

        var tokenKey = jwtOption.SecretKey;
        var issuer = jwtOption.Issuer;
        var audience = jwtOption.Audience;
        var expire = jwtOption.ExpireMin;

        var dateExpire = DateTime.UtcNow.AddHours(7).AddMinutes(expire);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.GivenName,user.FullName),
            new Claim(nameof(ActionDes),JsonConvert.SerializeObject(ActionDes)),
            new Claim(ClaimTypes.Role,JsonConvert.SerializeObject(roles)),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        // 3. Nếu xác định được 1 RestaurantId duy nhất => add vào token
        if (activeRestaurantId.HasValue)
        {
            claims.Add(new Claim("restaurant_id", activeRestaurantId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, expires: dateExpire, signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateAccessTokenForRestaurantAsync(User user, Guid restaurantId)
    {
        var ActionDes = EnumHelper<ActionType>.GetNameAndDescription().Values;
        var roles = await GetRolesByUserAsync(user);
        //var roles = await GetRolesByUserBinaryAsync(user);

        var tokenKey = jwtOption.SecretKey;
        var issuer = jwtOption.Issuer;
        var audience = jwtOption.Audience;
        var expire = jwtOption.ExpireMin;

        var dateExpire = DateTime.UtcNow.AddHours(7).AddMinutes(expire);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.GivenName, user.FullName ?? string.Empty),
            new Claim(nameof(ActionDes), JsonConvert.SerializeObject(ActionDes)),
            new Claim(ClaimTypes.Role, JsonConvert.SerializeObject(roles)),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim("restaurant_id", restaurantId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: dateExpire,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var Key = Encoding.UTF8.GetBytes(jwtOption.SecretKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Key),
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    /// <summary>
    /// Currently: Hiện tại check quyền theo quyền tổng hợp quyền trong RoleClaim và UserClaim
    /// Currently: Không ưu tiên UserClaim mà lấy quyền cao nhất của 1 trong 2 RoleClaim hoặc UserClaim
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<Dictionary<string, int>> GetRolesByUserAsync(User user)
    {
        var results = new Dictionary<string, int>();
        var roleClaims = await GetClaimsByUserRoleAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        if (userClaims.Any())
            roleClaims.AddRange(userClaims);

        var roleClaimNames = roleClaims.Select(x => x.Type).Distinct();

        foreach (var name in roleClaimNames)
        {
            if (!results.ContainsKey(name))
            {
                var claims = roleClaims.Where(p => p.Type == name);
                if (!claims.Any(p => p.Value == PermissionConstants.Deny.ToString()))
                {
                    var value = 0;
                    foreach (var claim in claims)
                    {
                        if (int.TryParse(claim.Value, out int claimValue))
                            value |= claimValue;
                    }
                    results.Add(name, value);
                }
            }
        }

        return results;
    }

    private async Task<Dictionary<string, string>> GetRolesByUserBinaryAsync(User user)
    {
        var resultBinary = new Dictionary<string, string>();
        var resultInt = new Dictionary<string, int>();

        var roleClaims = await GetClaimsByUserRoleAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        if (userClaims.Any())
            roleClaims.AddRange(userClaims);

        var roleClaimNames = roleClaims.Select(x => x.Type).Distinct();

        foreach (var name in roleClaimNames)
        {
            if (!resultInt.ContainsKey(name))
            {
                var claims = roleClaims.Where(p => p.Type == name);
                if (!claims.Any(p => p.Value == PermissionConstants.Deny.ToString()))
                {
                    var value = 0;
                    foreach (var claim in claims)
                    {
                        if (int.TryParse(claim.Value, out int claimValue))
                            value |= claimValue;
                    }
                    resultInt.Add(name, value);
                }
            }
        }

        // Convert Decimal to Binary and revert also insert '0' at the end of the role if role count less than action count
        /**
		 * Action count = 9 ()
         * Example: 8 => Convert = 1000 => Revert = 0001 =>> Insert '0' = 000100000
         * Example: 13 => Convert = 1101 => Revert = 1011 =>> Insert '0' = 101100000
         */
        var ActionCount = EnumHelper<ActionType>.GetNameAndDescription().Count;

        foreach (var item in resultInt)
        {
            var RoleBinary = StringBuilderReverseMethod(DecimalToBinary(item.Value));
            var RoleBinaryLength = RoleBinary.Length;

            if (RoleBinaryLength < ActionCount)
            {
                RoleBinary.Insert(RoleBinaryLength, "0", ActionCount - RoleBinaryLength);
            }
            resultBinary.Add(item.Key, RoleBinary.ToString());
        }

        return resultBinary;
    }

    private string DecimalToBinary(int num)
    {
        var bin = new StringBuilder();
        do
        {
            bin.Insert(0, (num % 2));
            num /= 2;
        } while (num != 0);

        return bin.ToString();
    }

    private StringBuilder StringBuilderReverseMethod(string stringToReverse)
    {
        var sb = new StringBuilder(stringToReverse.Length);
        for (int i = stringToReverse.Length - 1; i >= 0; i--)
        {
            sb.Append(stringToReverse[i]);
        }
        return sb;
    }

    private async Task<List<Claim>> GetClaimsByUserRoleAsync(User user)
    {
        var roleNames = await _userManager.GetRolesAsync(user);
        var roles = await _roleManager.Roles.Where(p => roleNames.Contains(p.Name)).ToListAsync();
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            var resultClaims = await _roleManager.GetClaimsAsync(role);
            if (resultClaims.Any())
            {
                roleClaims.AddRange(resultClaims);
            }
        }
        return roleClaims;
    }
}
