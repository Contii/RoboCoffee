using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoboCoffee.Api.Middleware;
using RoboCoffee.Infrastructure.Persistence;

namespace RoboCoffee.IntegrationTests;

public sealed class CorrelationIdTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<RoboCoffeeDbContext>();
                    services.RemoveAll<DbContextOptions<RoboCoffeeDbContext>>();
                    services.RemoveAll<IDbContextOptionsConfiguration<RoboCoffeeDbContext>>();

                    services.AddDbContext<RoboCoffeeDbContext>(options =>
                        options.UseInMemoryDatabase("RoboCoffeeCorrelationTests"));
                });
            });

        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task MissingCorrelationIdCreatesResponseHeader()
    {
        var response = await _client.GetAsync("/health");

        Assert.True(response.Headers.TryGetValues(
            CorrelationIdMiddleware.HeaderName,
            out var values));

        var correlationId = Assert.Single(values);
        Assert.True(Guid.TryParse(correlationId, out _));
    }

    [Fact]
    public async Task ProvidedCorrelationIdIsReturned()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.TryAddWithoutValidation(
            CorrelationIdMiddleware.HeaderName,
            "manual-correlation-123");

        var response = await _client.SendAsync(request);

        Assert.True(response.Headers.TryGetValues(
            CorrelationIdMiddleware.HeaderName,
            out var values));
        Assert.Equal("manual-correlation-123", Assert.Single(values));
    }

    [Fact]
    public async Task OversizedCorrelationIdIsReplaced()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.TryAddWithoutValidation(
            CorrelationIdMiddleware.HeaderName,
            new string('x', 129));

        var response = await _client.SendAsync(request);

        Assert.True(response.Headers.TryGetValues(
            CorrelationIdMiddleware.HeaderName,
            out var values));
        Assert.True(Guid.TryParse(Assert.Single(values), out _));
    }
}
