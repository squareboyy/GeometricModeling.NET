using SkiaSharp;
using GeometricModeling.NET.Client.Enums;

namespace GeometricModeling.NET.Client.Extensions
{
    public static class SKPoint3Extension
    {
        public static float[] GetArray(this SKPoint3 point)
        {
            float[] points =
            [
                point.X,
                point.Y,
                point.Z
            ];

            return points;
        }

        public static SKPoint3 Multiply(this SKPoint3 point, double number)
        {
            SKPoint3 resultPoint = new()
            {
                X = (float)(point.X * number),
                Y = (float)(point.Y * number),
                Z = (float)(point.Z * number)
            };

            return resultPoint;
        }

        public static SKPoint GetProjectionPoint(this SKPoint3 point, ProjectionPlane zeroPlane)
        {
            SKPoint result = zeroPlane switch
            {
                ProjectionPlane.X => new SKPoint(point.Z, point.Y),
                ProjectionPlane.Y => new SKPoint(point.Z, point.X),
                ProjectionPlane.Z => new SKPoint(point.X, point.Y),
                _ => throw new NotImplementedException()
            };

            return result;
        }

        public static SKPoint3 ApplyTransformMatrix(this SKPoint3 point, float[,] transformMatrix)
        {
            float[] homogeneousСoordinates = GetHomogeneousCoordinates(point);
            homogeneousСoordinates = homogeneousСoordinates.Multiply(transformMatrix);

            return GetCartesianPoint(homogeneousСoordinates);
        }

        public static float[] GetHomogeneousCoordinates(this SKPoint3 point)
        {
            return [.. point.GetArray(), 1.0f];
        }

        public static SKPoint3 GetCartesianPoint(this float[] coordinates)
        {
            coordinates = coordinates.Select(c => c / coordinates[^1]).ToArray();
            SKPoint3 point = new()
            {
                X = coordinates[0],
                Y = coordinates[1],
                Z = coordinates[2]
            };

            return point;
        }
    }
}