using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers;

[Route("testing-api")]
[ApiController]
public class CleanupApiController : ControllerBase
{
    private readonly IDbService _dbService;

    public CleanupApiController(IDbService DbService)
    {
        _dbService = DbService;
    }

    [Route("clear")]
    [HttpPost]
    public IActionResult Clear()
    {
        var flights = _dbService.Query<Flight>().ToList();
        foreach (var flight in flights)
        {
            flight.From = null;
            flight.To = null;
            _dbService.Update(flight);
        }
        _dbService.DeleteRange<Flight>();
        _dbService.DeleteRange<Airport>();

        return Ok();
    }
}