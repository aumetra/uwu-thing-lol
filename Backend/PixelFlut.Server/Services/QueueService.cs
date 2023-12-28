using PixelFlut.Shared;

namespace PixelFlut.Server.Services;

public interface IQueueService
{
    Task<QueueItem?> NextAsync();
}

public class QueueService : IQueueService
{
    private readonly ILogger<QueueService> logger;
    private readonly SemaphoreSlim semaphore = new(1, 1);

    public QueueService(ILogger<QueueService> logger)
    {
        this.logger = logger;
    }

    private int index = 0;
    private QueueItem[] items;

    public async Task<QueueItem?> NextAsync()
    {
        await semaphore.WaitAsync();
        try
        {
            if (index >= items.Length)
                index = 0;

            return items[index++];
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error in " + nameof(NextAsync));
            return null;
        }
        finally
        {
            semaphore.Release();
        }
    }
}
