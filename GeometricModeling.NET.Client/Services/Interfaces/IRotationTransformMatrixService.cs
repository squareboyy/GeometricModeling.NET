namespace GeometricModeling.NET.Client.Services.Interfaces
{
    public interface IRotationTransformMatrixService
    {
        public float[,] GetMatrixAroundAxisX(int angle);
        public float[,] GetMatrixAroundAxisX(float f);
        public float[,] GetMatrixAroundAxisY(int angle);
        public float[,] GetMatrixAroundAxisY(float f);
        public float[,] GetMatrixAroundAxisZ(int angle);
        public float[,] GetMatrixAroundAxisZ(float f);
        public float[,] GetMatrixAroundAxisXYZ(int angleX, int angleY, int angleZ);
    }
}
