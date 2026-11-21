using Microsoft.AspNetCore.Mvc;
using RoboCoffee.Application.Integrations;
using RoboCoffee.Contracts.Integrations;

namespace RoboCoffee.Api.Controllers;

[ApiController]
[Route("api/addresses")]
public sealed class AddressesController : ControllerBase
{
    private readonly IAddressLookupService _addressLookup;

    public AddressesController(IAddressLookupService addressLookup)
    {
        _addressLookup = addressLookup;
    }

    [HttpGet("{postalCode}")]
    [ProducesResponseType<AddressLookupResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AddressLookupResponse>> Lookup(
        string postalCode,
        CancellationToken cancellationToken)
    {
        var digits = new string(postalCode.Where(char.IsDigit).ToArray());
        if (digits.Length != 8)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid postal code",
                Detail = "Postal code must contain exactly 8 digits.",
                Instance = HttpContext.Request.Path
            });
        }

        var address = await _addressLookup.LookupAsync(postalCode, cancellationToken);

        if (address is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Address not found",
                Detail = "The postal code could not be found or confirmed.",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(address);
    }
}
