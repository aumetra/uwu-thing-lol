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
            BaseAddress = new Uri("http://localhost:32774"),
        };
        refreshTask = Task.Run(RefreshTask);
    }

    private async Task RefreshTask()
    {
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
