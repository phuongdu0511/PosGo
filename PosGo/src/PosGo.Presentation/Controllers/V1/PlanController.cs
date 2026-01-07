using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.Plan;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class PlanController : ApiController
{
    public PlanController(ISender sender) : base(sender)
    {
        
    }

    /// <summary>
    /// Create Plan
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManagePlans, ActionType.Add)]
    public async Task<IActionResult> CreatePlan([FromBody] Command.CreatePlanCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Plans
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<IEnumerable<Response.PlanResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManagePlans, ActionType.View)]
    public async Task<IActionResult> GetPlans(
        string? searchTerm = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetPlansQuery(
            searchTerm,
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
    /// Get Plan by Id
    /// </summary>
    [HttpGet("{planId}")]
    [ProducesResponseType(typeof(Result<Response.PlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManagePlans, ActionType.View)]
    public async Task<IActionResult> GetPlanById(Guid planId)
    {
        var result = await Sender.Send(new Query.GetPlanByIdQuery(planId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Plan
    /// </summary>
    [HttpPost("{planId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManagePlans, ActionType.Update)]
    public async Task<IActionResult> UpdatePlan(Guid planId, [FromBody] Command.UpdatePlanCommand request)
    {
        var command = new Command.UpdatePlanCommand(planId, request.Code, request.Description, request.IsActive);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete Plan
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManagePlans, ActionType.Delete)]
    public async Task<IActionResult> DeletePlan([FromBody] Command.DeletePlanCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Plan Functions
    /// </summary>
    [HttpGet("{planId}/functions")]
    [ProducesResponseType(typeof(Result<IEnumerable<Response.PlanFunctionResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManagePlans, ActionType.View)]
    public async Task<IActionResult> GetPlanFunctions(Guid planId)
    {
        var result = await Sender.Send(new Query.GetPlanFunctionsQuery(planId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Add Plan Function
    /// </summary>
    [HttpPost("{planId}/functions")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManagePlans, ActionType.Add)]
    public async Task<IActionResult> AddPlanFunction(Guid planId, [FromBody] Command.AddPlanFunctionCommand request)
    {
        var command = new Command.AddPlanFunctionCommand(planId, request.FunctionId, request.ActionValue);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Plan Function
    /// </summary>
    [HttpPost("{planId}/functions/update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManagePlans, ActionType.Update)]
    public async Task<IActionResult> UpdatePlanFunction(Guid planId, [FromBody] Command.UpdatePlanFunctionCommand request)
    {
        var command = new Command.UpdatePlanFunctionCommand(planId, request.FunctionId, request.ActionValue);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Remove Plan Function
    /// </summary>
    [HttpPost("{planId}/functions/remove")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManagePlans, ActionType.Delete)]
    public async Task<IActionResult> RemovePlanFunction(Guid planId, [FromBody] Command.RemovePlanFunctionCommand request)
    {
        var command = new Command.RemovePlanFunctionCommand(planId, request.FunctionId);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
