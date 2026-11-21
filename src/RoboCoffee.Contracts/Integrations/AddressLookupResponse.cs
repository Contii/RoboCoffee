namespace RoboCoffee.Contracts.Integrations;

public sealed record AddressLookupResponse(
    string PostalCode,
    string Street,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    bool IsConfirmed);
