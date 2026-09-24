using GeometricModeling.NET.Client.Services.Interfaces;
using SkiaSharp;

namespace GeometricModeling.NET.Client.Services
{
    public class SurfaceStateService : ISurfaceStateService
    {
        public SKPoint AxisStartPoint { get; } = new SKPoint(5, 5);
        public List<SKPoint> AxedPoints { get; } = []; //add get AxedPoints.ToArray() ???
        public List<SKPoint> GridPoints { get; } = [];
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsActiveAxes { get; set; } = true;
        public bool IsActiveGrid { get; set; } = true;
        public int GridStep { get; set; } = 75;

        public void GetAxesPoints()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
                //throw new InvalidOperationException("Width and Height must be greater than zero.");
            }

            AxedPoints.Clear();
            AxedPoints.AddRange(
            [
                new SKPoint(0, 0),
                new SKPoint(0, Height),
                new SKPoint(0, 0),
                new SKPoint(Width, 0)
            ]);
        }

        public void GetGridPoints()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            GridPoints.Clear();

            for (int x = 0; x <= Width; x += GridStep)
            {
                GridPoints.Add(new SKPoint(x, 0));
                GridPoints.Add(new SKPoint(x, Height));
            }

            for (int y = 0; y <= Height; y += GridStep)
            {
                GridPoints.Add(new SKPoint(0, y));
                GridPoints.Add(new SKPoint(Width, y));
            }
        }
    }
}
