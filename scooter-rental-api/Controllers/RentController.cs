using Microsoft.AspNetCore.Mvc;
using scooter_rental.Services;

namespace scooter_rental_api.Controllers;

[Route("api")]
[ApiController]
public class RentController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpPost]
    [Route("rent/{scooterId}")]
    public IActionResult StartRent(string scooterId, DateTime time)
    {
        var result = _rentalService.StartRent(scooterId, time);

        if (!result.Success) return BadRequest(result.FormattedErrors);

        return Ok(result.Entity);
    }

    [HttpPost]
    [Route("endRent/{scooterId}")]
    public IActionResult EndRent(string scooterId, DateTime time)
    {
        var result = _rentalService.EndRent(scooterId, time);

        if (!result.Success) return BadRequest(result.FormattedErrors);

        return Ok(result.Entity);
    }

    [HttpGet]
    [Route("getIncome")]
    public IActionResult GetIncome(int? year, bool includeIncompleteRentals, DateTime? currentTime)
    {
        if (includeIncompleteRentals && currentTime == null)
            return BadRequest("Current time cannot be null when requesting income from incomplete rentals");

        if (currentTime != null && currentTime.Value.Year < year) 
            return BadRequest("Current year cannot be before year of report");

        return Ok(_rentalService.GetIncome(year, includeIncompleteRentals, currentTime));
    }
}