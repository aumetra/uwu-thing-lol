using System.Net.Sockets;

namespace PixelFlut.SimpleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var path = "..\\..\\..\\..\\PixelFlut.Server\\Commands\\37C3\\";
                var di = new DirectoryInfo(path);
                var files = di.GetFiles();
                var frames = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    frames[i] = File.ReadAllText(path + i + ".txt");
                }
                //echo "SIZE" | netcat 151.217.15.79 1337
                using var client = new TcpClient();
                await client.ConnectAsync("151.217.15.79", 1337);
                await using var writer = new StreamWriter(client.GetStream());
                int index = 0;
                while (true)
                {
                    if (index >= frames.Length)
                        index = 0;

                    await writer.WriteAsync(frames[index]);
                    await writer.FlushAsync();
                    Console.WriteLine("Frame: " + index++);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
