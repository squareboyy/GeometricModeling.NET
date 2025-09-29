using GeometricModeling.NET.Client.Enums;

namespace GeometricModeling.NET.Client.Services.Interfaces
{
    public interface IOrthographicProjectionMatrixService
    {
        public float[,] GetMatrixProjectionOnPlane(ProjectionPlane zeroPlane);
    }
}