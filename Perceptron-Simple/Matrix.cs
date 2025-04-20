using System;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroConsole
{
    class Matrix1d
    {
        private double[] data;
        public int Length { get => data.Length; }
        public Matrix2d T
        {
            get
            {
                int len = Length;

                Matrix2d result = new Matrix2d(len, 1);

                for (int i = 0; i < len; ++i)
                {
                    result[i, 0] = data[i];
                }
                return result;
            }
        }

        public Matrix1d(int length)
        {
            data = new double[length];
        }
        public Matrix1d(double[] array)
        {
            int len = array.Length;
            data = new double[len];
            Array.Copy(array, data, len);
        }

        public double this[int index]
        {
            get => data[index];
            set => data[index] = value;
        }
        public double Sum() => data.Sum();
        public string ToString(int padding = 4)
        {
            string text = "";
            for (int i = 0; i < data.GetLength(0); i++)
            {
                string text2 = (string)Convert.ChangeType(Math.Round(data[i], 4), typeof(string));
                text = text + text2.PadLeft(padding) + " ";
                
            }
            return text;
        }
        public void ToConsole() => Console.WriteLine(ToString());
        public static Matrix1d operator +(Matrix1d a, Matrix1d b)
        {
            int aLen = a.Length;
            int bLen = b.Length;
            if (aLen != bLen) throw new Exception("Length of arrays is not same");

            Matrix1d result = new Matrix1d(aLen);
            for (int i = 0; i < aLen; ++i)
            {
                result[i] = a[i] + b[i];
            }
            return result;
        }
        public static Matrix1d operator -(Matrix1d a, Matrix1d b)
        {
            int aLen = a.Length;
            int bLen = b.Length;
            if (aLen != bLen) throw new Exception("Length of arrays is not same");

            Matrix1d result = new Matrix1d(aLen);
            for (int i = 0; i < aLen; ++i)
            {
                result[i] = a[i] - b[i];
            }
            return result;
        }
        public static Matrix1d operator *(Matrix1d a, Matrix1d b)
        {
            int aLen = a.Length;
            int bLen = b.Length;
            if (aLen != bLen) throw new Exception("Length of arrays is not same");

            Matrix1d result = new Matrix1d(aLen);
            for (int i = 0; i < aLen; ++i)
            {
                result[i] = a[i] * b[i];
            }
            return result;
        }
        public static Matrix1d operator *(Matrix1d a, Matrix2d b)
        {
            int bRows = b.Length(0);
            int bCols = b.Length(1);
            int num = 1;
            int aLen = a.Length;
            if (bRows != aLen)
            {
                throw new Exception("Non-conformable matrices in MatrixProduct");
            }

            Matrix1d result = new Matrix1d(aLen);
            for (int i = 0; i < bCols; i++)
            {
                for (int j = 0; j < aLen; j++)
                {
                    result[i] += a[j] * b[j, i];
                }
            }

            return result;
        }
    }

    class Matrix2d
    {
        private double[,] data;

        public Matrix2d(int rows, int cols)
        {
            data = new double[rows, cols];
        }
        public Matrix2d(double[,] matrix)
        {
            data = matrix.Clone() as double[,];
        }

        public int Length(int axis)
        {
            switch(axis)
            {
                case 0:
                    return data.GetLength(0);
                    break;
                case 1:
                    return data.GetLength(1);
                    break;
            }
            return -1;
        }

        public double this[int row, int col]
        {
            get => data[row, col];
            set => data[row, col] = value;
        }
        public string ToString(int padding = 4)
        {
            string text = "";
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    string text2 = (string)Convert.ChangeType(data[i, j], typeof(string));
                    text = text + text2.PadLeft(padding) + " ";
                }
                text += Environment.NewLine;
            }
            return text;
        }
        public void ToConsole() => Console.WriteLine(ToString());
        public static Matrix2d operator *(Matrix2d a, Matrix2d b)
        {
            int aRows = a.Length(0);
            int aCols = a.Length(1);
            int bRows = b.Length(0);
            int bCols = b.Length(1);

            if (aCols != bRows)
            {
                throw new Exception("Non-conformable matrices in MatrixProduct");
            }

            Matrix2d result = new Matrix2d(aRows, bCols);
            Parallel.For(0, aRows, delegate (int i)
            {
                for (int j = 0; j < bCols; j++)
                {
                    for (int k = 0; k < aCols; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            });
            return result;
        }
    }
}
