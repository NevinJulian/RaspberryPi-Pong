using Explorer700Library;
using System.Drawing;

namespace PongGame;

internal class Program
{
    private const int DisplayWidth = 128;
    private const int DisplayHeight = 64;

    private static Explorer700 exp;
    private static Paddle paddle;
    private static Ball ball;
    private static Scoreboard scoreboard;
    private static DateTime lastPaddleMove;

    private static bool GameStillRunning = true;

    static async Task Main(string[] args)
    {
        Console.WriteLine("Pong Game Start...");
        exp = new Explorer700();

        await SetupGame();

        await Task.Delay(TimeSpan.FromSeconds(2)); // Wait for 2 seconds before starting the game
        Console.WriteLine("Starting Game loop...");

        while (GameStillRunning)
        {
            // Execute game loop
            HandleInput();
            await UpdateGameState();
            DrawGame();

            if (!GameStillRunning)
            {
                DrawScore();

                await Task.Delay(TimeSpan.FromSeconds(3)); // Wait for 2 seconds before restarting the game
                GameStillRunning = true;
                scoreboard.ResetPlayerScore();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(20)); // Adjust for game speed
        }
    }

    static async Task SetupGame()
    {
        paddle = new Paddle(20, 0, 20, 7, DisplayHeight, 5);
        ball = new Ball(30, 100, 1, 1, 10, DisplayHeight, DisplayWidth);
        scoreboard = new Scoreboard();

        await scoreboard.LoadPersistedHighScore();
    }

    static void HandleInput()
    {
        exp.Joystick.JoystickChanged += (_, e) =>
        {
            if (e.Keys.HasFlag(Keys.Down) && IsAllowedToMovePaddle())
            {
                paddle.MoveRight(GetAmountOfSteps(e.Keys));
                lastPaddleMove = DateTime.UtcNow;
            }
            else if (e.Keys.HasFlag(Keys.Up) && IsAllowedToMovePaddle())
            {
                paddle.MoveLeft(GetAmountOfSteps(e.Keys));
                lastPaddleMove = DateTime.UtcNow;
            }
        };

        static bool IsAllowedToMovePaddle()
        {
            // Limit paddle movement to every 100ms
            return DateTime.UtcNow - lastPaddleMove > TimeSpan.FromMilliseconds(100);
        }

        static int GetAmountOfSteps(Keys keys)
        {
            return keys.HasFlag(Keys.Center)
                ? 3
                : 1;
        }
    }

    static async Task UpdateGameState()
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
            Console.WriteLine($"Player Score: {scoreboard.PlayerScore}");

            // Increase ball speed
            ball.Velocity = new PointF(ball.Velocity.X * 1.1f, ball.Velocity.Y * 1.1f);
        }
        else if (ball.IsCollidingWithBottomWall())
        {
            Console.WriteLine($"Game Over with score {scoreboard.PlayerScore}");

            // Reset the ball position and velocity
            ball.Reset();

            // Check and update high score
            if (scoreboard.UpdateHighScore())
            {
                // It is a high score
                Console.WriteLine($"New High Score: {scoreboard.HighScore}");

                // Persist high score
                await scoreboard.PersistHighScore();
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
