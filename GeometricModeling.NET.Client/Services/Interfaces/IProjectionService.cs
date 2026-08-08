using SkiaSharp;

namespace GeometricModeling.NET.Client.Services.Interfaces
{
    public interface IProjectionService
    {
        public float[,] GetDimetricProjectionMatrix(int alpha);
        public SKPoint GetDimetricProjection(SKPoint3 point, int alpha);
        public SKPoint[] GetDimetricProjection(SKPoint3[] points, int alpha);
        public List<SKPoint> GetDimetricProjection(List<SKPoint3> points, int alpha);
        public SKPath GetDimetricProjection2(SKPoint3[] points, int alpha);
    }
}