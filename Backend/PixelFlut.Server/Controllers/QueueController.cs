using Microsoft.AspNetCore.Mvc;
using PixelFlut.Server.Services;
using PixelFlut.Shared;

namespace PixelFlut.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class QueueController : ControllerBase
{
    private readonly QueueService prepService;
    private readonly ILogger<QueueController> logger;   

    public QueueController(QueueService prepService, ILogger<QueueController> logger)
    {
        this.prepService = prepService;
        this.logger = logger;
    }

    [HttpGet]
    public Task<QueueItem?> GetNext()
    {
        return prepService.NextAsync();
    }
}
