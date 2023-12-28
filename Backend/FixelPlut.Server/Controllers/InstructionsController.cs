using FixelPlut.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FixelPlut.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class InstructionsController : ControllerBase
{
    private readonly IQueueService queueService;

    public InstructionsController(IQueueService queueService)
    {
        this.queueService = queueService;
    }

    static InstructionsController()
    {
        //Instructions = System.IO.File.ReadAllLines(@"1.txt");
    }

    [HttpGet]
    public Task<string[]> GetNextInstruction()
    {
        return Task.FromResult(queueService.GetNext());
    }
}
