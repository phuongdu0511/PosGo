namespace PosGo.Application.Abstractions;

public interface IS3Service
{
    /// <summary>
    /// Tạo pre-signed PUT URL để upload file lên S3
    /// </summary>
    Task<string> GeneratePresignedPutUrlAsync(
        string objectKey,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo pre-signed GET URL để download file từ S3
    /// </summary>
    Task<string> GeneratePresignedGetUrlAsync(
        string objectKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Xóa file từ S3
    /// </summary>
    Task DeleteObjectAsync(
        string objectKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra file có tồn tại trong S3 không
    /// </summary>
    Task<bool> ObjectExistsAsync(
        string objectKey,
        CancellationToken cancellationToken = default);
}
