using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.DishSku;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class DishSkuController : ApiController
{
    public DishSkuController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Create Dish SKU
    /// </summary>
    [HttpPost("dish/{dishId}/skus")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Add)]
    public async Task<IActionResult> CreateDishSku(int dishId, [FromBody] Command.CreateDishSkuCommand request)
    {
        // Ensure dishId matches
        if (dishId != request.DishId)
        {
            return BadRequest("DishId in URL does not match request body.");
        }

        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Dish SKU
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateDishSku([FromBody] Command.UpdateDishSkuCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete Dish SKU
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Delete)]
    public async Task<IActionResult> DeleteDishSku([FromBody] Command.DeleteDishSkuCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update SKU Price
    /// </summary>
    [HttpPost("update-price")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateSkuPrice([FromBody] Command.UpdateSkuPriceCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update SKU Stock
    /// </summary>
    [HttpPost("update-stock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> UpdateSkuStock([FromBody] Command.UpdateSkuStockCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Set Default SKU
    /// </summary>
    [HttpPost("set-default")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.Update)]
    public async Task<IActionResult> SetDefaultSku([FromBody] Command.SetDefaultSkuCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Dish SKUs
    /// </summary>
    [HttpGet("dish/{dishId}/skus")]
    [ProducesResponseType(typeof(Result<List<Response.DishSkuResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageDishes, ActionType.View)]
    public async Task<IActionResult> GetDishSkus(int dishId)
    {
        var result = await Sender.Send(new Query.GetDishSkusQuery(dishId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Available SKUs
    /// </summary>
    [HttpGet("dish/{dishId}/available-skus")]
    [ProducesResponseType(typeof(Result<List<Response.DishSkuResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous] // Allow anonymous access for menu
    public async Task<IActionResult> GetAvailableSkus(int dishId)
    {
        var result = await Sender.Send(new Query.GetAvailableSkusQuery(dishId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Default SKU
    /// </summary>
    [HttpGet("dish/{dishId}/default-sku")]
    [ProducesResponseType(typeof(Result<Response.DishSkuResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous] // Allow anonymous access for menu
    public async Task<IActionResult> GetDefaultSku(int dishId)
    {
        var result = await Sender.Send(new Query.GetDefaultSkuQuery(dishId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Find SKU by Variant Options
    /// </summary>
    [HttpGet("find")]
    [ProducesResponseType(typeof(Result<Response.DishSkuResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous] // Allow anonymous access for menu
    public async Task<IActionResult> FindSkuByVariantOptions(int dishId, [FromQuery] List<int> variantOptionIds)
    {
        var result = await Sender.Send(new Query.GetSkuByVariantOptionsQuery(dishId, variantOptionIds));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}