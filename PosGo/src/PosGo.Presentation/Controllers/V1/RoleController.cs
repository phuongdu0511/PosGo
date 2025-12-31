using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Role;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class RoleController : ApiController
{
    public RoleController(ISender sender) : base(sender) { }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.Add)]
    public async Task<IActionResult> CreateUserRole([FromBody] Command.CreateUserRole request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
