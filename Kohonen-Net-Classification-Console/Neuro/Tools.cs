

using System;

namespace Neuro
{
    public static class Tools
    {
        private static Random random = new Random();
        
        public static T[,] T<T>(T[,] m)
        {
            T[,] array = new T[m.GetLength(1), m.GetLength(0)];
            for (int i = 0; i < m.GetLength(1); i++)
            {
                for (int j = 0; j < m.GetLength(0); j++)
                {
                    array[i, j] = m[j, i];
                }
            }
            return array;
        }

        public static T[] GetRow<T>(T[,] m, int index)
        {
            T[] array = new T[m.GetLength(1)];
            for (int i = 0; i < m.GetLength(1); i++)
            {
                array[i] = m[index, i];
            }
            return array;
        }

        public static double[,] FillRandoms(double[,] m, double minNumber, double maxNumber)
        {
            int length = m.GetLength(0);
            int length2 = m.GetLength(1);
            double[,] array = new double[length, length2];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    array[i, j] = random.NextDoubleRange(minNumber, maxNumber);
                }
            }

            return array;
        }

        public static double NextDoubleRange(this Random random, double minNumber, double maxNumber)
        {
            return random.NextDouble() * (maxNumber - minNumber) + minNumber;
        }
    }
}
