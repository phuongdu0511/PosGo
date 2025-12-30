using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.User;
using PosGo.Domain.Enums;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

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
    [BinaryAuthorize(PermissionConstants.ManageUsers, ActionType.View)]
    public async Task<IActionResult> Users(string? searchTerm = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetUsersQuery(searchTerm, sortColumn,
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
    [BinaryAuthorize(PermissionConstants.ManageUsers, ActionType.View)]
    public async Task<IActionResult> Users(Guid userId)
    {
        var result = await Sender.Send(new Query.GetUserByIdQuery(userId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost(Name = "CreateUser")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageUsers, ActionType.Add)]
    public async Task<IActionResult> Users([FromBody] Command.CreateUserCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("{userId}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageUsers, ActionType.Update)]
    public async Task<IActionResult> Users(Guid userId, [FromBody] Command.UpdateUserCommand request)
    {
        var command = new Command.UpdateUserCommand(userId, request.FullName, request.PhoneNumber);
        var result = await Sender.Send(command);
        return Ok(result);
    }

    [HttpPost("change-password/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageUsers, ActionType.Update)]
    public async Task<IActionResult> Users(Guid userId, [FromBody] Command.ChangePasswordUserCommand request)
    {
        var command = new Command.ChangePasswordUserCommand(userId, request.NewPassword, request.ConfirmNewPassword);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok();
    }

    [HttpPost("change-status/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageUsers, ActionType.Update)]
    public async Task<IActionResult> ChangeStatus(Guid userId, [FromBody] Command.ChangeStatusUserCommand request)
    {
        var command = new Command.ChangeStatusUserCommand(userId, request.status);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok();
    }

    [HttpPost("update-roles/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageUsers, ActionType.Update)]
    public async Task<IActionResult> UpdateUserRoles(Guid userId, [FromBody] Command.UpdateUserRolesCommand request)
    {
        var command = new Command.UpdateUserRolesCommand(userId, request.RoleCodes);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok();
    }
}
