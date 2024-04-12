using System.Drawing;

namespace PongGame;
internal class Scoreboard
{
    public int HighScore { get; set; }
    public int PlayerScore { get; set; }

    public Scoreboard()
    {
        HighScore = 0;
        PlayerScore = 0;
    }

    public bool UpdateHighScore()
    {
        var isHighScore = PlayerScore > HighScore;
        if (isHighScore)
        {
            HighScore = PlayerScore;
        }

        return isHighScore;
    }

    public void IncrementPlayerScore() => PlayerScore++;

    public void ResetPlayerScore() => PlayerScore = 0;

    public Task PersistHighScore()
    {
        var filePath = GetHighScoreFilePath();
        Console.WriteLine($"Persisting high score to {filePath}");

        return File.WriteAllTextAsync(filePath, HighScore.ToString());
    }

    public async Task LoadPersistedHighScore()
    {
        var filePath = GetHighScoreFilePath();
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"High score file not found at {filePath}.");
            HighScore = 0;
            return;
        }

        var fileContent = await File.ReadAllTextAsync(filePath);
        HighScore = int.TryParse(fileContent, out int highScore) 
            ? highScore 
            : 0;

        Console.WriteLine($"High score loaded from {filePath}: {HighScore}");
    }

    public void Draw(Graphics g)
    {
        string playerScoreText = $"Score: {PlayerScore}";
        string highScoreText = $"High Score: {HighScore}";

        Font scoreFont = new Font("Arial", 12, FontStyle.Bold);
        Font highScoreFont = new Font("Arial", 10);
        Brush scoreBrush = Brushes.White;
        PointF playerScorePos = new PointF(30, 10);
        PointF highScorePos = new PointF(19, 33);

        g.DrawString(playerScoreText, scoreFont, scoreBrush, playerScorePos);
        g.DrawString(highScoreText, highScoreFont, scoreBrush, highScorePos);
        g.FillEllipse(Brushes.White, 15, 15, 10, 10);
        g.FillEllipse(Brushes.White, 100, 15, 10, 10);
        g.FillRectangle(Brushes.White, 0, 60, 128,4);
    }

    private static string GetHighScoreFilePath() => Path.Combine(Path.GetTempPath(), "pong-highscore.txt");
}
