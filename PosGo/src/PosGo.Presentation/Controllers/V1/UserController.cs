using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.User;
using PosGo.Presentation.Abstractions;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class UserController : ApiController
{
    public UserController(ISender sender) : base(sender)
    {
    }

    [HttpGet(Name = "GetUsers")]
    [ProducesResponseType(typeof(Result<IEnumerable<Response.UserResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Users(string? serchTerm = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetUsersQuery(serchTerm, sortColumn,
            SortOrderExtension.ConvertStringToSortOrder(sortOrder),
            SortOrderExtension.ConvertStringToSortOrderV2(sortColumnAndOrder),
            pageIndex,
            pageSize));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(Result<Response.UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Users(Guid userId)
    {
        var result = await Sender.Send(new Query.GetUserByIdQuery(userId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost(Name = "CreateUser")]
    //[Authorize(Roles = "SystemAdmin")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Users([FromBody] Command.CreateUserCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    //[Authorize(Roles = "SystemAdmin, Owner")]
    public async Task<IActionResult> Users(Guid userId, [FromBody] Command.UpdateUserCommand request)
    {
        var updateUserCommand = new Command.UpdateUserCommand(userId, request.UserName, request.FullName, request.Phone);
        var result = await Sender.Send(updateUserCommand);
        return Ok(result);
    }

    [HttpPost("change-password/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    //[Authorize(Roles = "SystemAdmin, Owner")]
    public async Task<IActionResult> Users(Guid userId, [FromBody] Command.ChangePasswordUserCommand request)
    {
        var changePasswordUserCommand = new Command.ChangePasswordUserCommand(userId, request.NewPassword, request.ConfirmNewPassword);
        var result = await Sender.Send(changePasswordUserCommand);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok();
    }

    [HttpPost("change-status/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    //[Authorize(Roles = "SystemAdmin, Owner")]
    public async Task<IActionResult> ChangeStatus(Guid userId, [FromBody] Command.ChangeStatusUserCommand request)
    {
        var changePasswordUserCommand = new Command.ChangeStatusUserCommand(userId, request.status);
        var result = await Sender.Send(changePasswordUserCommand);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok();
    }
}
