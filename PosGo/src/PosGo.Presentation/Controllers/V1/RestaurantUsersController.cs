using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Common.Constants;
using PosGo.Presentation.Abstractions;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize(Roles = SystemConstants.Role.ADMIN + "," + SystemConstants.Role.OWNER)]
public class RestaurantUsersController : ApiController
{
    public RestaurantUsersController(ISender sender) : base(sender) { }

    //[HttpPost]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(StatusCodes.Status403Forbidden)]
    //public async Task<IActionResult> UpsertRestaurantUser(
    //    Guid restaurantId,
    //    [FromBody] PosGo.Contract.Services.V1.RestaurantUser.Command.UpsertRestaurantUserCommand body,
    //    CancellationToken cancellationToken)
    //{
    //    // đảm bảo body.RestaurantId khớp route
    //    var command = body with { RestaurantId = restaurantId };

    //    var result = await Sender.Send(command, cancellationToken);

    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result.Value);
    //}
}
