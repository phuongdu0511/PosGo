using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.Employee;
using PosGo.Domain.Utilities.Constants;
using PosGo.Presentation.Abstractions;
using PosGo.Presentation.Attributes;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class EmployeeController : ApiController
{
    public EmployeeController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Create Staff (Owner tạo nhân viên)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageEmployees, ActionType.Add)]
    public async Task<IActionResult> CreateStaff([FromBody] Command.CreateStaffCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Staffs (Lấy danh sách nhân viên phân trang)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<IEnumerable<Response.StaffResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageEmployees, ActionType.View)]
    public async Task<IActionResult> GetStaffs(
        string? searchTerm = null,
        string? sortColumn = null,
        string? sortOrder = null,
        string? sortColumnAndOrder = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = await Sender.Send(new Query.GetStaffsQuery(
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
    /// Get Staff by Id (Xem chi tiết nhân viên)
    /// </summary>
    [HttpGet("{staffId}")]
    [ProducesResponseType(typeof(Result<Response.StaffDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageEmployees, ActionType.View)]
    public async Task<IActionResult> GetStaffById(Guid staffId)
    {
        var result = await Sender.Send(new Query.GetStaffByIdQuery(staffId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Staff Permissions (Lấy quyền của Staff)
    /// </summary>
    [HttpGet("{staffId}/permissions")]
    [ProducesResponseType(typeof(Result<IEnumerable<Response.StaffPermissionResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageEmployees, ActionType.View)]
    public async Task<IActionResult> GetStaffPermissions(Guid staffId)
    {
        var result = await Sender.Send(new Query.GetStaffPermissionsQuery(staffId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Update Staff Permissions (Cập nhật quyền cho Staff - có thể truyền list)
    /// </summary>
    [HttpPost("{staffId}/permissions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageEmployees, ActionType.Update)]
    public async Task<IActionResult> UpdateStaffPermissions(Guid staffId, [FromBody] Command.UpdateStaffPermissionsCommand request)
    {
        var command = new Command.UpdateStaffPermissionsCommand(staffId, request.Permissions);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Remove Staff Permission (Xóa 1 quyền cụ thể của Staff)
    /// </summary>
    [HttpPost("{staffId}/permissions/remove")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageEmployees, ActionType.Delete)]
    public async Task<IActionResult> RemoveStaffPermission(Guid staffId, [FromBody] Command.RemoveStaffPermissionCommand request)
    {
        var command = new Command.RemoveStaffPermissionCommand(staffId, request.PermissionKey);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Transfer Staff (Chuyển nhân viên từ chi nhánh này sang chi nhánh khác)
    /// </summary>
    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [BinaryAuthorize(PermissionConstants.ManageEmployees, ActionType.Update)]
    public async Task<IActionResult> TransferStaff([FromBody] Command.TransferStaffCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
