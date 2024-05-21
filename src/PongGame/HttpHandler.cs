using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PongGame
{
    public class HttpHandler
    {
        private TcpClient client;

        public HttpHandler(TcpClient client)
        {
            this.client = client;
        }

        public void Do()
        {
            StreamReader sr = new StreamReader(client.GetStream());
            StreamWriter sw = new StreamWriter(client.GetStream()) { AutoFlush = true };

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
                client.Close();
            }
        }

        private void ServeLogFile(StreamWriter sw, bool isDownload)
        {
            string logFilePath = Path.Combine(Path.GetTempPath(), "pong-joystick-logs.txt");
            if (!File.Exists(logFilePath))
            {
                sw.WriteLine("HTTP/1.1 404 Not Found");
                sw.WriteLine("Content-Type: text/plain");
                sw.WriteLine("Connection: close");
                sw.WriteLine("");
                sw.WriteLine("404 Not Found");
                return;
            }

            sw.WriteLine("HTTP/1.1 200 OK");
            if (isDownload)
            {
                sw.WriteLine("Content-Type: application/octet-stream");
                sw.WriteLine($"Content-Disposition: attachment; filename=\"{Path.GetFileName(logFilePath)}\"");
            }
            else
            {
                sw.WriteLine("Content-Type: text/plain");
            }
            sw.WriteLine("Connection: close");
            sw.WriteLine(""); // Leere Zeile signalisiert Ende der Header

            using (var fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sw.WriteLine(line);
                }
            }
        }

        private void SendNotFound(StreamWriter sw)
        {
            sw.WriteLine("HTTP/1.1 404 Not Found");
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine("Connection: close");
            sw.WriteLine("");
            sw.WriteLine("404 Not Found");
        }



        private void SendBadRequest(StreamWriter sw)
        {
            sw.WriteLine("HTTP/1.1 400 Bad Request");
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine("Connection: close");
            sw.WriteLine("");
            sw.WriteLine("400 Bad Request");
        }

        private void SendMethodNotAllowed(StreamWriter sw)
        {
            sw.WriteLine("HTTP/1.1 405 Method Not Allowed");
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine("Connection: close");
            sw.WriteLine("");
            sw.WriteLine("405 Method Not Allowed");
        }
    }
}
