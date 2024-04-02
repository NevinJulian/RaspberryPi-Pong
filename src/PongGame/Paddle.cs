using System.Drawing;

namespace PongGame;

internal class Paddle
{
    private readonly float moveSpeed;
    private readonly float canvasWidth;

    public RectangleF Position { get; set; }

    public Paddle(float x, float y, float width, float height, float canvasWidth, float moveSpeed)
    {
        Position = new RectangleF(x, y, width, height);
        this.canvasWidth = canvasWidth;
        this.moveSpeed = moveSpeed;
    }

    public void MoveRight()
    {
        Move(moveSpeed);
    }

    public void MoveLeft()
    {
        Move(-moveSpeed);
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
