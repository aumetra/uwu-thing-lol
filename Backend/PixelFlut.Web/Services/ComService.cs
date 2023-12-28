using System.Net.Sockets;

namespace PixelFlut.Web.Services
{
    public class ComService : IDisposable
    {
        private readonly TcpClient tcpClient;
        private readonly StreamWriter writer;

        public ComService()
        {
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpClient = new TcpClient();
            tcpClient.Connect("151.217.15.79", 1337);
            writer = new StreamWriter(tcpClient.GetStream());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
