using System.Net.Sockets;
using System.Net;

namespace PongGame
{
    internal class HttpServer : IDisposable
    {
        private readonly TcpListener listener;
        private readonly JoystickLogger joystickLogger;
        private bool isRunning = true;

        public HttpServer(int port, JoystickLogger joystickLogger)
        {
            this.listener = new TcpListener(IPAddress.Any, port);
            this.joystickLogger = joystickLogger;
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("HTTP Server started on port " + ((IPEndPoint)listener.LocalEndpoint).Port);

            try
            {
                while (isRunning)
                {
                    try
                    {
                        var client = listener.AcceptTcpClient();
                        var handler = new HttpHandler(client, joystickLogger);
                        var clientThread = new Thread(new ThreadStart(handler.HandleRequest));
                        clientThread.Start();
                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
                    {
                        // Expected exception when stopping the listener.
                        Console.WriteLine("AcceptTcpClient interrupted due to server stopping.");
                    }
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        public void Dispose()
        {
            isRunning = false;
            listener?.Stop();
            listener?.Dispose();

            Console.WriteLine("HTTP Server stopped.");
        }
    }
}
