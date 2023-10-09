using FlightPlanner.Storage;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers;

[Route("testing-api")]
[ApiController]
public class CleanupApiController : ControllerBase
{
    private readonly FlightStorage _storage;

    public CleanupApiController(FlightStorage storage)
    {
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