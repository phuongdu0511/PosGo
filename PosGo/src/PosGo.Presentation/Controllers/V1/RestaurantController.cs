using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.Restaurant;
using PosGo.Presentation.Abstractions;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize(Roles = SystemConstants.Role.ADMIN)]
public class RestaurantController : ApiController
{
    public RestaurantController(ISender sender) : base(sender)
    {
        
    }

    [HttpPost(Name = "CreateRestaurant")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Restaurants([FromBody] Command.CreateRestaurantCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet(Name = "GetRestaurants")]
    [ProducesResponseType(typeof(Result<IEnumerable<Response.RestaurantResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Restaurants(string? serchTerm = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetRestaurantsQuery(serchTerm, sortColumn,
            SortOrderExtension.ConvertStringToSortOrder(sortOrder),
            SortOrderExtension.ConvertStringToSortOrderV2(sortColumnAndOrder),
            pageIndex,
            pageSize));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("{restaurantId}")]
    [ProducesResponseType(typeof(Result<Response.RestaurantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Restaurants(Guid restaurantId)
    {
        var result = await Sender.Send(new Query.GetRestaurantByIdQuery(restaurantId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("{restaurantId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateRestaurant(Guid restaurantId, [FromBody] Command.UpdateRestaurantCommand request)
    {
        var updateRestaurantCommand = new Command.UpdateRestaurantCommand(
            restaurantId, request.Name, request.Slug, request.DefaultLanguageId, 
            request.Address, request.City, request.Country, request.Phone, request.TimeZone, 
            request.LogoUrl, request.Description, request.RestaurantGroupId);

        var result = await Sender.Send(updateRestaurantCommand);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
