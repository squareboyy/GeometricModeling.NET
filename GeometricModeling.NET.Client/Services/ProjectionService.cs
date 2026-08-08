using GeometricModeling.NET.Client.Enums;
using GeometricModeling.NET.Client.Extensions;
using GeometricModeling.NET.Client.Services.Interfaces;
using SkiaSharp;
using System.Diagnostics;
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

        private float[,] GetMatrixTest(int alpha)
        {
            float alfaInRadians = alpha.GetRadianF();
            float alfaSinSquared = (float)Pow(Sin(alfaInRadians), 2);
            float betaSin = (float)Sqrt(alfaSinSquared / (1 - alfaSinSquared));
            float betaInRadians = (float)Asin(betaSin);

            float[,] transformMatrix =
            {
                {Cos(betaInRadians), Sin(betaInRadians) * Sin(alfaInRadians), 0},
                {0, Cos(alfaInRadians), 0},
                {Sin(betaInRadians), -Sin(alfaInRadians) * Cos(betaInRadians), 0}
            };

            
            /*for (int i = 0; i < transformMatrix.GetLength(0); i ++)
            {
                for (int j = 0; j < transformMatrix.GetLength(1); j ++)
                {
                    Console.WriteLine(transformMatrix[i, j]);
                }
                Console.WriteLine();
            }*/

            return transformMatrix;
        }

        public SKPoint GetDimetricProjection(SKPoint3 point, int alpha)
        {
            float[,] projectionTransformMatrix = GetDimetricProjectionMatrix(alpha);
            SKPoint3 projectionPoint = point.ApplyTransformMatrix(projectionTransformMatrix);

            return projectionPoint.GetProjectionPoint(ProjectionPlane.Z);
        }

        public SKPoint[] GetDimetricProjection(SKPoint3[] points, int alpha)
        {
            float[,] projectionTransformMatrix = GetMatrixTest(alpha);
            SKPoint[] projectedPoints = points
                .Select(p => p.Multiply(projectionTransformMatrix))
                .ToArray();

            /*float[,] projectionTransformMatrix = GetDimetricProjectionMatrix(alpha);
            SKPoint[] projectedPoints = points
                .Select(p => p.ApplyTransformMatrix(projectionTransformMatrix).GetProjectionPoint(ProjectionPlane.Z))
                .ToArray();*/

            /*var timer = Stopwatch.StartNew();
            timer.Stop();
            Console.WriteLine($"time ms: {timer.ElapsedMilliseconds}");*/

            return projectedPoints;
        }

        public SKPath GetDimetricProjection2(SKPoint3[] points, int alpha)
        {
            SKPath path = new SKPath();
            float[,] projectionTransformMatrix = GetDimetricProjectionMatrix(alpha);

            for (int i = 0; i < points.Length; i++)
            {
                path.LineTo(points[i].ApplyTransformMatrix(projectionTransformMatrix).GetProjectionPoint(ProjectionPlane.Z));
            }

            return path;
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
