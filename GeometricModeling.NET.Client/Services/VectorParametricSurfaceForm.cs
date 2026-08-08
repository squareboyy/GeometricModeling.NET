using GeometricModeling.NET.Client.Services.Interfaces;
using SkiaSharp;
using static System.MathF;

namespace GeometricModeling.NET.Client.Services
{
    public class VectorParametricSurfaceForm : ISurfaceService
    {
        //може бути різна кількість відрізків для u-v
        public List<SKPoint3[]> GetProjectiveEllipsoid(int a, int b, int c, int stepsNumber, float maxRadU, float maxRadV, float minRadU = 0f, float minRadV = 0f)
        {
            List<SKPoint3[]> result = [];

            SetGridSurfaceValues(maxRadU, minRadU, maxRadV, minRadV);
            SetGridSurfaceValues(maxRadV, minRadV, maxRadU, minRadU, true);

            return result;

            void SetGridSurfaceValues(float firstMaxAngleRad, float firstMinAngleRad, float secondMaxAngleRad, float secondMinAngleRad, bool reverse = false)
            {
                float firstStep = firstMaxAngleRad / stepsNumber;
                float secondStep = secondMaxAngleRad / stepsNumber;

                for (float i = firstMinAngleRad, firstCounter = 0; firstCounter < stepsNumber; i += firstStep, ++firstCounter)
                {
                    SKPoint3[] points = new SKPoint3[stepsNumber];
                    int secondCounter = 0;
                   
                    for (float j = secondMinAngleRad; secondCounter < stepsNumber; j += secondStep, ++secondCounter)
                    {
                        float x, y, z;

                        if (reverse)
                        {
                             x = a * Cos(i) * Cos(j);
                             y = b * Cos(i) * Sin(j);
                             z = c * Sin(i);
                        }
                        else
                        {
                             x = a * Cos(j) * Cos(i);
                             y = b * Cos(j) * Sin(i);
                             z = c * Sin(j);
                        }
                        
                        points[secondCounter] = new SKPoint3(x, y, z);
                    }

                    result.Add(points);
                }
            }
        }
    }
}
