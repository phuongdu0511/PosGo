using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Media;

public static class Query
{
    /// <summary>
    /// Lấy pre-signed GET URL để download ảnh (nếu cần)
    /// </summary>
    public record GetPresignedGetUrlQuery(
        string ImageKey
    ) : IQuery<GetPresignedGetUrlResponse>;
}

public record GetPresignedGetUrlResponse(
    string Url
);
