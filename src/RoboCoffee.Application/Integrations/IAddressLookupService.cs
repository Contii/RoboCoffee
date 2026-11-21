using RoboCoffee.Contracts.Integrations;

namespace RoboCoffee.Application.Integrations;

public interface IAddressLookupService
{
    Task<AddressLookupResponse?> LookupAsync(
        string postalCode,
        CancellationToken cancellationToken);
}
