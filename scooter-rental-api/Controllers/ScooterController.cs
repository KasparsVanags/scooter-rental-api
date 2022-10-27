using Microsoft.AspNetCore.Mvc;
using scooter_rental.Core.Interfaces;
using scooter_rental.Core.Models;
using scooter_rental.Core.Services;

namespace scooter_rental_api.Controllers;

[Route("api/scooter")]
[ApiController]
public class ScooterController : ControllerBase
{
    private readonly IEntityService<Scooter> _entityService;
    private readonly IEnumerable<IScooterValidator> _scooterValidator;

    public ScooterController(IEntityService<Scooter> entityService, IEnumerable<IScooterValidator> scooterValidator)
    {
        _entityService = entityService;
        _scooterValidator = scooterValidator;
    }

    [HttpPost]
    [Route("add")]
    public IActionResult AddScooter(Scooter scooter)
    {
        if (_scooterValidator.Any(x => !x.IsValid(scooter))) return BadRequest();

        if (_entityService.GetById(scooter.Id) != null) return Conflict(scooter.Id);
        
        var result = _entityService.Create(scooter);
        
        return Created("", scooter);
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public IActionResult DeleteScooter(string id)
    {
        var scooter = _entityService.GetById(id);

        if (scooter == null) return NotFound();
        
        _entityService.Delete(scooter);

        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetScooter(string id)
    {
        var scooter = _entityService.GetById(id);

        if (scooter == null) return NotFound($"Scooter id {id} does not exist");

        return Ok(scooter);
    }

    [HttpGet]
    [Route("list")]
    public IActionResult GetAllScooters()
    {
        var scooters = _entityService.GetAll();

        if (!scooters.Any()) return NoContent();
        
        return Ok(scooters);
    }
}