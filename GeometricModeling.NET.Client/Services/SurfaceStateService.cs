using GeometricModeling.NET.Client.Services.Interfaces;
using SkiaSharp;

namespace GeometricModeling.NET.Client.Services
{
    public class SurfaceStateService : ISurfaceStateService
    {
        public List<SKPoint> AxedPoints { get; } = []; //add get AxedPoints.ToArray() ???
        public List<SKPoint> GridPoints { get; } = [];
        public SKPoint OriginPoint
        {
            get => _originPoint;
            set
            {
                if (value.X < 0 || value.Y < 0)
                {
                    return;
                    //throw new ArgumentOutOfRangeException(nameof(value), "Origin point coordinates must be non-negative.");
                }

                _originPoint = value;
                GetAxesPoints();
                GetGridPoints();
            }
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsActiveAxes { get; set; } = true;
        public bool IsActiveGrid { get; set; } = true;
        public int GridStep { get; set; } = 75;

        private SKPoint _originPoint = new SKPoint(75, 75);

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
                new SKPoint(0, -_originPoint.Y),
                new SKPoint(0, Height - _originPoint.Y),
                new SKPoint(-_originPoint.X, 0),
                new SKPoint(Width - _originPoint.X, 0)
            ]);
        }

        public void GetGridPoints()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            GridPoints.Clear();
           
            for (int x = (int)-_originPoint.X; x <= (int)(Width - _originPoint.X); x += GridStep)
            {
                GridPoints.Add(new SKPoint(x, -_originPoint.Y));
                GridPoints.Add(new SKPoint(x, Height - _originPoint.Y));
            }

            for (int y = (int)-_originPoint.Y; y <= (int)(Height - _originPoint.Y); y += GridStep)
            {
                GridPoints.Add(new SKPoint(-_originPoint.X, y));
                GridPoints.Add(new SKPoint(Width - _originPoint.X, y));
            }
        }
    }
}
