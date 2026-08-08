using SkiaSharp;

namespace GeometricModeling.NET.Client.Services.Interfaces
{
    public interface ISurfaceService
    {
        public List<SKPoint3[]> GetProjectiveEllipsoid(int a, int b, int c, int stepsNumber, float maxRadU, float maxRadV, float minRadU = 0f, float minRadV = 0f);
        //public SKPoint3[][] GetProjectiveEllipsoid(int a, int b, int c, int stepsNumber, float maxRadU, float maxRadV, float minRadU = 0f, float minRadV = 0f);
        //public SKPath GetProjectiveCylinder(int r, IProjectionService projectionService);
    }
}
