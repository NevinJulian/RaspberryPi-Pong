using System.Drawing;

namespace PongGame;

internal class Ball
{
    private readonly float canvasWidth;
    private readonly float canvasHeight;
    private readonly PointF originalPosition;
    private readonly PointF originalVelocity;

    public PointF Position { get; set; }
    public PointF Velocity { get; set; }
    public float Radius { get; set; }

    public Ball(float x, float y, float velocityX, float velocityY, float radius, float canvasWidth, float canvasHeight)
    {
        originalPosition = new PointF(x, y);
        originalVelocity = new PointF(velocityX, velocityY).TransformCoordinates();
        Position = originalPosition;
        Velocity = originalVelocity;
        Radius = radius;
        this.canvasWidth = canvasWidth;
        this.canvasHeight = canvasHeight;
    }

    public void Move()
    {
        float newX = Position.X + Velocity.X;
        float newY = Position.Y + Velocity.Y;

        Position = new PointF(newX, newY);
    }

    public bool IsCollidingWithLeftWall() => Position.X < Radius;

    public bool IsCollidingWithRightWall() => Position.X > canvasWidth - Radius;

    public bool IsCollidingWithTopWall() => Position.Y > canvasHeight - Radius;

    public bool IsCollidingWithBottomWall() => Position.Y < Radius;

    public bool IsCollidingWithPaddle(Paddle paddle) => Position.Y - Radius < paddle.Position.Location.Y + paddle.Position.Height && Position.X > paddle.Position.X && Position.X < paddle.Position.X + paddle.Position.Width;

    public void Reset()
    {
        Position = originalPosition;
        Velocity = originalVelocity;
    }

    public void Draw(Graphics g)
    {
        var transformedPosition = Position.TransformCoordinates();
        g.FillEllipse(Brushes.White, transformedPosition.X - Radius, transformedPosition.Y - Radius, Radius * 2, Radius * 2);
    }
}
