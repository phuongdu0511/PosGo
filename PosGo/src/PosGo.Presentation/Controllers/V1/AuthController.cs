using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Enumerations;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
public class AuthController : ApiController
{
    public AuthController(ISender sender) : base(sender)
    {
        
    }

    [HttpPost(Name = "Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AuthenticationV1([FromBody] Contract.Services.V1.Identity.Query.Login login)
    {
        var result = await Sender.Send(login);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("switch-restaurant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    [BinaryAuthorize(PermissionConstants.SwitchRestaurant, ActionType.View)]
    public async Task<IActionResult> SwitchRestaurant([FromBody] Contract.Services.V1.Identity.Query.SwitchRestaurant request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
