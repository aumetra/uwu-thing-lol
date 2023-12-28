using FixelPlut.Shared.Models;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text;

namespace FixelPlut.Client;

internal class InstructionService
{
    internal static readonly InstructionService Instance = new();

    private readonly HttpClient client;
    private readonly BlockingCollection<byte[][]> instructionQueue = new();
    private readonly Task refreshTask;

    public InstructionService()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("http://151.217.2.77:5000"),
        };
        refreshTask = Task.Run(RefreshTask);
    }

    private async Task RefreshTask()
    {
        //var a = await client.PostAsJsonAsync(
        //    "PingPong",
        //    new PingPongTick
        //    {
        //        BallX = 100,
        //        BallY = 100,
        //        Player1X = 100,
        //        Player1Y = 100,
        //        Player2X = 100,
        //        Player2Y = 100,
        //    });

        while (true)
        {
            try
            {
                if (instructionQueue.Count > 3)
                {
                    await Task.Delay(50);
                    continue;
                }

                var content = await client.GetFromJsonAsync<string[]>("Instructions");
                if (content is { })
                    instructionQueue.Add(content.Select(Encoding.ASCII.GetBytes).ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in nstruction service!\r\n{ex}");
            }
        }
    }

    public byte[][] GetNextInstruction()
        => instructionQueue.Take();
}
