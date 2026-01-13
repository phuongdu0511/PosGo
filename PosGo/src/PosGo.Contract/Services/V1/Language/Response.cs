namespace PosGo.Contract.Services.V1.Language;

public static class Response
{
    public record LanguageResponse(
        string Code,
        string Name,
        bool IsActive
    );

    public record LanguageDetailResponse(
        string Code,
        string Name,
        bool IsActive
    );
}