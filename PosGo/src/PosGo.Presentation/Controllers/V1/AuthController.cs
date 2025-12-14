using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Presentation.Abstractions;

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
}
