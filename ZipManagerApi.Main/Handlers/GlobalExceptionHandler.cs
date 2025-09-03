using System;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ZipManagerApi.Domain.Exceptions;

namespace ZipManagerApi.Main.Handlers;

public class GlobalExceptionHandler(IProblemDetailsService _detailsService) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        HttpStatusCode status = exception switch
        {
            AppException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError,
        };
        httpContext.Response.StatusCode = (int)status;
        return _detailsService.TryWriteAsync(
            new ProblemDetailsContext()
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails()
                {
                    Type = exception.GetType().Name,
                    Title = "An error occurred",
                    Detail = exception is AppException ? exception.Message : "Inner exception",
                },
            }
        );
    }
}
