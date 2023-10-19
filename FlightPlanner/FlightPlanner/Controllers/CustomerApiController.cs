using FlightPlanner.Exceptions;
using FlightPlanner.Core.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Mvc;
using FlightPlanner.Core.Services;

namespace FlightPlanner.Controllers;

[Route("api")]
[ApiController]
public class CustomerApiController : ControllerBase
{
    private readonly IDbService _dbService;
    private readonly FlightStorage _storage;

    public CustomerApiController(FlightStorage storage)
    {
        _storage = storage;
    }

    [Route("airports")]
    [HttpGet]
    public IActionResult SearchAirports(string search)
    {
        return Ok(_dbService.SearchAirports(search));
    }

    [Route("flights/search")]
    [HttpPost]
    public IActionResult SearchFlights(SearchFlightRequest req)
    {
        try
        {
            _dbService.SearchFlights(req);
            return Ok(_dbService.SearchFlights(req));
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
            _dbService.GetById<Flight>(id);

            return Ok(_dbService.GetById<Flight>(id));
        }
        catch (InvalidFlightException)
        {
            return NotFound();
        }
    }
}