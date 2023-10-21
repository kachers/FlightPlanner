using AutoMapper;
using FlightPlanner.Exceptions;
using FlightPlanner.Core.Models;
using Microsoft.AspNetCore.Mvc;
using FlightPlanner.Core.Services;
using FlightPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers;

[Route("api")]
[ApiController]
public class CustomerApiController : ControllerBase
{
    private readonly IDbService _dbService;
    private readonly IMapper _mapper;

    public CustomerApiController(IDbService DbService, IMapper mapper)
    {
        _dbService = DbService;
        _mapper = mapper;
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
        
        return Ok(result.Select(airport => _mapper.Map<AirportRequest>(airport)).ToList());
    }

    [Route("flights/search")]
    [HttpPost]
    public IActionResult SearchFlights(SearchFlightRequest req)
    {
        try
        {
            if (string.IsNullOrEmpty(req.From) ||
                string.IsNullOrEmpty(req.To) ||
                string.IsNullOrEmpty(req.DepartureDate))
            {
                throw new InvalidFlightException();
            }

            if (req.From.ToLower().Trim().Equals(req.To.ToLower().Trim()))
            {
                throw new InvalidFlightException();
            }

            var flights = _dbService.Query<Flight>()
                .Include(f => f.From)
                .Include(f => f.To)
                .Where(flight =>
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
            var flight = _dbService.GetById<Flight>(id) ?? throw new InvalidFlightException();
            return Ok(_mapper.Map<FlightRequest>(flight));
        }
        catch (InvalidFlightException)
        {
            return NotFound();
        }
    }
}