namespace PosGo.Application.UserCases.V1.Queries.Menu;

public class GetMenuByUserResponse
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string? CssClass { get; set; }
    public int SortOrder { set; get; }
    public int ParrentId { get; set; }
}
