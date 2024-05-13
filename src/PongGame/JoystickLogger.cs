using Explorer700Library;

namespace PongGame;
internal class JoystickLogger : IDisposable
{
    private readonly string logFilePath;
    private readonly StreamWriter streamWriter;

    public JoystickLogger()
    {
        this.logFilePath = Path.Combine(Path.GetTempPath(), "pong-joystick-logs.txt");
        if (!File.Exists(logFilePath))
        {
            streamWriter = new StreamWriter(File.Create(logFilePath), leaveOpen: false);
            WriteHeader();
        }
        else
        {
            streamWriter = new StreamWriter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write), leaveOpen: false);
        }

        streamWriter.AutoFlush = true;
    }

    public Task LogJoystickEvent(KeyEventArgs e)
    {
        if (e.Keys == Keys.NoKey)
        {
            // we don't want to log when the joystick returns to the center position
            return Task.CompletedTask;
        }

        return streamWriter.WriteLineAsync($"Joystick: {e.Keys}: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
    }

    public Task<string> GetLogContent()
    {
        streamWriter.Flush();
        return File.ReadAllTextAsync(logFilePath);
    }

    public void Dispose()
    {
        Console.WriteLine("Disposing");
        streamWriter?.Dispose();
    }

    private void WriteHeader()
    {
        streamWriter.WriteLine($"Logs from PongGame - Team 02");
    }
}
