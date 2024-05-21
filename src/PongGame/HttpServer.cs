using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PongGame
{
    public class HttpServer
    {
        private TcpListener listener;

        public HttpServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("HTTP Server started on port " + ((IPEndPoint)listener.LocalEndpoint).Port);

            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    HttpHandler handler = new HttpHandler(client);
                    Thread clientThread = new Thread(new ThreadStart(handler.Do));
                    clientThread.Start();
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        public void Stop()
        {
            listener.Stop();
            Console.WriteLine("HTTP Server stopped.");
        }
    }
}
