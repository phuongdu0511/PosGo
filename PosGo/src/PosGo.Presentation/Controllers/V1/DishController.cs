using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.Dish;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class DishController : ApiController
{
    public DishController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Create Dish
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Add)]
    public async Task<IActionResult> CreateDish([FromBody] Command.CreateDishCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Dish
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateDish([FromBody] Command.UpdateDishCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Dish Status
    /// </summary>
    [HttpPost("update-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateDishStatus([FromBody] Command.UpdateDishStatusCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Dish Availability
    /// </summary>
    [HttpPost("update-availability")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateDishAvailability([FromBody] Command.UpdateDishAvailabilityCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Dish Menu Visibility
    /// </summary>
    [HttpPost("update-menu-visibility")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateDishMenuVisibility([FromBody] Command.UpdateDishMenuVisibilityCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete Dish
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Delete)]
    public async Task<IActionResult> DeleteDish([FromBody] Command.DeleteDishCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Dish by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<Response.DishDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.View)]
    public async Task<IActionResult> GetDishById(int id)
    {
        var result = await Sender.Send(new Query.GetDishByIdQuery(id));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Dish by Code
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(Result<Response.DishResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.View)]
    public async Task<IActionResult> GetDishByCode(string code)
    {
        var result = await Sender.Send(new Query.GetDishByCodeQuery(code));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Active Menu Dishes
    /// </summary>
    [HttpGet("menu")]
    [ProducesResponseType(typeof(Result<List<Response.DishResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous] // Allow anonymous access for menu
    public async Task<IActionResult> GetActiveMenuDishes([FromQuery] Guid restaurantId)
    {
        var result = await Sender.Send(new Query.GetActiveMenuDishesQuery(restaurantId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Dishes by Category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(typeof(Result<List<Response.DishResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.View)]
    public async Task<IActionResult> GetDishesByCategory(int categoryId)
    {
        var result = await Sender.Send(new Query.GetDishesByCategoryQuery(categoryId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Dishes with Pagination and Filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<Response.DishResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.View)]
    public async Task<IActionResult> GetDishes(
        string? searchTerm = null,
        int? categoryId = null,
        int? unitId = null,
        int? dishTypeId = null,
        bool? isActive = null,
        bool? isAvailable = null,
        bool? showOnMenu = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetDishesQuery(
            searchTerm,
            categoryId,
            unitId,
            dishTypeId,
            isActive,
            isAvailable,
            showOnMenu,
            sortColumn,
            SortOrderExtension.ConvertStringToSortOrder(sortOrder),
            SortOrderExtension.ConvertStringToSortOrderV2(sortColumnAndOrder),
            pageIndex,
            pageSize));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Dish Translations
    /// </summary>
    [HttpPost("{id}/translations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateDishTranslations(int id, [FromBody] List<DishTranslationDto> translations)
    {
        var result = await Sender.Send(new Command.UpdateDishTranslationsCommand(id, translations));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Dish Translations
    /// </summary>
    [HttpGet("{id}/translations")]
    [ProducesResponseType(typeof(Result<List<Response.DishTranslationResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.View)]
    public async Task<IActionResult> GetDishTranslations(int id)
    {
        var result = await Sender.Send(new Query.GetDishTranslationsQuery(id));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}