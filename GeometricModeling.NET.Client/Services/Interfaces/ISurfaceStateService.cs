using SkiaSharp;

namespace GeometricModeling.NET.Client.Services.Interfaces
{
    public interface ISurfaceStateService
    {
        public SKPoint AxisStartPoint { get; }
        public List<SKPoint> AxedPoints { get; }
        public List<SKPoint> GridPoints { get; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsActiveAxes { get; set; }
        public bool IsActiveGrid { get; set; }
        public int GridStep { get; set; }
        public void GetAxesPoints();
        public void GetGridPoints();
    }
}
