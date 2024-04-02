using Explorer700Library;
using System.Drawing;

namespace PongGame;

internal class Program
{
    private static Explorer700 exp;
    private static Paddle paddle;
    private static Ball ball;
    private static Scoreboard scoreboard;
    private static DateTime lastPaddleMove;

    private const int DisplayWidth = 128;
    private const int DisplayHeight = 64;

    private static bool GameStillRunning = true;

    static async Task Main(string[] args)
    {
        Console.WriteLine("Pong Game Start...");
        exp = new Explorer700();

        SetupGame();

        while (GameStillRunning)
        {
            // Execute Game loop
            HandleInput();
            UpdateGameState();
            DrawGame();

            if (!GameStillRunning)
            {
                DrawScore();

                Thread.Sleep(2000);
                GameStillRunning = true;
                scoreboard.ResetPlayerScore();
            }

            await Task.Delay(20); // Adjust for game speed
        }
    }

    static void SetupGame()
    {
        paddle = new Paddle(20, 0, 20, 7, DisplayHeight, 5);
        ball = new Ball(30, 100, 1, 1, 10, DisplayHeight, DisplayWidth);
        scoreboard = new Scoreboard(0);
    }

    static void HandleInput()
    {
        exp.Joystick.JoystickChanged += (_, e) =>
        {
            if (e.Keys.HasFlag(Keys.Down) && IsAllowedToMovePaddle())
            {
                paddle.MoveRight();
                lastPaddleMove = DateTime.UtcNow;
            }
            else if (e.Keys.HasFlag(Keys.Up) && IsAllowedToMovePaddle())
            {
                paddle.MoveLeft();
                lastPaddleMove = DateTime.UtcNow;
            }
        };

        static bool IsAllowedToMovePaddle()
        {
            // Limit paddle movement to every 100ms
            return DateTime.UtcNow - lastPaddleMove > TimeSpan.FromMilliseconds(100);
        }
    }

    static void UpdateGameState()
    {
        // Move the ball
        ball.Move();

        // Collision with walls
        if (ball.IsCollidingWithLeftWall() || ball.IsCollidingWithRightWall())
        {
            // Reverse the x velocity of the ball to bounce off the wall
            ball.Velocity = new PointF(-ball.Velocity.X, ball.Velocity.Y);
        }
        else if (ball.IsCollidingWithTopWall())
        {
            // Reverse the y velocity of the ball to bounce off the wall
            ball.Velocity = new PointF(ball.Velocity.X, -ball.Velocity.Y);
        }
        else if (ball.IsCollidingWithPaddle(paddle))
        {
            // Reverse the y velocity of the ball to bounce off the paddle
            ball.Velocity = new PointF(ball.Velocity.X, -ball.Velocity.Y);

            // Increase player score
            scoreboard.IncrementPlayerScore();

            // Increase ball speed
            ball.Velocity = new PointF(ball.Velocity.X * 1.1f, ball.Velocity.Y * 1.1f);
        }
        else if (ball.IsCollidingWithBottomWall())
        {
            // Reset the ball position and velocity
            ball.Reset();

            // Check and update high score
            if (scoreboard.UpdateHighScore())
            {
                // It is a high score
                Console.WriteLine($"New High Score: {scoreboard.HighScore}");
            }

            GameStillRunning = false;
        }
    }

    static void DrawGame()
    {
        Graphics g = exp.Display.Graphics;
        g.Clear(Color.Black);

        paddle.Draw(g);
        ball.Draw(g);

        exp.Display.Update(); // Update display
    }

    static void DrawScore()
    {
        Graphics g = exp.Display.Graphics;
        g.Clear(Color.Black);

        scoreboard.Draw(g);

        exp.Display.Update();
    }
}
