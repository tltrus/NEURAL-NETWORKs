using System;
using System.Collections.Generic;

namespace MyMINST.Classes
{
    public class MyMatrix
    {
        private double[,] data2D;
        private double[] data1D;

        private int rows;
        private int cols;
        public int Rows { get => rows; }
        public int Cols { get => cols; }

        public MyMatrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            data2D = new double[rows, cols];
        }
        public MyMatrix(int cols)
        {
            this.cols = cols;
            data1D = new double[cols];
        }

        public void Map(Action<int, int> func)
        {
            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Cols; j++)
                    func(i, j);
        }
        public void Map(Action<int> func)
        {
            for (var j = 0; j < Cols; j++)
                func(j);
        }

        public double this[int y, int x]
        {
            get
            {
                return data2D[y, x];
            }
            set
            {
                data2D[y, x] = value;
            }
        }
        public double this[int x]
        {
            get
            {
                return data1D[x];
            }
            set
            {
                data1D[x] = value;
            }
        }

        public void Randomize(Random rnd) => Map((i, j) => this[i, j] = rnd.NextDouble());

        public static MyMatrix CreateIdentityMatrix(int n)
        {
            var result = new MyMatrix(n, n);
            for (var i = 0; i < n; i++)
            {
                result[i, i] = 1;
            }
            return result;
        }
        public MyMatrix Copy()
        {
            MyMatrix result = new MyMatrix(0, 0);
            if (is2D())
            {
                result = new MyMatrix(Rows, Cols);
                for (var i = 0; i < Rows; i++)
                    for (var j = 0; j < Cols; j++)
                        result[i, j] = this[i, j];
            }
            if (is1D())
            {
                result = new MyMatrix(Cols);
                for (var j = 0; j < Cols; j++)
                    result[j] = this[j];
            }
            return result;
        }
        public MyMatrix GetRow(int index)
        {
            MyMatrix result = new MyMatrix(Cols);

            for (int i = 0; i < Cols; i++)
                result[i] = this[index, i];
            return result;
        }


        public double Mean()
        {
            if (data1D is null)
                throw new ArgumentException("matrixe is not 1D");

            double result = 0;
            for (int i = 0; i < Cols; i++)
                result += this[i];
            return result / Cols;
        }
        public double Amax()
        {
            if (data1D is null)
                throw new ArgumentException("matrixe is not 1D");

            double result = this[0];
            for (int i = 1; i < Cols; i++)
            {
                if (this[i] > result)
                    result = this[i];
            }
            return result;
        }
        public double Amax(int row)
        {
            if (data2D is null)
                throw new ArgumentException("matrixe is not 2D");

            double result = this[0, 0];
            for (int i = 1; i < Cols; i++)
            {
                if (this[0, i] > result)
                    result = this[0, i];
            }
            return result;
        }
        public double Amin()
        {
            if (data1D is null)
                throw new ArgumentException("matrixe is not 1D");

            double result = this[0];
            for (int i = 1; i < Cols; i++)
            {
                if (this[i] < result)
                    result = this[i];
            }
            return result;
        }
        public double Sum()
        {
            var result = 0.0;
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    result += this[i, j];
            return result;
        }
        public static MyMatrix Log(MyMatrix matrix)
        {
            int Rows = matrix.Rows;
            int Cols = matrix.Cols;
            MyMatrix result = new MyMatrix(Rows, Cols);

            if (matrix.is2D())
            {
                for (var i = 0; i < Rows; i++)
                    for (var j = 0; j < Cols; j++)
                        result[i, j] = Math.Log(matrix[i, j]);
            }
            if (matrix.is1D())
            {
                for (var j = 0; j < Cols; j++)
                    result[j] = Math.Log(matrix[j]);
            }
            return result;
        }

        public MyMatrix T()
        {
            var result = new MyMatrix(Cols, Rows);
            result.Map((i, j) => result[i, j] = this[j, i]);
            return result;
        }

        private bool is2D()
        {
            if (data2D != null && data1D == null)
                return true;
            return false;
        }
        private bool is1D()
        {
            if (data1D != null && data2D == null)
                return true;
            return false;
        }
        private static bool isSizeEqual(MyMatrix matrix1, MyMatrix matrix2)
        {
            int m1Rows = matrix1.Rows;
            int m1Cols = matrix1.Cols;
            int m2Rows = matrix2.Rows;
            int m2Cols = matrix2.Cols;

            if (m1Rows == m2Rows && m1Cols == m2Cols)
                return true;
            else
                return false;
        }

        public string ToString(int padding = 4, string format = "F7")
        {
            string s = "";

            if (is1D())
            {
                for (int j = 0; j < Cols; ++j)
                {
                    var str = this[j].ToString(format);
                    s += str.PadLeft(padding) + " ";
                }
                s += Environment.NewLine;
            }
            if (is2D())
            {
                for (int i = 0; i < Rows; ++i)
                {
                    for (int j = 0; j < Cols; ++j)
                    {
                        var str = this[i, j].ToString(format);
                        s += str.PadLeft(padding) + " ";
                    }
                    s += Environment.NewLine;
                }
            }
            return s;
        }
        public void ToConsole(string format = "F7") => Console.WriteLine($"{ToString(4, format)}");

        public static MyMatrix From2DArray(double[,] m)
        {
            int Rows = m.GetLength(0);
            int Cols = m.GetLength(1);

            var result = new MyMatrix(Rows, Cols);
            result.Map((i, j) => result[i, j] = m[i, j]);
            return result;
        }
        public static MyMatrix From1DArray(double[] m)
        {
            int Cols = m.Length;

            var result = new MyMatrix(Cols);
            result.Map((j) => result[j] = m[j]);
            return result;
        }

        public double[,] To2DArray()
        {
            if (data2D is null)
                throw new ArgumentException("matrixes can not be converted");

            var result = new double[Rows, Cols];
            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Cols; j++)
                    result[i, j] = data2D[i, j];
            return result;
        }
        public double[] To1DArray()
        {
            if (data1D is null)
                throw new ArgumentException("matrixes can not be converted");

            var result = new double[Cols];
            for (var j = 0; j < Cols; j++)
                result[j] = data1D[j];
            return result;
        }

        public static MyMatrix FromArray(double[] m)
        {
            int Cols = m.Length;
            var result = new MyMatrix(Cols, 1);
            for (var j = 0; j < Cols; j++)
                result[j, 0] = m[j];
            return result;
        }
        public List<double> ToArray()
        {
            var result = new List<double>();
            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Cols; j++)
                    result.Add(data2D[i, j]);
            return result;
        }

        public static MyMatrix operator *(MyMatrix matrix, double val)
        {
            MyMatrix result = matrix.Copy();
            if (matrix.is2D())
            {
                result.Map((i, j) => result[i, j] = matrix[i, j] * val);
            }
            else
            if (matrix.is1D())
            {
                result.Map((j) => result[j] = matrix[j] * val);
            }
            return result;
        }
        public static MyMatrix operator *(MyMatrix matrix1, MyMatrix matrix2)
        {
            int m1Rows = matrix1.Rows;
            int m1Cols = matrix1.Cols;
            int m2Rows = matrix2.Rows;
            int m2Cols = matrix2.Cols;

            MyMatrix result = new MyMatrix(m1Rows, m1Cols);

            if (isSizeEqual(matrix1, matrix2))
            {
                for (int i = 0; i < m1Rows; ++i)
                {
                    for (int j = 0; j < m1Cols; ++j)
                    {
                        result[i, j] = matrix1[i, j] * matrix2[i, j];
                    }
                }
                return result;
            }

            if (matrix1.Cols != matrix2.Rows)
                throw new ArgumentException("matrixes can not be multiplied");

            if (matrix1.is1D())
            {
                result = new MyMatrix(m1Cols);
                for (int i = 0; i < m2Cols; ++i)
                {
                    for (int j = 0; j < m1Cols; ++j)
                    {
                        result[i] += matrix1[j] * matrix2[j, i];
                    }
                }
            }
            else
            if (matrix1.is2D())
            {
                result = new MyMatrix(m1Rows, m2Cols);
                for (int a = 0; a < m1Rows; ++a)
                {
                    for (int j = 0; j < m2Cols; ++j)
                        for (int k = 0; k < m1Cols; ++k)
                            result[a, j] += matrix1[a, k] * matrix2[k, j];
                };
            }
            return result;
        }

        public static MyMatrix operator +(MyMatrix matrix1, MyMatrix matrix2)
        {
            if (matrix1.Rows != matrix2.Rows || matrix1.Cols != matrix2.Cols)
            {
                throw new ArgumentException(
                    "matrixes dimensions should be equal");
            }
            var result = new MyMatrix(matrix1.Rows, matrix1.Cols);
            result.Map((i, j) =>
                result[i, j] = matrix1[i, j] + matrix2[i, j]);
            return result;
        }
        public static MyMatrix operator +(MyMatrix matrix, double val)
        {
            MyMatrix result = matrix;
            if (matrix.is2D())
            {
                result.Map((i, j) => result[i, j] = matrix[i, j] + val);
            }
            else
            if (matrix.is1D())
            {
                result.Map((j) => result[j] = matrix[j] + val);
            }
            return result;
        }

        public static MyMatrix operator -(MyMatrix matrix1, MyMatrix matrix2) => matrix1 + matrix2 * -1;
        public static MyMatrix operator -(MyMatrix matrix, double val)
        {
            MyMatrix result = matrix.Copy();
            if (matrix.is2D())
            {
                result.Map((i, j) => result[i, j] = matrix[i, j] - val);
            }
            else
            if (matrix.is1D())
            {
                result.Map((j) => result[j] = matrix[j] - val);
            }
            return result;
        }
        public static MyMatrix operator -(double val, MyMatrix matrix)
        {
            MyMatrix result = new MyMatrix(matrix.Rows, matrix.Cols);
            if (matrix.is2D())
            {
                result.Map((i, j) => result[i, j] = val - matrix[i, j]);
            }
            else
            if (matrix.is1D())
            {
                result.Map((j) => result[j] = val - matrix[j]);
            }
            return result;
        }

        public static MyMatrix operator /(MyMatrix matrix, double val)
        {
            MyMatrix result = matrix.Copy();
            if (matrix.is2D())
            {
                result.Map((i, j) => result[i, j] = matrix[i, j] / val);
            }
            else
            if (matrix.is1D())
            {
                result.Map((j) => result[j] = matrix[j] / val);
            }
            return result;
        }
    }
}
