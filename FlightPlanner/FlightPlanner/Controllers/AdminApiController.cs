using AutoMapper;
using FlightPlanner.Exceptions;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers;

[Authorize]
[Route("admin-api/flights")]
[ApiController]
public class AdminApiController : ControllerBase
{
    private readonly IDbService _dbService;
    private readonly IMapper _mapper;
    private static readonly object AddLock = new();
    private static readonly object DeleteLock = new();

    public AdminApiController(IDbService DbService, IMapper mapper)
    {
        _dbService = DbService;
        _mapper = mapper;
    }

    [HttpPut]
    public IActionResult PutFlight(FlightRequest request)
    {
        lock (AddLock)
        {
            try
            {
                if (string.IsNullOrEmpty(request.To.Country) ||
                    string.IsNullOrEmpty(request.To.City) ||
                    string.IsNullOrEmpty(request.To.Airport) ||
                    string.IsNullOrEmpty(request.From.Country) ||
                    string.IsNullOrEmpty(request.From.City) ||
                    string.IsNullOrEmpty(request.From.Airport) ||
                    string.IsNullOrEmpty(request.Carrier) ||
                    string.IsNullOrEmpty(request.DepartureTime) ||
                    string.IsNullOrEmpty(request.ArrivalTime))
                {
                    throw new InvalidFlightException();
                }

                if (request.From.Airport.ToLower().Trim().Equals(request.To.Airport.ToLower().Trim()) &&
                    request.From.City.ToLower().Trim().Equals(request.To.City.ToLower().Trim()) &&
                    request.From.Country.ToLower().Trim().Equals(request.To.Country.ToLower().Trim()))
                {
                    throw new InvalidFlightException();
                }

                if (DateTime.Parse(request.ArrivalTime).CompareTo(DateTime.Parse(request.DepartureTime)) <= 0)
                {
                    throw new InvalidFlightException();
                }

                var duplicate = _dbService.Query<Flight>()
                        .Include(f => f.From)
                        .Include(f => f.To)
                        .Where(f=>
                            f.To.AirportCode.Equals(request.To.Airport) && 
                            f.From.AirportCode.Equals(request.From.Airport) &&
                            f.DepartureTime.Equals(request.DepartureTime))
                        .ToList();

                if (duplicate.Count > 0)
                {
                    throw new DuplicateFlightException();
                }

                var flight = _mapper.Map<Flight>(request);
                _dbService.Create(flight);
                request = _mapper.Map<FlightRequest>(flight);
                return Created("", request);
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
        lock (AddLock)
        {
            if (_dbService.GetById<Flight>(id) == null) return NotFound();

            return Ok(_dbService.GetById<Flight>(id));
        }
    }

    [Route("{id:int}")]
    [HttpDelete]
    public IActionResult DeleteFlight(int id)
    {
        var flight = _dbService.GetById<Flight>(id);

        if (flight == null) return Ok();
        lock (DeleteLock) 
        { 
            _dbService.Delete(flight);
        }

        return Ok();
    }
}