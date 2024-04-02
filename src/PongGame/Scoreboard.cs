using System.Drawing;

namespace PongGame;
internal class Scoreboard
{
    public int HighScore { get; set; }
    public int PlayerScore { get; set; }

    public Scoreboard(int highScore)
    {
        HighScore = highScore;
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

    public void Draw(Graphics g)
    {
        string playerScoreText = $"Score: {PlayerScore}";
        string highScoreText = $"High Score: {HighScore}";

        Font scoreFont = new Font("Arial", 12);
        Brush scoreBrush = Brushes.White;
        PointF playerScorePos = new PointF(10, 10);
        PointF highScorePos = new PointF(10, 40);

        g.DrawString(playerScoreText, scoreFont, scoreBrush, playerScorePos);
        g.DrawString(highScoreText, scoreFont, scoreBrush, highScorePos);
    }
}
