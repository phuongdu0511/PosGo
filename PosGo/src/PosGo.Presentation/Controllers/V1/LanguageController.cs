using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.Language;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class LanguageController : ApiController
{
    public LanguageController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Create Language
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.Add)]
    public async Task<IActionResult> CreateLanguage([FromBody] Command.CreateLanguageCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Language
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.Update)]
    public async Task<IActionResult> UpdateLanguage([FromBody] Command.UpdateLanguageCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Language Status
    /// </summary>
    [HttpPost("update-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.Update)]
    public async Task<IActionResult> UpdateLanguageStatus([FromBody] Command.UpdateLanguageStatusCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete Language
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.Delete)]
    public async Task<IActionResult> DeleteLanguage([FromBody] Command.DeleteLanguageCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Language by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<Response.LanguageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.View)]
    public async Task<IActionResult> GetLanguageById(int id)
    {
        var result = await Sender.Send(new Query.GetLanguageByIdQuery(id));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Language by Code
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(Result<Response.LanguageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.View)]
    public async Task<IActionResult> GetLanguageByCode(string code)
    {
        var result = await Sender.Send(new Query.GetLanguageByCodeQuery(code));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Active Languages
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(Result<List<Response.LanguageResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous] // Allow anonymous access for public language list
    public async Task<IActionResult> GetActiveLanguages()
    {
        var result = await Sender.Send(new Query.GetActiveLanguagesQuery());

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Languages with Pagination and Filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<Response.LanguageResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageRestaurants, ActionType.View)]
    public async Task<IActionResult> GetLanguages(
        string? searchTerm = null,
        bool? isActive = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetLanguagesQuery(
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