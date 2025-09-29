using GeometricModeling.NET.Client.Services.Interfaces;
using GeometricModeling.NET.Client.Extensions;
using static System.MathF;

namespace GeometricModeling.NET.Client.Services
{
    public class RotationTransformMatrixService : IRotationTransformMatrixService
    {
        public float[,] GetMatrixAroundAxisX(float f)
        {
            float[,] transformMatrix =
            {
                {1, 0, 0, 0},
                {0, Cos(f), -Sin(f), 0},
                {0, Sin(f), Cos(f), 0},
                {0, 0, 0, 1}
            };

            return transformMatrix;
        }

        public float[,] GetMatrixAroundAxisX(int angle)
        {
            float f = angle.GetRadianF();

            return GetMatrixAroundAxisX(f);
        }

        public float[,] GetMatrixAroundAxisY(float f)
        {
            float[,] transformMatrix =
            {
                {Cos(f), 0, Sin(f), 0},
                {0, 1, 0, 0},
                {-Sin(f), 0, Cos(f), 0},
                {0, 0, 0, 1}
            };

            return transformMatrix;
        }

        public float[,] GetMatrixAroundAxisY(int angle)
        {
            float f = angle.GetRadianF();

            return GetMatrixAroundAxisY(f);
        }

        public float[,] GetMatrixAroundAxisZ(float f)
        {
            float[,] transformMatrix =
            {
                {Cos(f), -Sin(f), 0, 0},
                {Sin(f), Cos(f), 0, 0},
                {0, 0, 1, 0},
                {0, 0, 0, 1}
            };

            return transformMatrix;
        }

        public float[,] GetMatrixAroundAxisZ(int angle)
        {
            float f = angle.GetRadianF();

            return GetMatrixAroundAxisZ(f);
        }

        public float[,] GetMatrixAroundAxisXYZ(int angleX, int angleY, int angleZ)
        {
            float[,] rotationAroundX = GetMatrixAroundAxisX(angleX);
            float[,] rotationAroundY = GetMatrixAroundAxisY(angleY);
            float[,] rotationAroundZ = GetMatrixAroundAxisZ(angleZ);

            return rotationAroundX.Multiply(rotationAroundY).Multiply(rotationAroundZ);
        }
    }
}
