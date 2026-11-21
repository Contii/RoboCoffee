using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Polly.CircuitBreaker;
using Polly.Timeout;
using RoboCoffee.Application.Integrations;
using RoboCoffee.Contracts.Integrations;

namespace RoboCoffee.Infrastructure.Integrations;

public sealed class ViaCepAddressLookupService : IAddressLookupService
{
    private readonly HttpClient _httpClient;

    public ViaCepAddressLookupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AddressLookupResponse?> LookupAsync(
        string postalCode,
        CancellationToken cancellationToken)
    {
        var normalizedPostalCode = NormalizePostalCode(postalCode);
        if (normalizedPostalCode is null)
        {
            return null;
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<ViaCepResponse>(
                $"{normalizedPostalCode}/json/",
                cancellationToken);

            if (response is null || response.HasError)
            {
                return null;
            }

            return new AddressLookupResponse(
                NormalizePostalCode(response.PostalCode) ?? normalizedPostalCode,
                response.Street ?? string.Empty,
                response.Complement,
                response.Neighborhood ?? string.Empty,
                response.City ?? string.Empty,
                response.State ?? string.Empty,
                IsConfirmed: true);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
        catch (TimeoutRejectedException)
        {
            return null;
        }
        catch (BrokenCircuitException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? NormalizePostalCode(string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
        {
            return null;
        }

        var normalized = new string(postalCode.Where(char.IsDigit).ToArray());
        return normalized.Length == 8 ? normalized : null;
    }

    private sealed class ViaCepResponse
    {
        [JsonPropertyName("cep")]
        public string? PostalCode { get; init; }

        [JsonPropertyName("logradouro")]
        public string? Street { get; init; }

        [JsonPropertyName("complemento")]
        public string? Complement { get; init; }

        [JsonPropertyName("bairro")]
        public string? Neighborhood { get; init; }

        [JsonPropertyName("localidade")]
        public string? City { get; init; }

        [JsonPropertyName("uf")]
        public string? State { get; init; }

        [JsonPropertyName("erro")]
        public bool HasError { get; init; }
    }
}
