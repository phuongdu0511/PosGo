using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Account;
using PosGo.Presentation.Abstractions;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class AccountController : ApiController
{
    public AccountController(ISender sender) : base(sender)
    {
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Accounts()
    {
        var result = await Sender.Send(new Query.GetAccountMe());

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Accounts([FromBody] Command.UpdateProfileCommand updateProfile)
    {
        var updateProfileCommand = new Command.UpdateProfileCommand(updateProfile.FullName, updateProfile.Phone);
        var result = await Sender.Send(updateProfileCommand);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Accounts([FromBody] Command.ChangePasswordCommand command)
    {
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok();
    }
}
