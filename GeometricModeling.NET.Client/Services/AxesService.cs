using GeometricModeling.NET.Client.Services.Interfaces;
using SkiaSharp;

namespace GeometricModeling.NET.Client.Services
{
    public class AxesService : IAxesService
    {
        public SKPoint3[] GetAxes3D(int width, int height, SKPoint originPoint)
        {
            SKPoint3[] axesPoints =
            [
                new(-originPoint.X, 0, 0),
                new(width - originPoint.X, 0, 0),
                new(0, -originPoint.Y, 0),
                new(0, height - originPoint.Y, 0),
                new(0, 0, -originPoint.X),
                new(0, 0, width - originPoint.X)
            ];

            return axesPoints;
        }

        public SKPath GetAxesPath(SKPoint[] points)
        {
            SKPath path = new SKPath();
            for (int i = 0; i < points.Length; i++)
            {
                if (i % 2 == 0)
                {
                    path.MoveTo(points[i]);
                }
                else
                {
                    path.LineTo(points[i]);
                }
            }

            return path;
        }
    }
}
