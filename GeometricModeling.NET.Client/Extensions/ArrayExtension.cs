namespace GeometricModeling.NET.Client.Extensions
{
    public static class ArrayExtension
    {
        public static float[] Multiply(this float[] array, float[,] array2D)
        {
            if (array.Length != array2D.GetUpperBound(0) + 1)
            {
                throw new ArgumentException();
            }

            float[] result = new float[array2D.GetUpperBound(1) + 1];

            for (int i = 0; i < array2D.GetUpperBound(1) + 1; i++)
            {
                for (int j = 0; j < array2D.GetUpperBound(0) + 1; j++)
                {
                    result[i] += array2D[j, i] * array[j];
                }
            }

            return result;
        }

        public static float[,] Multiply(this float[,] matrixA, float[,] matrixB)
        {
            if (matrixA.GetLength(1) != matrixB.GetLength(0))
                throw new InvalidOperationException("Кількість стовпців першої матриці має дорівнювати кількості рядків другої матриці");

            int rows = matrixA.GetLength(0);
            int columns = matrixB.GetLength(1);
            var result = new float[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    for (int k = 0; k < matrixA.GetLength(1); k++)
                    {
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            }

            return result;
        }
    }
}