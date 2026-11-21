using Serilog.Context;

namespace RoboCoffee.Api.Middleware;

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";
    public const string ItemName = "RoboCoffee.CorrelationId";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId) || correlationId.Length > 128)
        {
            correlationId = Guid.NewGuid().ToString();
        }
        else
        {
            correlationId = correlationId.Trim();
        }

        context.Items[ItemName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
