using FlightPlanner.Exceptions;
using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers;

[Authorize]
[Route("admin-api/flights")]
[ApiController]
public class AdminApiController : ControllerBase
{
    private static readonly object AddLock = new();
    private static readonly object DeleteLock = new();
    private readonly FlightStorage _storage;

    public AdminApiController(FlightStorage storage)
    {
        _storage = storage;
    }

    [HttpPut]
    public IActionResult PutFlight(Flight flight)
    {
        lock (AddLock)
        {
            try
            {
                _storage.AddFlight(flight);
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
            _storage.GetFlight(id);

            return Ok(_storage.GetFlight(id));
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
        lock (DeleteLock)
        {
            _storage.DeleteFlight(id);
        }

        return Ok();
    }
}