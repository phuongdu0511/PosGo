using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Media;
using PosGo.Presentation.Abstractions;

namespace PosGo.Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class MediaController : ApiController
{
    public MediaController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Tạo pre-signed URL để upload ảnh lên S3
    /// </summary>
    [HttpPost("presigned-url")]
    [ProducesResponseType(typeof(Result<GeneratePresignedUrlResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GeneratePresignedUrl(
        [FromBody] Command.GeneratePresignedUrlCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    /// <summary>
    /// Lấy pre-signed GET URL để download ảnh
    /// </summary>
    [HttpGet("presigned-url/{imageKey}")]
    [ProducesResponseType(typeof(Result<GetPresignedGetUrlResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPresignedGetUrl(string imageKey)
    {
        var result = await Sender.Send(new Query.GetPresignedGetUrlQuery(imageKey));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
