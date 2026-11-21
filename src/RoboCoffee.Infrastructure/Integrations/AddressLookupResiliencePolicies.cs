using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace RoboCoffee.Infrastructure.Integrations;

public static class AddressLookupResiliencePolicies
{
    public static IAsyncPolicy<HttpResponseMessage> CreateTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(5),
            TimeoutStrategy.Pessimistic);
    }

    public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                2,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1)));
    }

    public static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .CircuitBreakerAsync(
                5,
                TimeSpan.FromSeconds(30));
    }
}
