using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoboCoffee.Application.Integrations;
using RoboCoffee.Contracts.Integrations;
using RoboCoffee.Infrastructure.Persistence;

namespace RoboCoffee.IntegrationTests;

public sealed class AddressLookupTests : IAsyncLifetime
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
                    services.RemoveAll<IAddressLookupService>();

                    services.AddDbContext<RoboCoffeeDbContext>(options =>
                        options.UseInMemoryDatabase("RoboCoffeeAddressTests"));
                    services.AddSingleton<IAddressLookupService, StubAddressLookupService>();
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
    public async Task LookupReturnsConfirmedAddress()
    {
        var response = await _client.GetAsync("/api/addresses/01001-000");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var address = await response.Content.ReadFromJsonAsync<AddressLookupResponse>();
        Assert.NotNull(address);
        Assert.True(address.IsConfirmed);
        Assert.Equal("01001000", address.PostalCode);
        Assert.Equal("Sao Paulo", address.City);
    }

    [Fact]
    public async Task LookupInvalidPostalCodeReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/addresses/123");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LookupUnknownPostalCodeReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/addresses/00000000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class StubAddressLookupService : IAddressLookupService
    {
        public Task<AddressLookupResponse?> LookupAsync(
            string postalCode,
            CancellationToken cancellationToken)
        {
            var normalized = new string(postalCode.Where(char.IsDigit).ToArray());
            if (normalized == "00000000")
            {
                return Task.FromResult<AddressLookupResponse?>(null);
            }

            return Task.FromResult<AddressLookupResponse?>(new AddressLookupResponse(
                "01001000",
                "Praca da Se",
                "lado impar",
                "Se",
                "Sao Paulo",
                "SP",
                true));
        }
    }
}
