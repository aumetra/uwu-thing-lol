using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace FixelPlut.Client;

internal static class Program
{
    internal static readonly string s_Ip_Backend = Environment.GetEnvironmentVariable("BACKEND_IP") ?? "151.217.2.77";// "151.217.2.77";
    internal static readonly string s_Port_Backend = Environment.GetEnvironmentVariable("BACKEND_PORT") ?? "5000";
    internal static readonly string s_Ip_PixelFlut = Environment.GetEnvironmentVariable("PIXELFLUT_IP") ?? "151.217.15.90";
    internal static readonly string s_Port_PixelFlut = Environment.GetEnvironmentVariable("PIXELFLUT_PORT") ?? "1337";

    const int s_connectingTimeout = 5000;
    static readonly Task consoleWriteTask;
    private readonly static byte[] nl = new byte[] { (byte)'\n' };

    static Program()
    {
        consoleWriteTask = Task.Run(() =>
        {
            while (true)
                Console.WriteLine(msgQueue.Take());
        });
    }

    static async Task Main(string[] args)
    {
        await Run(s_Ip_PixelFlut, int.Parse(s_Port_PixelFlut));
    }

    private static async Task Run(string ip, int port)
    {
        Socket[] sockets;
        do
        {
            sockets = await TryOpenConnections(ip, port);
        } while (sockets.Length == 0);

        var tasks = new List<Task>();
        for (int i = 0; i < sockets.Length; i++)
        {
            var index = i;
            var task = Task.Run(async () => await RunClient(index, sockets[index]));
            tasks.Add(task);
        }
        Task.WaitAll(tasks.ToArray());
    }

    static async Task<Socket[]> TryOpenConnections(string ip, int port)
    {
        var sockets = new List<Socket>();
        try
        {
            while (true)
            {
                ConsoleWriteLine(-1, "Testing client " + sockets.Count);
                var clientSocket = await CreateSocketConnection(ip, port);
                sockets.Add(clientSocket);
                ConsoleWriteLine(-1, "Client " + sockets.Count + " OK");
            }
        }
        catch (Exception ex)
        {
            ConsoleWriteLine(-1, "Client " + sockets.Count + " NOT OK");
        }
        ConsoleWriteLine(-1, "Total clients " + sockets.Count);
        return sockets.ToArray();
    }

    private static async Task<Socket> CreateSocketConnection(string ip, int port)
    {
        var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await clientSocket.ConnectAsync(ip, port, new CancellationTokenSource(s_connectingTimeout).Token);
        return clientSocket;
    }

    static async Task RunClient(int taskId, Socket socket)
    {
        try
        {
            var networkStream = new NetworkStream(socket, true);

            ConsoleWriteLine(taskId, "Running");

            while (true)
            {
                try
                {
                    var cmds = InstructionService.Instance.GetNextInstruction();
                    for (int i = 0; i < cmds.Length; i++)
                    {
                        //ConsoleWriteLine(taskId, Encoding.ASCII.GetString(cmds[i]));
                        await networkStream.WriteAsync(cmds[i]);
                        await networkStream.WriteAsync(nl);
                        await networkStream.FlushAsync();
                    }
                    ConsoleWriteLine(taskId, "Update");
                }
                catch (Exception ex)
                {
                    ConsoleWriteLine(taskId, "Fail");
                }
            }
        }
        catch (Exception ex)
        {

            throw;
        }
        finally
        {
            socket?.Dispose();
        }
    }

    static readonly BlockingCollection<string> msgQueue = new();

    static void ConsoleWriteLine(int taskId, string msg)
        => _ = Task.Run(() => msgQueue.Add(string.Format("{0} [{1}] {2}", DateTime.Now.ToString("HH:mm:ss.FFF"), taskId, msg)));
}
