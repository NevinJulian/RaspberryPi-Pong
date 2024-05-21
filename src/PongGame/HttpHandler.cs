using System.Net.Sockets;

namespace PongGame
{
    internal class HttpHandler
    {
        private TcpClient client;
        private readonly JoystickLogger joystickLogger;

        public HttpHandler(TcpClient client, JoystickLogger joystickLogger)
        {
            this.client = client;
            this.joystickLogger = joystickLogger;
        }

        public void HandleRequest()
        {
            using var sr = new StreamReader(client.GetStream());
            using var sw = new StreamWriter(client.GetStream()) { AutoFlush = true };

            try
            {
                string requestLine = sr.ReadLine();
                Console.WriteLine("Request: " + requestLine);

                if (string.IsNullOrEmpty(requestLine))
                {
                    return; // Kein gültiger Request
                }

                string[] tokens = requestLine.Split(' ');
                if (tokens.Length != 3)
                {
                    SendBadRequest(sw);
                    return;
                }

                string method = tokens[0];
                string url = tokens[1];
                string httpVersion = tokens[2];

                if (method != "GET")
                {
                    SendMethodNotAllowed(sw);
                    return;
                }

                switch (url)
                {
                    case "/logs":
                        ServeLogFile(sw, false); // Nur anzeigen
                        break;
                    case "/download":
                        ServeLogFile(sw, true); // Download
                        break;
                    default:
                        SendNotFound(sw);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                client?.Dispose();
            }
        }

        private void ServeLogFile(StreamWriter sw, bool isDownload)
        {
            sw.WriteLine("HTTP/1.1 200 OK");
            if (isDownload)
            {
                sw.WriteLine("Content-Type: application/octet-stream");
                sw.WriteLine($"Content-Disposition: attachment; filename=\"pong-joystick-logs.txt\"");
            }
            else
            {
                sw.WriteLine("Content-Type: text/plain");
            }
            sw.WriteLine("Connection: close");
            sw.WriteLine(""); // Leere Zeile signalisiert Ende der Header

            var logContent = joystickLogger.GetLogContent().Result;
            sw.WriteLine(logContent);
        }

        private void SendNotFound(StreamWriter sw)
        {
            sw.WriteLine("HTTP/1.1 404 Not Found");
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine("Connection: close");
            sw.WriteLine("");
            sw.WriteLine("404 Not Found");
        }

        private static void SendBadRequest(StreamWriter sw)
        {
            sw.WriteLine("HTTP/1.1 400 Bad Request");
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine("Connection: close");
            sw.WriteLine("");
            sw.WriteLine("400 Bad Request");
        }

        private static void SendMethodNotAllowed(StreamWriter sw)
        {
            sw.WriteLine("HTTP/1.1 405 Method Not Allowed");
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine("Connection: close");
            sw.WriteLine("");
            sw.WriteLine("405 Method Not Allowed");
        }
    }
}
