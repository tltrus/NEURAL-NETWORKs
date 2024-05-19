using System;
using System.Collections.Generic;
using System.Text;

namespace MyMINST.Classes
{
    internal static class Extentions
    {
        public static double Sum(this double[,] array)
        {
            var result = 0.0;
            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    result += array[i, j];
            return result;
        }
    }
}
