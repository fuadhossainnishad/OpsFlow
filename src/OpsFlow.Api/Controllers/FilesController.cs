using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Application.Authorization;
using OpsFlow.Application.Features.Files.DeleteFile;
using OpsFlow.Application.Features.Files.GetFile;
using OpsFlow.Application.Features.Files.ListFiles;
using OpsFlow.Application.Features.Files.UploadFile;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/files")]
[Authorize]
public sealed class FilesController(
    UploadFileHandler uploadFileHandler,
    ListFilesHandler listFilesHandler,
    GetFileHandler getFileHandler,
    DeleteFileHandler deleteFileHandler) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = PermissionCodes.FilesCreate)]
    [RequestSizeLimit(26 * 1024 * 1024)]
    public async Task<ActionResult<UploadFileResult>> Upload(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null)
            return BadRequest("File is required.");

        await using var stream = file.OpenReadStream();

        var result = await uploadFileHandler.HandleAsync(
            stream,
            file.FileName,
            file.ContentType,
            file.Length,
            cancellationToken);

        return CreatedAtAction(
            nameof(Download),
            new { fileId = result.Id },
            result);
    }

    [HttpGet]
    [Authorize(Policy = PermissionCodes.FilesRead)]
    public async Task<ActionResult<ListFilesResult>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return Ok(await listFilesHandler.HandleAsync(
            new ListFilesQuery(page, pageSize),
            cancellationToken));
    }

    [HttpGet("{fileId:guid}")]
    [Authorize(Policy = PermissionCodes.FilesRead)]
    public async Task<IActionResult> Download(
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var result = await getFileHandler.HandleAsync(
            fileId,
            cancellationToken);

        return File(
            result.Content,
            result.ContentType,
            result.FileName,
            enableRangeProcessing: true);
    }

    [HttpDelete("{fileId:guid}")]
    [Authorize(Policy = PermissionCodes.FilesDelete)]
    public async Task<IActionResult> Delete(
        Guid fileId,
        CancellationToken cancellationToken)
    {
        await deleteFileHandler.HandleAsync(
            fileId,
            cancellationToken);

        return NoContent();
    }
}
