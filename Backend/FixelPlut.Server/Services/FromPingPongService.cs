
using FixelPlut.Shared.Models;
using System.Collections.Concurrent;
using System.Drawing;

namespace FixelPlut.Server.Services;

public class FromPingPongService : ILoaderService
{
    const int PlayerHeight = 125;
    const int PlayerWidth = 30;
    const int BallHeight = 25;
    const int BallWidth = 25;

    private readonly BlockingCollection<PingPongTick> pingPongTickQueue = new();
    private readonly QueueService queueService;
    private Task computeTicksTask;

    public FromPingPongService(IQueueService queueService)
    {
        this.queueService = (QueueService)queueService;
    }

    public void AddTick(PingPongTick tick)
        => pingPongTickQueue.Add(tick);

    private async Task ComputeTicks()
    {
        while (true)
        {
            try
            {
                // Dispose 4 game ticks if the queue has more then 5 ticks
                if (pingPongTickQueue.Count > 5)
                    _ = pingPongTickQueue.Take(4);

                // Get next tick
                var tick = pingPongTickQueue.Take();

                var rect1 = GenerateRectangle(tick.Player1X, tick.Player1Y, PlayerWidth, PlayerHeight, "ffff00");
                var rect2 = GenerateRectangle(tick.Player2X, tick.Player2Y, PlayerWidth, PlayerHeight, "ffff00");
                var ball = GenerateBall(tick.BallX, tick.BallY, BallWidth, BallHeight, "ffff00");

                queueService.SemaphoreSlim.Wait();
                try
                {
                    queueService.Clear();

                    //queueService.Add(background);
                    queueService.Add(rect1);
                    queueService.Add(rect2);
                    queueService.Add(ball);
                }
                finally
                {
                    queueService.SemaphoreSlim.Release();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        computeTicksTask = Task.Run(ComputeTicks);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    private IEnumerable<string> GenerateRectangle(int posX, int posY, int width, int height, string color)
    {
        const string black = "000000";
        const string white = "ffffff";

        var movementOffset = 30;
        var list = new List<string>();
        var minY = posY - movementOffset;
        var maxY = posY + height + movementOffset;
        for (int x = posX; x < posX + width; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                string clr = color;

                if (y < posY)
                {
                    clr = white;
                }
                else if (y > posY + height)
                {
                    clr = white;
                }
                else if (y == minY || y == maxY - 1)
                {
                    clr = black;
                }
                else if (x == posX || x == posX + width - 1)
                {
                    clr = black;
                }
                else if (x == (posX + width) / 2)
                {
                    clr = white;
                }


                list.Add(string.Format("PX {0} {1} {2}", x, y, clr));
            }
        }
        Shuffle(ref list);
        return list;
    }

    private IEnumerable<string> GenerateBall(int posX, int posY, int width, int height, string color)
    {
        var movementOffset = 30;

        var list = new List<string>();
        for (int x = posX; x < posX + width; x++)
        {
            for (int y = posY; y < posY + height; y++)
            {
                string clr = "000000";

                // check if coordinates are inside circle
                if (Math.Pow(x - posX, 2) + Math.Pow(y - posY, 2) <= Math.Pow(width / 2, 2))
                {
                    clr = color;
                }
                else
                {
                    continue;
                }

                list.Add(string.Format("PX {0} {1} {2}", x, y, clr));
                list.Add(string.Format("PX {0} {1} {2}", x + movementOffset, y + movementOffset, clr));
                list.Add(string.Format("PX {0} {1} {2}", x + movementOffset, y - movementOffset, clr));
                list.Add(string.Format("PX {0} {1} {2}", x - movementOffset, y - movementOffset, clr));
                list.Add(string.Format("PX {0} {1} {2}", x - movementOffset, y + movementOffset, clr));
            }
        }
        Shuffle(ref list);
        return list;
    }

    internal static void Shuffle(ref List<string> items)
    {
        var rng = new Random();
        int n = items.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (items[k], items[n]) = (items[n], items[k]);
        }
    }
}
