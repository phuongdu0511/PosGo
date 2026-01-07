using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.Restaurant;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class RestaurantController : ApiController
{
    public RestaurantController(ISender sender) : base(sender)
    {

    }

    /// <summary>
    /// Create Restaurant
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.Add)]
    public async Task<IActionResult> Restaurants([FromBody] Command.CreateRestaurantCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Restaurants
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortOrder"></param>
    /// <param name="sortColumnAndOrder"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(Result<IEnumerable<Response.RestaurantResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.View)]
    public async Task<IActionResult> Restaurants(string? searchTerm = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetRestaurantsQuery(searchTerm, sortColumn,
            SortOrderExtension.ConvertStringToSortOrder(sortOrder),
            SortOrderExtension.ConvertStringToSortOrderV2(sortColumnAndOrder),
            pageIndex,
            pageSize));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Restaurant by Id
    /// </summary>
    /// <param name="restaurantId"></param>
    /// <returns></returns>
    [HttpGet("{restaurantId}")]
    [ProducesResponseType(typeof(Result<Response.RestaurantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.View)]
    public async Task<IActionResult> Restaurants(Guid restaurantId)
    {
        var result = await Sender.Send(new Query.GetRestaurantByIdQuery(restaurantId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Restaurant
    /// </summary>
    /// <param name="restaurantId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{restaurantId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.Update)]
    public async Task<IActionResult> Update(Guid restaurantId, [FromBody] Command.UpdateRestaurantCommand request)
    {
        var command = new Command.UpdateRestaurantCommand(
            restaurantId, request.Name, request.Slug, request.DefaultLanguageId,
            request.Address, request.City, request.Country, request.Phone, request.TimeZone,
            request.LogoUrl, request.Description, request.RestaurantGroupId, request.IsActive);

        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Restaurant Plan
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("plan")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.Update)]
    public async Task<IActionResult> UpdateRestaurantPlan([FromBody] Command.UpdateRestaurantPlanCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
    /// <summary>
    /// Assign User To Restaurant
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("assign-user")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.Update)]
    public async Task<IActionResult> AssignUserToRestaurant([FromBody] Command.AssignUserToRestaurantCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.Delete)]
    public async Task<IActionResult> DeleteRestaurant([FromBody] Command.DeleteRestaurantCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get My Restaurants (danh sách restaurant mà user thuộc về)
    /// </summary>
    /// <returns></returns>
    [HttpGet("my-restaurants")]
    [ProducesResponseType(typeof(Result<IEnumerable<Response.MyRestaurantResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.SwitchRestaurant, ActionType.View)]
    public async Task<IActionResult> GetMyRestaurants()
    {
        var result = await Sender.Send(new Query.GetMyRestaurantsQuery());

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
