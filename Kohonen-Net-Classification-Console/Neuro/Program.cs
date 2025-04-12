using System;
using System.Linq;

namespace Neuro
{
    // Made according to the example of the program: R.V. Shamin. Lecture No. 4. Classification by the Kohonen network
    // https://github.com/rwsh/ClassK
    // https://www.youtube.com/watch?v=UUMKKtkzZWg

    class Program
    {
        static int N = 3; // размерность Х и W
        static int K = 2; // кол-во классов
        static double[,] W;
        static double[,] X;
        static int iterations = 100;
        static double lambda = 0.3; // скорость обучения
        static double delta = 0.05; // шаг изменения обучения (лямбды)

        static void Main(string[] args)
        {
            X = new double[,] 
            {
                { 1, 5, 10 },
                { 1, 5, 9},
                { 7, 9, 11},
                { 7, 9, 10}
            };

            Console.WriteLine("INITIAL DATA\n\nMatrix Х:");
            Print(X);

            X = Normalize(X); // приведение компонентов (по столбцу) к виду [0...1]

            Console.WriteLine("\nMatrix Х after data normalization:");
            Print(X);

            W = new double[K, N];
            W = Tools.FillRandoms(W, 0.1, 0.3);

            Console.WriteLine("\nМатрица W:");
            Print(W);

            Console.WriteLine($"\nTotal classes (K): {K}");

            // ************************
            //      LEARNING
            // ************************

            Console.WriteLine("\nLEARNING...");

            while (lambda > 0)
            {
                while(iterations > 0)
                {
                    // Обучение
                    for (int i = 0; i < X.GetLength(0); i++) // Проход по строкам Х
                    {
                        int index_nearest_w = GetNearest(i);

                        // Для каждого х корректируем компоненты близкого вектора w
                        for (int h = 0; h < W.GetLength(1); h++) // Проход по столбцам W
                        {
                            var y = index_nearest_w;
                            W[y, h] += lambda * (X[i, h] - W[y, h]);
                        }
                    }
                    iterations--;
                }
                lambda -= delta; // уменьшаем коэффициент обучения
            }

            // ************************
            //      КЛАССИФИКАЦИЯ
            // ************************

            Console.WriteLine("CLASSIFICATION...\n");

            for (int i = 0; i < X.GetLength(0); i++) // Проход по строкам Х
            {
                int index_nearest_w = GetNearest(i);
                Console.WriteLine($"X[{i}] belongs to the class {index_nearest_w}", i, index_nearest_w);
            }

            Console.ReadKey();
        }
        static int GetNearest(int i)
        {
            double min_dist = double.MaxValue;
            int index_min_w = 0; // индекс минимального w

            // поиск близкого вектора w
            for (int j = 0; j < W.GetLength(0); j++) // Проход по строкам W
            {
                double dist = 0;

                for (int h = 0; h < W.GetLength(1); h++) // Проход по столбцам W
                {
                    dist += (W[j, h] - X[i, h]) * (W[j, h] - X[i, h]);
                }
                dist = Math.Sqrt(dist);

                if (dist < min_dist)
                {
                    min_dist = dist;
                    index_min_w = j;
                }
            }
            return index_min_w;
        }
        static double[,] Normalize(double[,] X)
        {
            var XT = Tools.T(X);
                
            for(int i = 0; i < XT.GetLength(0); i++)
            {
                var row = Tools.GetRow(XT, i);
                var max = row.Max();
                var min = row.Min();
                var a = 1 / (max - min);
                var b = -min / (max - min);
                
                for (int j = 0; j < row.Length; j++) // цикл по столбцам
                {
                    var val = a * row[j] + b;
                    XT[i, j] = val;
                }
            }

            return Tools.T(XT);
        }
        static void Print(double[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                string s = "[";

                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    s += Math.Round(arr[i, j], 3) + "\t";
                }

                s += "]";

                Console.WriteLine(s);
            }
        }
    }
}
