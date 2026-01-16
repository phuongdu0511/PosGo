using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.DishCategory;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class DishCategoryController : ApiController
{
    public DishCategoryController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Create Dish Category
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Add)]
    public async Task<IActionResult> CreateDishCategory([FromBody] Command.CreateDishCategoryCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Dish Category
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateDishCategory([FromBody] Command.UpdateDishCategoryCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Dish Category Status
    /// </summary>
    [HttpPost("update-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateDishCategoryStatus([FromBody] Command.UpdateDishCategoryStatusCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete Dish Category
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Delete)]
    public async Task<IActionResult> DeleteDishCategory([FromBody] Command.DeleteDishCategoryCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Active Dish Categories
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(Result<List<Response.DishCategoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous] // Allow anonymous access for menu
    public async Task<IActionResult> GetActiveDishCategories([FromQuery] Guid restaurantId)
    {
        var result = await Sender.Send(new Query.GetActiveDishCategoriesQuery(restaurantId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Dish Categories with Pagination and Filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<Response.DishCategoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.View)]
    public async Task<IActionResult> GetDishCategories(
        string? searchTerm = null,
        bool? isActive = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetDishCategoriesQuery(
            searchTerm,
            isActive,
            sortColumn,
            SortOrderExtension.ConvertStringToSortOrder(sortOrder),
            SortOrderExtension.ConvertStringToSortOrderV2(sortColumnAndOrder),
            pageIndex,
            pageSize));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}