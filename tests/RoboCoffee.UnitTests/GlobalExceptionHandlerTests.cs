using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using RoboCoffee.Api.Infrastructure;

namespace RoboCoffee.UnitTests;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task UnexpectedExceptionReturnsSanitizedProblemDetails()
    {
        var handler = new GlobalExceptionHandler(
            NullLogger<GlobalExceptionHandler>.Instance);
        var context = new DefaultHttpContext();
        context.Request.Path = "/test";
        context.Response.Body = new MemoryStream();

        var handled = await handler.TryHandleAsync(
            context,
            new InvalidOperationException("secret database detail"),
            CancellationToken.None);

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Contains("An unexpected error occurred", body);
        Assert.DoesNotContain("secret database detail", body);
    }
}
