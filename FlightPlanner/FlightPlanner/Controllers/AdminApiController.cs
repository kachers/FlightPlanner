using FlightPlanner.Exceptions;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers;

[Authorize]
[Route("admin-api/flights")]
[ApiController]
public class AdminApiController : ControllerBase
{
    private readonly IDbService _dbService;
    private static readonly object AddLock = new();
    private static readonly object DeleteLock = new();

    public AdminApiController(IDbService DbService)
    {
        _dbService = DbService;
    }

    [HttpPut]
    public IActionResult PutFlight(Flight flight)
    {
        lock (AddLock)
        {
            try
            {
                _dbService.Create(flight);
                return Created("", flight);
            }
            catch (DuplicateFlightException)
            {
                return Conflict();
            }
            catch (InvalidFlightException)
            {
                return BadRequest();
            }
        }
    }

    [Route("{id:int}")]
    [HttpGet]
    public IActionResult GetFlight(int id)
    {
        try
        {
            _dbService.GetById<Flight>(id);

            return Ok(_dbService.GetById<Flight>(id));
        }
        catch (InvalidFlightException)
        {
            return NotFound();
        }
    }

    [Route("{id:int}")]
    [HttpDelete]
    public IActionResult DeleteFlight(int id)
    {
        var flight = _dbService.GetById<Flight>(id);
        lock (DeleteLock)
        {
            _dbService.Delete(flight);
        }

        return Ok();
    }
}