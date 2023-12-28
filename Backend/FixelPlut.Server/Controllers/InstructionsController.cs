using FixelPlut.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace FixelPlut.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class InstructionsController : ControllerBase
{
    private readonly ILogger<InstructionsController> logger;
    private readonly IQueueService queueService;

    public InstructionsController(ILogger<InstructionsController> logger, IQueueService queueService)
    {
        this.logger = logger;
        this.queueService = queueService;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string[]))]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public IActionResult GetNextInstruction()
    {
        try
        {
            return Ok(queueService.GetNext());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while sending next instructions!");
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}
