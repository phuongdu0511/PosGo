using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PosGo.Contract.Enumerations;

namespace PosGo.Presentation.Attributes;

public class BinaryAuthorizeAttribute : TypeFilterAttribute
{
    public string RoleName { get; set; }
    public ActionType ActionValue { get; set; }

    public BinaryAuthorizeAttribute(string roleName, ActionType actionValue) : base(typeof(BinaryAuthorizationFilter))
    {
        RoleName = roleName;
        ActionValue = actionValue;
        Arguments = new object[] { RoleName, ActionValue };
    }
}


public class BinaryAuthorizationFilter : IAuthorizationFilter
{
    public string RoleName { get; set; }
    public ActionType ActionValue { get; set; }

    public BinaryAuthorizationFilter(string roleName, ActionType actionValue)
    {
        RoleName = roleName;
        ActionValue = actionValue;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var hasRole = CanAccessToAction(context.HttpContext);
        if (!hasRole)
            context.Result = new ForbidResult();
    }

    private bool CanAccessToAction(HttpContext httpContext)
    {
        if (!httpContext.User.Identity.IsAuthenticated)
            return false;

        if (string.IsNullOrEmpty(RoleName))
            return true;

        var claimRoleValue = httpContext.User.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrEmpty(claimRoleValue))
            return false;

        var log = httpContext.RequestServices.GetRequiredService<ILogger<BinaryAuthorizationFilter>>();
        try
        {
            var roles = JsonConvert.DeserializeObject<Dictionary<string, int>>(claimRoleValue);
            var actionValue = (int)ActionValue;

            foreach (var name in RoleName.Split(new char[] { ',', ';' }))
            {
                if (roles.TryGetValue(name, out int roleValue))
                {
                    var checkValue = (roleValue & actionValue) == actionValue;
                    if (checkValue)
                        return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Authorization Filter Error");
            return false;
        }
    }

}

public class BinaryAuthorizesANDAttribute : TypeFilterAttribute
{
    public List<ObjectPermisstion> Roles { get; set; }

    /// <summary>
    /// This api will be accept when all functions is pass
    /// </summary>
    /// <param name="roleRules">Has format: "Function1 : Action1 | Function2 : Action2 ..."</param>
    public BinaryAuthorizesANDAttribute(string roleRules) : base(typeof(BinaryAuthorizationsANDFilter))
    {
        Roles = new List<ObjectPermisstion>();
        var RoleRules = roleRules.Split('|');

        foreach (var roleRule in RoleRules)
        {
            var roles = roleRule.Split(':');
            Roles.Add(new ObjectPermisstion() { FunctionName = roles[0].Trim(), ActionType = (ActionType)System.Enum.Parse(typeof(ActionType), roles[1].Trim()) });
        }
        Arguments = new object[] { roleRules };
    }
}

public class BinaryAuthorizationsANDFilter : IAuthorizationFilter
{
    public List<ObjectPermisstion> Roles { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roleRules">Has format: "Function1 : Action1 | Function2 : Action2 ..."</param>
    public BinaryAuthorizationsANDFilter(string roleRules)
    {
        Roles = new List<ObjectPermisstion>();
        var RoleRules = roleRules.Split('|');

        foreach (var roleRule in RoleRules)
        {
            var roles = roleRule.Split(':');
            Roles.Add(new ObjectPermisstion() { FunctionName = roles[0].Trim(), ActionType = (ActionType)System.Enum.Parse(typeof(ActionType), roles[1].Trim()) });
        }
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var hasRole = CanAccessToAction(context.HttpContext);
        if (!hasRole)
            context.Result = new ForbidResult();
    }

    private bool CanAccessToAction(HttpContext httpContext)
    {
        if (!httpContext.User.Identity.IsAuthenticated)
            return false;

        var claimRoleValue = httpContext.User.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrEmpty(claimRoleValue))
            return false;

        var log = httpContext.RequestServices.GetRequiredService<ILogger<BinaryAuthorizationFilter>>();
        try
        {
            var roles = JsonConvert.DeserializeObject<Dictionary<string, int>>(claimRoleValue);

            foreach (var role in Roles)
            {
                if (roles.TryGetValue(role.FunctionName, out int roleValue))
                {
                    var actionValue = (int)role.ActionType;
                    var checkValue = (roleValue & actionValue) == actionValue;
                    if (!checkValue)
                        return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Authorizations Filter Error");
            return false;
        }
    }
}

public class BinaryAuthorizesORAttribute : TypeFilterAttribute
{
    public List<ObjectPermisstion> Roles { get; set; }

    /// <summary>
    /// This api will be accept when one of all functions is passed
    /// </summary>
    /// <param name="roleRules">Has format: "Function1 : Action1 | Function2 : Action2 ..."</param>
    public BinaryAuthorizesORAttribute(string roleRules) : base(typeof(BinaryAuthorizationsORFilter))
    {
        Roles = new List<ObjectPermisstion>();
        var RoleRules = roleRules.Split('|');

        foreach (var roleRule in RoleRules)
        {
            var roles = roleRule.Split(':');
            Roles.Add(new ObjectPermisstion() { FunctionName = roles[0].Trim(), ActionType = (ActionType)System.Enum.Parse(typeof(ActionType), roles[1].Trim()) });
        }
        Arguments = new object[] { roleRules };
    }
}

public class BinaryAuthorizationsORFilter : IAuthorizationFilter
{
    public List<ObjectPermisstion> Roles { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roleRules">Has format: "Function1 : Action1 | Function2 : Action2 ..."</param>
    public BinaryAuthorizationsORFilter(string roleRules)
    {
        Roles = new List<ObjectPermisstion>();
        var RoleRules = roleRules.Split('|');

        foreach (var roleRule in RoleRules)
        {
            var roles = roleRule.Split(':');
            Roles.Add(new ObjectPermisstion() { FunctionName = roles[0].Trim(), ActionType = (ActionType)System.Enum.Parse(typeof(ActionType), roles[1].Trim()) });
        }
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var hasRole = CanAccessToAction(context.HttpContext);
        if (!hasRole)
            context.Result = new ForbidResult();
    }

    private bool CanAccessToAction(HttpContext httpContext)
    {
        if (!httpContext.User.Identity.IsAuthenticated)
            return false;

        var claimRoleValue = httpContext.User.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrEmpty(claimRoleValue))
            return false;

        var log = httpContext.RequestServices.GetRequiredService<ILogger<BinaryAuthorizationFilter>>();
        try
        {
            var roles = JsonConvert.DeserializeObject<Dictionary<string, int>>(claimRoleValue);

            foreach (var role in Roles)
            {
                if (roles.TryGetValue(role.FunctionName, out int roleValue))
                {
                    var actionValue = (int)role.ActionType;
                    var checkValue = (roleValue & actionValue) == actionValue;
                    if (checkValue)
                        return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Authorizations Filter Error");
            return false;
        }
    }

}

public class ObjectPermisstion
{
    public string FunctionName { get; set; }
    public ActionType ActionType { get; set; }
}
