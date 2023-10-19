using FlightPlanner.Core.Services;
using FlightPlanner.Services;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers;

[Route("testing-api")]
[ApiController]
public class CleanupApiController : ControllerBase
{
    private readonly FlightStorage _storage;
    private readonly IDbService _dbService;

    public CleanupApiController(FlightStorage storage, IDbService DbService)
    {
        _dbService = DbService;
        _storage = storage;
    }

    [Route("clear")]
    [HttpPost]
    public IActionResult Clear()
    {
        _storage.Clear();

        return Ok();
    }
}