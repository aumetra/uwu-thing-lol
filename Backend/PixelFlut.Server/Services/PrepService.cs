
using System.Text.Json;

namespace PixelFlut.Server.Services;

public class PrepService : IHostedService
{
    private readonly ILogger<PrepService> logger;
    private readonly IQueueService queueService;

    public PrepService(ILogger<PrepService> logger, IQueueService queueService)
    {
        this.logger = logger;
        this.queueService = queueService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var fs = new FileStream("exp.json", FileMode.Open, FileAccess.Read);
            var a = JsonSerializer.Deserialize<List<string>>(fs);


            var content = await File.ReadAllTextAsync("exp.json");
            var cmds = JsonSerializer.Deserialize<List<string>>(content);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
