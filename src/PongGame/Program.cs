using Explorer700Library;
using System;
using System.Drawing;
using System.Reflection.Metadata;

namespace PongGame
{
    class Program
    {
        private static Explorer700 exp;
        private static Paddle paddle;
        private static Ball ball;

        private static readonly int DisplayWidth = 128;
        private static readonly int DisplayHeight = 64;

        static void Main(string[] args)
        {
            Console.WriteLine("Pong Game Start...");
            exp = new Explorer700();

            SetupGame();

            while (true)
            {
                HandleInput();
                UpdateGameState();
                DrawGame();
                System.Threading.Thread.Sleep(20); // Adjust for game speed
            }
        }

        static void SetupGame()
        {
            paddle = new Paddle(100, 230, 50, 10);
            ball = new Ball(120, 110, 2, 2);
        }

        static void HandleInput()
        {
            // Use exp.Joystick.JoystickChanged event to move paddles
        }

        static void UpdateGameState()
        {
            // Move the ball
            ball.Position = new PointF(ball.Position.X + ball.Velocity.X, ball.Position.Y + ball.Velocity.Y);

            // Collision with walls - Left or Right
            if (ball.Position.X < ball.Radius || ball.Position.X > DisplayWidth - ball.Radius)
            {
                ball.Velocity = new PointF(-ball.Velocity.X, ball.Velocity.Y);
            }
            if (ball.Position.Y < ball.Radius)
            {
                ball.Velocity = new PointF(ball.Velocity.X, -ball.Velocity.Y);
            }
            else if (ball.Position.Y > DisplayHeight - ball.Radius) // Bottom wall
            {
                // Reset ball position or end game
                ball.Position = new PointF(DisplayWidth / 2, DisplayHeight / 2);
                ball.Velocity = new PointF(ball.Velocity.X, -2); // Example reset velocity
            }

            // Collision with paddle
            if (ball.Position.Y + ball.Radius > paddle.Position.Y && ball.Position.X > paddle.Position.X && ball.Position.X < paddle.Position.X + paddle.Position.Width)
            {
                // Here we reverse the Y direction of velocity upon collision
                ball.Velocity = new PointF(ball.Velocity.X, -ball.Velocity.Y);

                // Optionally, adjust ball.Velocity.X based on where it hits the paddle for more dynamics
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
    }
}
