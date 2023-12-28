using FixelPlut.Shared.Models;
using System.Collections.Concurrent;
using System.Linq;

namespace FixelPlut.Server.Services;

public class QueueService : IQueueService
{
    private const int s_take = 10000;
    private int index = 0;
    private readonly List<string> workItemQueue = new();

    internal void Add(string workItem)
        => workItemQueue.Add(workItem);

    internal int Length()
        => workItemQueue.Count;

    public string[] GetNext()
    {
        if (index > workItemQueue.Count - s_take)
            index = 0;
        var a = workItemQueue.Skip(index).Take(s_take).ToArray();
        index += s_take;
        return a;
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
}
