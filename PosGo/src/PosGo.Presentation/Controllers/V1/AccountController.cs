using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.Account;
using PosGo.Presentation.Abstractions;
using static PosGo.Contract.Services.V1.Product.Command;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class AccountController : ApiController
{
    public AccountController(ISender sender) : base(sender)
    {
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Accounts()
    {
        var result = await Sender.Send(new Query.GetAccountMe());

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Accounts([FromBody] Command.UpdateProfileCommand updateProfile)
    {
        var updateProfileCommand = new Command.UpdateProfileCommand(updateProfile.FullName, updateProfile.Phone);
        var result = await Sender.Send(updateProfileCommand);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(Result<Response.AccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Accounts(Guid userId)
    {
        var result = await Sender.Send(new Query.GetAccountByIdQuery(userId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("GetAccounts")]
    [ProducesResponseType(typeof(Result<IEnumerable<Response.AccountResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Accounts(string? serchTerm = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetAccountsQuery(serchTerm, sortColumn,
            SortOrderExtension.ConvertStringToSortOrder(sortOrder),
            SortOrderExtension.ConvertStringToSortOrderV2(sortColumnAndOrder),
            pageIndex,
            pageSize));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
