using System.Drawing;

namespace PongGame;

internal class Ball
{
    private readonly int canvasWidth;
    private readonly int canvasHeight;
    private readonly Random random = new();

    private PointF originalVelocity;

    public PointF Position { get; set; }
    public PointF Velocity { get; set; }
    public int Radius { get; set; }

    public Ball(float x, float y, float velocityX, float velocityY, int radius, int canvasWidth, int canvasHeight)
    {
        Position = new PointF(x, y);
        Velocity = new PointF(velocityX, velocityY);
        originalVelocity = Velocity;
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

    public bool IsCollidingWithLeftWall() => Position.X <= Radius;

    public bool IsCollidingWithRightWall() => Position.X >= canvasWidth - Radius;

    public bool IsCollidingWithTopWall() => Position.Y >= canvasHeight - Radius;

    public bool IsCollidingWithBottomWall() => Position.Y <= Radius;

    public bool IsCollidingWithPaddle(Paddle paddle) =>
        Position.Y - Radius <= paddle.Position.Location.Y + paddle.Position.Height &&
        Position.X > paddle.Position.X &&
        Position.X < paddle.Position.X + paddle.Position.Width;

    public void ResetToRandomPosition()
    {
        // set random position within the canvas
        Position = new PointF(random.Next(Radius + 5, canvasWidth - (Radius + 5)), random.Next(20 + Radius, canvasHeight - (Radius + 5)));

        // generate numbers 1 or -1 to determine the direction of the ball
        var xVelocityMultiplicator = random.Next(0, 2) == 0
            ? 1
            : -1;

        var yVelocityMultiplicator = random.Next(0, 2) == 0
            ? 1
            : -1;

        Velocity = new PointF(originalVelocity.X * xVelocityMultiplicator, originalVelocity.Y * yVelocityMultiplicator);
    }

    public void Draw(Graphics g)
    {
        var transformedPosition = Position.TransformCoordinates();
        g.FillEllipse(Brushes.White, transformedPosition.X - Radius, transformedPosition.Y - Radius, Radius * 2, Radius * 2);
    }
}
