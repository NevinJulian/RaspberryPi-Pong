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
}
