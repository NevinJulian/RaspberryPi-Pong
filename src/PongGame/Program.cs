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
    private static Startscreen startscreen;
    private static JoystickLogger joystickLogger;
    private static bool gameRunning = false;
    private static bool showScore = false;

    static async Task Main(string[] args)
    {
        try
        {
            Console.CancelKeyPress += (_, _) => { joystickLogger?.Dispose(); }; // gracefully dispose joystickLogger on Ctrl+C
            Console.WriteLine("Pong Game Start...");
            exp = new Explorer700();
            joystickLogger = new JoystickLogger();

            await SetupGame();

            Console.WriteLine("Starting Game loop...");
            while (true)
            {
                if (!gameRunning)
                {
                    if (showScore)
                    {
                        DrawScore();

                        await Task.Delay(TimeSpan.FromSeconds(3)); // Wait for 3 seconds before going to startscreen
                        scoreboard.ResetPlayerScore(); // reset player score after showing it
                        showScore = false;
                    }
                    DrawStartScreen();

                    exp.Joystick.JoystickChanged += HandleGameStartInput;
                    while (!gameRunning)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(100)); // add delay to reduce CPU load
                    }
                    exp.Joystick.JoystickChanged -= HandleGameStartInput;
                }
                else
                {
                    // Execute Game loop
                    await UpdateGameState();
                    DrawGame();

                    await Task.Delay(20); // Adjust for game speed
                }
            }
        }
        finally
        {
            joystickLogger?.Dispose();
        }
    }

    private static async Task SetupGame()
    {
        paddle = new Paddle(0, 0, 20, 7, DisplayHeight, 5);
        ball = new Ball(0, 0, 1, 1, 10, DisplayHeight, DisplayWidth);

        paddle.ResetToRandomPosition();
        ball.ResetToRandomPosition();

        scoreboard = new Scoreboard();
        await scoreboard.LoadPersistedHighScore();

        startscreen = new Startscreen();

        exp.Joystick.JoystickChanged += HandleGameInput;
        exp.Joystick.JoystickChanged += async (_, e) => await joystickLogger.LogJoystickEvent(e);
    }

    private static void HandleGameStartInput(object? _, KeyEventArgs e)
    {
        gameRunning = true;
    }

    private static void HandleGameInput(object? _, KeyEventArgs e)
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

    private static async Task UpdateGameState()
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
            _ = Task.Run(() =>
            {
                exp.Buzzer.Beep(200);
            });
        }
        else if (ball.IsCollidingWithPaddle(paddle))
        {
            // Reverse the y velocity of the ball to bounce off the paddle
            ball.Velocity = new PointF(ball.Velocity.X, -ball.Velocity.Y);

            // Increase player score
            scoreboard.IncrementPlayerScore();
            Console.WriteLine($"Player Score: {scoreboard.PlayerScore}");

            if (scoreboard.PlayerScore == scoreboard.HighScore)
            {
                _ = Task.Run(async () =>
                {
                    exp.Buzzer.Beep(500);
                    await Task.Delay(500);
                    exp.Buzzer.Beep(200);
                    await Task.Delay(200);
                    exp.Buzzer.Beep(200);
                    await Task.Delay(200);
                    exp.Buzzer.Beep(500);
                });
            }

            // Increase ball speed
            ball.Velocity = new PointF(ball.Velocity.X * 1.1f, ball.Velocity.Y * 1.1f);
        }
        else if (ball.IsCollidingWithBottomWall())
        {
            Console.WriteLine($"Game Over with score {scoreboard.PlayerScore}");

            // Reset the ball position and velocity
            ball.ResetToRandomPosition();

            // Reset the paddle position
            paddle.ResetToRandomPosition();

            // Check and update high score
            if (scoreboard.UpdateHighScore())
            {
                // It is a high score
                Console.WriteLine($"New High Score: {scoreboard.HighScore}");

                // Persist high score
                await scoreboard.PersistHighScore();
            }

            _ = Task.Run(async () =>
            {
                exp.Buzzer.Beep(300);
                await Task.Delay(300);
                exp.Buzzer.Beep(500);
                await Task.Delay(500);
                exp.Buzzer.Beep(1000);
            });

            // Go back to start screen
            gameRunning = false;
            showScore = true;
        }
    }

    private static void DrawStartScreen()
    {
        Graphics g = exp.Display.Graphics;
        g.Clear(Color.Black);

        startscreen.Draw(g, scoreboard.HighScore);

        exp.Display.Update(); // Update display
    }

    private static void DrawGame()
    {
        Graphics g = exp.Display.Graphics;
        g.Clear(Color.Black);

        paddle.Draw(g);
        ball.Draw(g);

        exp.Display.Update(); // Update display
    }

    private static void DrawScore()
    {
        Graphics g = exp.Display.Graphics;
        g.Clear(Color.Black);

        scoreboard.Draw(g);

        exp.Display.Update();
    }
};
