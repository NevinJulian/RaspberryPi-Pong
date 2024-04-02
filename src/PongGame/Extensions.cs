using System.Drawing;

namespace PongGame;
internal static class Extensions
{
    internal static RectangleF TransformCoordinates(this RectangleF rect)
    {
        return new RectangleF(rect.Y, rect.X, rect.Height, rect.Width);
    }

    internal static PointF TransformCoordinates(this PointF point)
    {
        return new PointF(point.Y, point.X);
    }
}
