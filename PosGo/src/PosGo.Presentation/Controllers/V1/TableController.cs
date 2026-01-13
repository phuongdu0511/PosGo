using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.Table;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class TableController : ApiController
{
    public TableController(ISender sender) : base(sender)
    {
    }

    #region TableArea APIs

    /// <summary>
    /// Create Table Area
    /// </summary>
    [HttpPost("areas")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageTables, ActionType.Add)]
    public async Task<IActionResult> CreateTableArea([FromBody] Command.CreateTableAreaCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Table Area
    /// </summary>
    [HttpPost("areas/update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageTables, ActionType.Update)]
    public async Task<IActionResult> UpdateTableArea([FromBody] Command.UpdateTableAreaCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete Table Area
    /// </summary>
    [HttpPost("areas/delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageTables, ActionType.Delete)]
    public async Task<IActionResult> DeleteTableArea([FromBody] Command.DeleteTableAreaCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    #endregion

    #region Table APIs

    /// <summary>
    /// Create Table
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageTables, ActionType.Add)]
    public async Task<IActionResult> CreateTable([FromBody] Command.CreateTableCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Table by QR Code
    /// </summary>
    [HttpGet("qr/{qrCodeToken}")]
    [ProducesResponseType(typeof(Result<Response.TableResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IActionResult> GetTableByQrCode(string qrCodeToken)
    {
        var result = await Sender.Send(new Query.GetTableByQrCodeQuery(qrCodeToken));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Table
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageTables, ActionType.Update)]
    public async Task<IActionResult> UpdateTable([FromBody] Command.UpdateTableCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Table Status
    /// </summary>
    [HttpPost("update-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageTables, ActionType.Update)]
    public async Task<IActionResult> UpdateTableStatus([FromBody] Command.UpdateTableStatusCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete Table
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageTables, ActionType.Delete)]
    public async Task<IActionResult> DeleteTable([FromBody] Command.DeleteTableCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Tables by Area
    /// </summary>
    [HttpGet("areas/{areaId}/tables")]
    [ProducesResponseType(typeof(Result<List<Response.TableResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageTables, ActionType.View)]
    public async Task<IActionResult> GetTablesByArea(int areaId)
    {
        var result = await Sender.Send(new Query.GetTablesByAreaQuery(areaId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Tables by Restaurant with Pagination and Filters
    /// </summary>
    [HttpGet("restaurant")]
    [ProducesResponseType(typeof(Result<PagedResult<Response.TableResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageTables, ActionType.View)]
    public async Task<IActionResult> GetTablesByRestaurant(
        string? searchTerm = null,
        int? areaId = null,
        bool? isActive = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetTablesByRestaurantQuery(
            searchTerm, 
            areaId, 
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

    #endregion
}