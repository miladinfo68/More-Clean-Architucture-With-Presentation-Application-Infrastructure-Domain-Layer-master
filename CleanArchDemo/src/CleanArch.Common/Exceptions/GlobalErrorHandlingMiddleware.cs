using CleanArch.Common.Formater;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace CleanArch.Common.Exceptions;

public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public GlobalErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);

            //if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
            //{
            //    throw new NotFoundException();
            //}
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode = (int)GetStatusCode(exception);
        var response = ApiResponse<object>.ErrorResponse(statusCode, exception?.Message ?? "xxxx Bloody Error xxxx");
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
        context.Response.StatusCode = response.StatusCode;
        context.Response.ContentType = "application/json";
        
        return context.Response.WriteAsync(jsonResponse);
    }

    private static HttpStatusCode GetStatusCode(Exception ex)
    {
        var statusCode = ex switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            SqlServerException => HttpStatusCode.InternalServerError,
            RequestTimeoutException => HttpStatusCode.RequestTimeout,
            BadRequestException => HttpStatusCode.BadRequest,
            AuthorizationException => HttpStatusCode.Unauthorized,
            NetworkException => HttpStatusCode.ServiceUnavailable,
            AccessDenyException => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };
        
        return statusCode;
    }

}