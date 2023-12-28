using FixelPlut.Server.Services;
using FixelPlut.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FixelPlut.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PingPongController : ControllerBase
{
    private readonly ILogger<PingPongController> logger;
    private readonly FromPingPongService fromPingPongService;

    public PingPongController(FromPingPongService fromPingPongService, ILogger<PingPongController> logger)
    {
        this.fromPingPongService = fromPingPongService;
        this.logger = logger;
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public IActionResult Tick([FromBody] PingPongTick tick)
    {
        try
        {

            fromPingPongService.AddTick(tick);
            //logger.LogInformation("Recieved tick");
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while recieving tick!");
            return new StatusCodeResult(500);
        }
    }
}