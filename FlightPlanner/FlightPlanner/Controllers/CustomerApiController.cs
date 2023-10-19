using FlightPlanner.Exceptions;
using FlightPlanner.Core.Models;
using Microsoft.AspNetCore.Mvc;
using FlightPlanner.Core.Services;

namespace FlightPlanner.Controllers;

[Route("api")]
[ApiController]
public class CustomerApiController : ControllerBase
{
    private readonly IDbService _dbService;

    public CustomerApiController(IDbService DbService)
    {
        _dbService = DbService;
    }

    [Route("airports")]
    [HttpGet]
    public IActionResult SearchAirports(string search)
    {
        search = search.Trim();
        var searchTerm = search.ToLower();
        var allAirports = _dbService.Get<Airport>().ToList();

        var result = allAirports
            .Where(airport =>
                airport.AirportCode.ToLower().Contains(searchTerm) ||
                airport.City.ToLower().Contains(searchTerm) ||
                airport.Country.ToLower().Contains(searchTerm))
            .ToList();
        return Ok(result);
    }

    [Route("flights/search")]
    [HttpPost]
    public IActionResult SearchFlights(SearchFlightRequest req)
    {
        try
        {
            var flights = _dbService.Get<Flight>().Where(flight =>
                flight.From.AirportCode.Equals(req.From) &&
                flight.To.AirportCode.Equals(req.To) &&
                flight.DepartureTime.Contains(req.DepartureDate)).ToList();

            List<Flight> result = new();
            result.AddRange(flights);

            PageResult pageResult = new PageResult
            {
                Page = (int)Math.Floor((decimal)result.Count / 100),
                TotalItems = result.Count,
                Items = result
            };

            return Ok(pageResult);
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