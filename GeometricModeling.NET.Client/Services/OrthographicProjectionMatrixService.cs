using GeometricModeling.NET.Client.Enums;
using GeometricModeling.NET.Client.Services.Interfaces;

namespace GeometricModeling.NET.Client.Services
{
    public class OrthographicProjectionMatrixService : IOrthographicProjectionMatrixService
    {
        public float[,] GetMatrixProjectionOnPlane(ProjectionPlane zeroPlane)
        {
            byte x = 1, y = 1, z = 1;

            if (zeroPlane == ProjectionPlane.X) { x = 0; }
            else if (zeroPlane == ProjectionPlane.Y) { y = 0; }
            else if (zeroPlane == ProjectionPlane.Z) { z = 0; }

            float[,] projectionMatrix =
            {
                {x, 0, 0, 0},
                {0, y, 0, 0},
                {0, 0, z, 0},
                {0, 0, 0, 1}
            };

            return projectionMatrix;
        }
    }
}
