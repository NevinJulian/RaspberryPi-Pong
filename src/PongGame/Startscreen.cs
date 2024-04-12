using System.Drawing;

namespace PongGame;

internal class Startscreen
{

    public Startscreen()
    {
        
    }
    //$"Pong Game{Environment.NewLine}Press any key to start"
    public void Draw(Graphics g, int highScore)
    {
        g.DrawString("Pong Game", new Font("Arial", 12, FontStyle.Bold), Brushes.White, 18, 10);
        g.DrawString("Press any key to start", new Font("Arial", 8), Brushes.White, 10, 35);
        g.DrawString($"High Score: {highScore}", new Font("Arial", 8), Brushes.White, 30, 50);
        g.FillEllipse(Brushes.White, 8, 16, 8, 8);
        g.FillEllipse(Brushes.White, 112, 16, 8, 8);
    }
}