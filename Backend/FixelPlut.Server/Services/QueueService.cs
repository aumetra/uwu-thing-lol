namespace FixelPlut.Server.Services;

public class QueueService : IQueueService
{
    private readonly ILogger<QueueService> logger;

    private const int s_take = 1000;
    private readonly List<string> workItemQueue = new();
    private int index;

    public readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    public QueueService(ILogger<QueueService> logger)
    {
        this.logger = logger;
    }

    internal void Add(string workItem)
        => workItemQueue.Add(workItem);

    internal int Length()
        => workItemQueue.Count;

    public string[] GetNext()
    {
        try
        {
            SemaphoreSlim.Wait();
            if (index > workItemQueue.Count - s_take)
                index = 0;
            string[] items;
            if (index + s_take > workItemQueue.Count)
                items = workItemQueue.Skip(index).ToArray();
            else
                items = workItemQueue.Skip(index).Take(s_take).ToArray();
            logger.LogInformation("Left {Length}", items.Length - index);
            index += s_take;
            return items;
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    internal void Shuffle()
    {
        var rng = new Random();
        int n = workItemQueue.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            var temp = workItemQueue[n];
            workItemQueue[n] = workItemQueue[k];
            workItemQueue[k] = temp;
        }
    }

    internal void Clear()
    {
        workItemQueue.Clear();
        index = 0;
    }

    internal void Add(IEnumerable<string> rect1)
    {
        workItemQueue.AddRange(rect1);
    }

    internal void Limit(int count)
    {
        if (workItemQueue.Count > count)
            workItemQueue.RemoveRange(0, count);
    }
}
