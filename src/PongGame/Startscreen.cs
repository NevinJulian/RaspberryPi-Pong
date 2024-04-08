using System.Drawing;

namespace PongGame;

internal class Startscreen
{
    private string message;
    private bool start;

    public Startscreen(string msg)
    {
        this.message = msg;
        this.start = false;
    }
    
    public void Draw(Graphics g, int highScore)
    {
        g.DrawString(message, new Font("Arial", 8), Brushes.White, 10, 10);
        g.DrawString($"High Score: {highScore}", new Font("Arial", 8), Brushes.White, 10, 40);
    }
}