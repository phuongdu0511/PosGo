using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.DishVariant;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class DishVariantController : ApiController
{
    public DishVariantController(ISender sender) : base(sender)
    {
    }

    // CREATE endpoints removed - Variants are now created together with Dish

    /// <summary>
    /// Update Dish Variant
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateDishVariant([FromBody] Command.UpdateDishVariantCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete Dish Variant
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Delete)]
    public async Task<IActionResult> DeleteDishVariant([FromBody] Command.DeleteDishVariantCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    // CREATE endpoints removed - Variant Options are now created together with Dish

    /// <summary>
    /// Update Variant Option
    /// </summary>
    [HttpPost("option/update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateVariantOption([FromBody] Command.UpdateVariantOptionCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete Variant Option
    /// </summary>
    [HttpPost("option/delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Delete)]
    public async Task<IActionResult> DeleteVariantOption([FromBody] Command.DeleteVariantOptionCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Dish Variant Tree
    /// </summary>
    [HttpGet("dish/{dishId}/variant-tree")]
    [ProducesResponseType(typeof(Result<Response.DishVariantTreeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.View)]
    public async Task<IActionResult> GetDishVariantTree(int dishId, string? language = "vi")
    {
        var result = await Sender.Send(new Query.GetDishVariantTreeQuery(dishId, language));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Dish Variants
    /// </summary>
    [HttpGet("dish/{dishId}/variants")]
    [ProducesResponseType(typeof(Result<List<Response.DishVariantResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.View)]
    public async Task<IActionResult> GetDishVariants(int dishId, string? language = "vi")
    {
        var result = await Sender.Send(new Query.GetDishVariantsQuery(dishId, language));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Variant Options
    /// </summary>
    [HttpGet("variant/{variantId}/options")]
    [ProducesResponseType(typeof(Result<List<Response.VariantOptionResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.View)]
    public async Task<IActionResult> GetVariantOptions(int variantId, string? language = "vi")
    {
        var result = await Sender.Send(new Query.GetVariantOptionsQuery(variantId, language));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}