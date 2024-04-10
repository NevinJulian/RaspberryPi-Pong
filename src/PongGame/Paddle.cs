using System.Drawing;

namespace PongGame;

internal class Paddle
{
    private readonly int moveSpeed;
    private readonly int canvasWidth;
    private readonly Random random = new();

    public RectangleF Position { get; set; }

    public Paddle(float x, float y, float width, float height, int canvasWidth, int moveSpeed)
    {
        Position = new RectangleF(x, y, width, height);
        this.canvasWidth = canvasWidth;
        this.moveSpeed = moveSpeed;
    }

    public void MoveRight(int steps)
    {
        Move(moveSpeed * steps);
    }

    public void MoveLeft(int steps)
    {
        Move(-moveSpeed * steps);
    }

    public void ResetToRandomPosition()
    {
        Position = new RectangleF(random.Next(0, canvasWidth - 20), Position.Y, Position.Width, Position.Height);
    }

    private void Move(float deltaX)
    {
        float newX = Position.X + deltaX;
        if (newX < 0)
        {
            newX = 0;
        }
        else if (newX + Position.Width > canvasWidth)
        {
            newX = canvasWidth - Position.Width;
        }

        Position = new RectangleF(newX, Position.Y, Position.Width, Position.Height);
    }

    public void Draw(Graphics g)
    {
        g.FillRectangle(Brushes.White, Position.TransformCoordinates());
    }
}
