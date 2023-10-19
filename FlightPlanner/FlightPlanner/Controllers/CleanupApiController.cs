using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Services;
using FlightPlanner.Storage;
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
        _dbService.DeleteRange<Flight>();
        _dbService.DeleteRange<Airport>();

        return Ok();
    }
}