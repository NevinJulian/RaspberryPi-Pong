using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongGame
{
    class Paddle
    {
        public RectangleF Position { get; set; }
        public float Speed { get; set; } = 200; // Adjust as needed

        public Paddle(float x, float y, float width, float height)
        {
            Position = new RectangleF(x, y, width, height);
        }

        public void Move(float deltaX, float canvasWidth)
        {
            float newX = Position.X + deltaX;
            if (newX < 0) newX = 0;
            if (newX + Position.Width > canvasWidth) newX = canvasWidth - Position.Width;

            Position = new RectangleF(newX, Position.Y, Position.Width, Position.Height);
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.White, Position);
        }
    }
}
