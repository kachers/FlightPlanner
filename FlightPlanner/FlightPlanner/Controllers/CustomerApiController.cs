using FlightPlanner.Exceptions;
using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers;

[Route("api")]
[ApiController]
public class CustomerApiController : ControllerBase
{
    private readonly FlightStorage _storage;

    public CustomerApiController()
    {
        _storage = new FlightStorage();
    }

    [Route("airports")]
    [HttpGet]
    public IActionResult SearchAirports(string search)
    {
        return Ok(_storage.SearchAirports(search));
    }

    [Route("flights/search")]
    [HttpPost]
    public IActionResult SearchFlights(SearchFlightRequest req)
    {
        try
        {
            _storage.SearchFlights(req);
            return Ok(_storage.SearchFlights(req));
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

    [Route("flights/{id:int}")]
    [HttpGet]
    public IActionResult FindFlightsById(int id)
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
}