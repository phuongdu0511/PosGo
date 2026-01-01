using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosGo.Application.UserCases.V1.Queries.Menu;
using PosGo.Presentation.Abstractions;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class MenuController : ApiController
{
    public MenuController(ISender sender) : base(sender)
    {
        
    }

    [HttpGet("byUser")]
    public async Task<IActionResult> GetMenuByUser()
    {
        var request = new GetMenuByUserQuery();
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
