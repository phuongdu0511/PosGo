using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Application.Abstractions;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Media;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Queries.Media;

public sealed class GetPresignedGetUrlQueryHandler 
    : IQueryHandler<Query.GetPresignedGetUrlQuery, GetPresignedGetUrlResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepositoryBase<Domain.Entities.Media, int> _mediaRepository;

    public GetPresignedGetUrlQueryHandler(
        IS3Service s3Service,
        IHttpContextAccessor httpContextAccessor,
        IRepositoryBase<Domain.Entities.Media, int> mediaRepository)
    {
        _s3Service = s3Service;
        _httpContextAccessor = httpContextAccessor;
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<GetPresignedGetUrlResponse>> Handle(
        Query.GetPresignedGetUrlQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Lấy RestaurantId từ HttpContext
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext?.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure<GetPresignedGetUrlResponse>(
                new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // 2. Kiểm tra Media có tồn tại và thuộc về restaurant này không
        var media = await _mediaRepository.FindAll(
            m => m.ImageKey == request.ImageKey && m.RestaurantId == restaurantId.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (media == null)
        {
            return Result.Failure<GetPresignedGetUrlResponse>(
                new Error("MEDIA_NOT_FOUND", "Không tìm thấy media với imageKey này."));
        }

        // 3. Tạo pre-signed GET URL
        var url = await _s3Service.GeneratePresignedGetUrlAsync(
            request.ImageKey,
            cancellationToken);

        return Result.Success(new GetPresignedGetUrlResponse(
            Url: url
        ));
    }
}
