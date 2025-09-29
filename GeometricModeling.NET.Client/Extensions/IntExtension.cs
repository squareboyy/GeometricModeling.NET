namespace GeometricModeling.NET.Client.Extensions
{
    public static class IntExtension
    {
        public static double GetRadian(this int angle)
        {
            return angle * Math.PI / 180;
        }

        public static float GetRadianF(this int angle)
        {
            return angle * MathF.PI / 180;
        }
    }
}