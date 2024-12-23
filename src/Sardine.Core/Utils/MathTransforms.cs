namespace Sardine.Utils.Arithmetic
{
    public static class MathTransforms
    {
        public static double RadianToDegreeConstant => 180 / Math.PI;
        public static double RadianToDegree(double x) => x * RadianToDegreeConstant;

        public static double[] LinearInterpolation(double start, double end, int nPoints)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(nPoints, 2);

            double[] interpolationResult = new double[nPoints];
            double step = (end - start) / (nPoints - 1);
            interpolationResult[0] = start;
            for (int i = 1; i < nPoints; i++)
            {
                interpolationResult[i] = interpolationResult[i - 1] + step;
            }

            return interpolationResult;
        }

        public static bool NoneIsNULL(object?[] array)
        {
            if (array is null)
            {
                return false;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static T[]? GrowAndCloneArray<T>(T[]? arrayIn)
        {
            if (arrayIn is null)
            {
                return null;
            }

            T[] arrayOut = new T[arrayIn.Length + 1];

            for (int i = 0; i < arrayIn.Length; i++)
            {
                arrayOut[i] = arrayIn[i];
            }

            return arrayOut;
        }
    }
}
