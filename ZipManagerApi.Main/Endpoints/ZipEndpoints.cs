using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ZipManagerApi.Domain;
using ZipManagerApi.Main.Abstractions;

namespace ZipManagerApi.Main.Endpoints;

public static class ZipEndpoints
{
    public static IEndpointRouteBuilder UseZipEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("zip");
        group.MapGet("files", GetFiles);
        group.MapPost("", ZipFiles);
        group.MapGet("status", GetZipStatus);
        group.MapGet("", GetArchive);
        return builder;
    }

    public static Ok<List<string>> GetFiles(IZipService zipService)
    {
        return TypedResults.Ok(zipService.GetAllFiles());
    }

    public static Ok<Guid> ZipFiles([FromBody] List<string> fileNames, IZipService zipService)
    {
        return TypedResults.Ok(zipService.ZipFiles(fileNames));
    }

    public static Results<FileStreamHttpResult, NotFound> GetArchive(
        [FromQuery] Guid id,
        IZipService zipService
    )
    {
        var stream = zipService.GetArchive(id);
        if (stream == null)
            return TypedResults.NotFound();
        return TypedResults.File(stream, "application/zip", Path.GetFileName(stream.Name));
    }

    public static Results<Ok<ZipStatus>, NotFound> GetZipStatus(
        [FromQuery] Guid id,
        IZipService zipService
    )
    {
        var progress = zipService.GetProgress(id);
        if (!progress.HasValue)
            return TypedResults.NotFound();
        return TypedResults.Ok(progress.Value);
    }
}
