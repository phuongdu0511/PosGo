using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using PosGo.Application.Abstractions;
using PosGo.Infrastructure.DependencyInjection.Options;

namespace PosGo.Infrastructure.Services;

public class S3Service : IS3Service
{
    private readonly S3Option s3Option = new S3Option();
    private readonly IAmazonS3 _s3Client;
    public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        configuration.GetSection(nameof(S3Option)).Bind(s3Option);
    }

    public async Task<string> GeneratePresignedPutUrlAsync(
        string fileName,
        string contentType,
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = s3Option.BucketName,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.AddMinutes(s3Option.ExpireMin),
            Metadata =
            {
                ["file-name"] = fileName,
            }
        };

        return await Task.FromResult(_s3Client.GetPreSignedURL(request));
    }

    public async Task<string> GeneratePresignedGetUrlAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = s3Option.BucketName,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(s3Option.ExpireMin)
        };

        return await Task.FromResult(_s3Client.GetPreSignedURL(request));
    }

    public async Task DeleteObjectAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        await _s3Client.DeleteObjectAsync(s3Option.BucketName, objectKey, cancellationToken);
    }

    public async Task<bool> ObjectExistsAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _s3Client.GetObjectMetadataAsync(s3Option.BucketName, objectKey, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
