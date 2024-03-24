using System.Net;
using System.Text.Json;
using Common.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Common.Api;

public class ExceptionsHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionsHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next.Invoke(httpContext);
        }
        catch (Exception e)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var result = string.Empty;
            switch (e)
            {
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(notFoundException.Message);
                    break;
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(badRequestException.Message);
                    break;
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(validationException.Message);
                    break;
                case ForbiddenException forbiddenException:
                    statusCode = HttpStatusCode.Forbidden;
                    result = JsonSerializer.Serialize(forbiddenException.Message);
                    break;
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                result = JsonSerializer.Serialize(new { error = e.Message, innerMessage = e.InnerException?.Message, e.StackTrace });
            }

            httpContext.Response.StatusCode = (int)statusCode;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(result);
        }
    }
}