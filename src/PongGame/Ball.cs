using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongGame
{
    class Ball
    {
        public PointF Position { get; set; }
        public PointF Velocity { get; set; }

        public float Radius { get; set; } = 10; // Adjust as needed

        public Ball(float x, float y, float velocityX, float velocityY)
        {
            Position = new PointF(x, y);
            Velocity = new PointF(velocityX, velocityY);
        }

        public void Move()
        {
            float newX = Position.X + Velocity.X;
            float newY = Position.Y + Velocity.Y;
            
            Position = new PointF(newX, newY);
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.White, Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
        }
    }
}
