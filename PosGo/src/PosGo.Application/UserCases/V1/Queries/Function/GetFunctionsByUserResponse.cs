using PosGo.Contract.Enumerations;

namespace PosGo.Application.UserCases.V1.Queries.Function;

public class GetFunctionsByUserResponse
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string? CssClass { get; set; }
    public int SortOrder { set; get; }
    public int ParrentId { get; set; }
}
