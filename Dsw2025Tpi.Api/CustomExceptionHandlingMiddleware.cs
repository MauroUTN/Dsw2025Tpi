using System.Net;
using System.Text.Json;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api;
public class CustomExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;
    public CustomExceptionHandlingMiddleware(ILogger<CustomExceptionHandlingMiddleware> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            context.Response.ContentType = "application/json";

            var statusCode = e switch
            {
                EntityNotFoundException => HttpStatusCode.NotFound,
                ArgumentOutOfRangeException => HttpStatusCode.BadRequest,
                ArgumentNullException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                DuplicatedEntityException => HttpStatusCode.BadRequest,
                InvalidStatusException => HttpStatusCode.BadRequest,
                NotAuthenticatedException => HttpStatusCode.BadRequest,
                NotFoundException => HttpStatusCode.NotFound,
                NoContentException => HttpStatusCode.NoContent, 
                InvalidOperationException => HttpStatusCode.BadRequest,
                Dsw2025Tpi.Application.Exceptions.ApplicationException => HttpStatusCode.BadRequest,
                System.ApplicationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                status = (int)statusCode,
                title = statusCode.ToString(),
                detail = e.Message
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}
