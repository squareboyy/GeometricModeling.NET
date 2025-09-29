using SkiaSharp;

namespace GeometricModeling.NET.Client.Services.Interfaces
{
    public interface IAxesService
    {
        public SKPoint3[] GetAxes3D(int width, int height, SKPoint originPoint);
        public SKPath GetAxesPath(SKPoint[] points);
    }
}
