using CleanArch.Common.Exceptions;

namespace CleanArch.WebAPI.ExtensionMethods;

public static class ExceptionHandlerExtension
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
       => app.UseMiddleware<GlobalErrorHandlingMiddleware>();
}
