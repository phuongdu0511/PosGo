using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using static PosGo.Contract.Services.V1.Employee.Response;

namespace PosGo.Contract.Services.V1.Employee;

public static class Query
{
    public record GetStaffsQuery(
        string? SearchTerm,
        string? SortColumn,
        SortOrder? SortOrder,
        IDictionary<string, SortOrder>? SortColumnAndOrder,
        int PageIndex,
        int PageSize
    ) : IQuery<PagedResult<StaffResponse>>;

    public record GetStaffByIdQuery(Guid StaffId) : IQuery<StaffDetailResponse>;

    public record GetStaffPermissionsQuery(Guid StaffId) : IQuery<List<StaffPermissionResponse>>;
}
