using GeometricModeling.NET.Client.Enums;
using GeometricModeling.NET.Client.Extensions;
using GeometricModeling.NET.Client.Services.Interfaces;
using SkiaSharp;
using static System.MathF;

namespace GeometricModeling.NET.Client.Services
{
    //DimetricProjectionService - перейменувати, якщо розширювати функціонал іншими проекціями
    public class ProjectionService : IProjectionService
    {
        private readonly IRotationTransformMatrixService _rotationTransformMatrixService;
        private readonly IOrthographicProjectionMatrixService _orthographicProjectionMatrixService;

        public ProjectionService(IRotationTransformMatrixService rotationTransformMatrixService,
                                 IOrthographicProjectionMatrixService orthographicProjectionMatrixService)
        {
            _rotationTransformMatrixService = rotationTransformMatrixService;
            _orthographicProjectionMatrixService = orthographicProjectionMatrixService;
        }

        public float[,] GetDimetricProjectionMatrix(int alpha)
        {
            float alfaInRadians = alpha.GetRadianF();
            float alfaSinSquared = (float)Pow(Sin(alfaInRadians), 2);
            float betaSin = (float)Sqrt(alfaSinSquared / (1 - alfaSinSquared));
            float betaInRadians = (float)Asin(betaSin);

            float[,] firstRotationMatrix = _rotationTransformMatrixService.GetMatrixAroundAxisY(betaInRadians);
            float[,] secondRotationMatrix = _rotationTransformMatrixService.GetMatrixAroundAxisX(alfaInRadians);
            float[,] projectionMatrixOnPlaneZ = _orthographicProjectionMatrixService.GetMatrixProjectionOnPlane(ProjectionPlane.Z);

            return firstRotationMatrix.Multiply(secondRotationMatrix).Multiply(projectionMatrixOnPlaneZ);
        }

        public SKPoint GetDimetricProjection(SKPoint3 point, int alpha)
        {
            float[,] projectionTransformMatrix = GetDimetricProjectionMatrix(alpha);
            SKPoint3 projectionPoint = point.ApplyTransformMatrix(projectionTransformMatrix);

            return projectionPoint.GetProjectionPoint(ProjectionPlane.Z);
        }

        public SKPoint[] GetDimetricProjection(SKPoint3[] points, int alpha)
        {
            float[,] projectionTransformMatrix = GetDimetricProjectionMatrix(alpha);
            SKPoint[] projectedPoints = points
                .Select(p => p.ApplyTransformMatrix(projectionTransformMatrix).GetProjectionPoint(ProjectionPlane.Z))
                .ToArray();

            return projectedPoints;
        }

        public List<SKPoint> GetDimetricProjection(List<SKPoint3> points, int alpha)
        {
            float[,] projectionTransformMatrix = GetDimetricProjectionMatrix(alpha);
            List<SKPoint> projectedPoints = points
                .Select(p => p.ApplyTransformMatrix(projectionTransformMatrix).GetProjectionPoint(ProjectionPlane.Z))
                .ToList();

            return projectedPoints;
        }
    }
}
