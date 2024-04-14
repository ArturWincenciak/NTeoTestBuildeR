using System.Diagnostics;

namespace NTeoTestBuildeR.Infra.ErrorHandling;

internal sealed class ErrorHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(context, ex);
        }
    }

    private async static Task HandleErrorAsync(HttpContext context, Exception ex)
    {
        SetProblemDetailsContentType(context);
        var response = ErrorMapper.Map(ex);
        response.Extensions["traceId"] = Activity.Current?.Id!;
        context.Response.StatusCode = response.Status;
        await context.Response.WriteAsJsonAsync(response);
    }

    private static void SetProblemDetailsContentType(HttpContext context) =>
        context.Response.OnStarting(callback: state =>
        {
            var httpContext = (HttpContext) state;
            httpContext.Response.Headers.ContentType = "application/problem+json; charset=utf-8";
            return Task.CompletedTask;
        }, context);
}