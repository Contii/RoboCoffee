using System.Net;
using System.Text;
using RoboCoffee.Application.Integrations;
using RoboCoffee.Infrastructure.Integrations;
using Polly.CircuitBreaker;

namespace RoboCoffee.UnitTests;

public sealed class AddressLookupServiceTests
{
    [Fact]
    public async Task ViaCepResponseIsMappedToApplicationAddress()
    {
        var handler = new StubHttpMessageHandler(
            HttpStatusCode.OK,
            """
            {
              "cep": "01001-000",
              "logradouro": "Praca da Se",
              "complemento": "lado impar",
              "bairro": "Se",
              "localidade": "Sao Paulo",
              "uf": "SP"
            }
            """);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://viacep.test/ws/")
        };
        IAddressLookupService service = new ViaCepAddressLookupService(httpClient);

        var address = await service.LookupAsync("01001-000", CancellationToken.None);

        Assert.NotNull(address);
        Assert.Equal("01001000", address.PostalCode);
        Assert.Equal("Praca da Se", address.Street);
        Assert.Equal("Se", address.Neighborhood);
        Assert.Equal("Sao Paulo", address.City);
        Assert.Equal("SP", address.State);
        Assert.True(address.IsConfirmed);
    }

    [Fact]
    public async Task ViaCepErrorResponseReturnsNull()
    {
        var handler = new StubHttpMessageHandler(
            HttpStatusCode.OK,
            """{"erro": true}""");
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://viacep.test/ws/")
        };
        IAddressLookupService service = new ViaCepAddressLookupService(httpClient);

        var address = await service.LookupAsync("00000000", CancellationToken.None);

        Assert.Null(address);
    }

    [Fact]
    public async Task ViaCepHttpFailureReturnsNull()
    {
        var handler = new StubHttpMessageHandler(
            HttpStatusCode.ServiceUnavailable,
            "");
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://viacep.test/ws/")
        };
        IAddressLookupService service = new ViaCepAddressLookupService(httpClient);

        var address = await service.LookupAsync("01001000", CancellationToken.None);

        Assert.Null(address);
    }

    [Fact]
    public async Task RetryPolicyAttemptsThreeTimesOnTransientFailure()
    {
        var attempts = 0;
        var policy = AddressLookupResiliencePolicies.CreateRetryPolicy();

        await policy.ExecuteAsync(() =>
        {
            attempts++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        });

        Assert.Equal(3, attempts);
    }

    [Fact]
    public async Task CircuitBreakerOpensAfterFiveTransientFailures()
    {
        var policy = AddressLookupResiliencePolicies.CreateCircuitBreakerPolicy();

        for (var attempt = 0; attempt < 5; attempt++)
        {
            await policy.ExecuteAsync(() =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));
        }

        await Assert.ThrowsAsync<BrokenCircuitException<HttpResponseMessage>>(() =>
            policy.ExecuteAsync(() =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));
    }

    private sealed class StubHttpMessageHandler(
        HttpStatusCode statusCode,
        string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
        }
    }
}
