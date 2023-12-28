using FixelPlut.Shared.Models;
using System.Collections.Concurrent;
using System.Linq;

namespace FixelPlut.Server.Services;

public class QueueService : IQueueService
{
    private readonly ILogger<QueueService> logger;

    private const int s_take = 10000;
    private int index = 0;
    private readonly List<string> workItemQueue = new();

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
        if (index > workItemQueue.Count - s_take)
            index = 0;
        var items = workItemQueue.Skip(index).Take(s_take).ToArray();
        logger.LogInformation("Left {Length}", items.Length);
        index += s_take;
        return items;
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
    }

    internal void Add(IEnumerable<string> rect1)
    {
        workItemQueue.AddRange(rect1);
    }

    internal void Limit(int count)
    {
        if(workItemQueue.Count > count)
            workItemQueue.RemoveRange(0, count);
    }
}
