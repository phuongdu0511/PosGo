using Microsoft.AspNetCore.Http;
using PosGo.Application.Abstractions;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Media;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Media;

public sealed class GeneratePresignedUrlCommandHandler 
    : ICommandHandler<Command.GeneratePresignedUrlCommand, GeneratePresignedUrlResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepositoryBase<Domain.Entities.Media, int> _mediaRepository;

    public GeneratePresignedUrlCommandHandler(
        IS3Service s3Service,
        IHttpContextAccessor httpContextAccessor,
        IRepositoryBase<Domain.Entities.Media, int> mediaRepository)
    {
        _s3Service = s3Service;
        _httpContextAccessor = httpContextAccessor;
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<GeneratePresignedUrlResponse>> Handle(
        Command.GeneratePresignedUrlCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate file extension
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var fileExtension = Path.GetExtension(request.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return Result.Failure<GeneratePresignedUrlResponse>(
                new Error("INVALID_FILE_TYPE",
                    $"File type không được hỗ trợ. Chỉ chấp nhận: {string.Join(", ", allowedExtensions)}"));
        }

        // 2. Validate content type
        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowedContentTypes.Contains(request.ContentType.ToLowerInvariant()))
        {
            return Result.Failure<GeneratePresignedUrlResponse>(
                new Error("INVALID_CONTENT_TYPE",
                    $"Content type không được hỗ trợ. Chỉ chấp nhận: {string.Join(", ", allowedContentTypes)}"));
        }

        // 3. Lấy RestaurantId từ HttpContext
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext?.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure<GeneratePresignedUrlResponse>(
                new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // 4. Tạo imageKey: new Guid() (theo yêu cầu)
        var imageKey = Guid.NewGuid().ToString();

        // 5. Tạo pre-signed PUT URL
        var url = await _s3Service.GeneratePresignedPutUrlAsync(
            request.FileName,
            request.ContentType,
            imageKey,
            cancellationToken);

        // 7. Tạo Media record với FileSize = 0 (chưa upload)
        var folder = request.Folder ?? "images";
        var media = Domain.Entities.Media.Create(
            restaurantId.Value,
            imageKey,
            request.FileName,
            request.ContentType,
            0, // FileSize = 0 vì chưa upload
            folder,
            null, // Width
            null, // Height
            null  // Alt
        );

        _mediaRepository.Add(media);

        return Result.Success(new GeneratePresignedUrlResponse(
            ImageKey: imageKey,
            Url: url
        ));
    }
}
