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
            streamWriter = new StreamWriter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read), leaveOpen: false);
            WriteHeader();
        }
        else
        {
            streamWriter = new StreamWriter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write), leaveOpen: false);
        }

        streamWriter.AutoFlush = true;
    }

    public async Task LogJoystickEvent(KeyEventArgs e)
    {
        if (e.Keys == Keys.NoKey)
        {
            // we don't want to log when the joystick returns to the center position
            return;
        }

        using (var sw = new StreamWriter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read)))
        {
            await sw.WriteLineAsync($"Joystick: {e.Keys}: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            sw.Flush();
        }
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
