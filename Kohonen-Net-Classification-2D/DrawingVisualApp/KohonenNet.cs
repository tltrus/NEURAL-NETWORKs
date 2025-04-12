using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrawingVisualApp
{
    class KohonenNet
    {
        int N = 2; // размерность Х и W
        int K = 2; // кол-во классов
        double[,] W;
        double[,] X;
        int iterations = 100;
        double lambda = 0.3; // скорость обучения
        double delta = 0.05; // шаг изменения обучения (лямбды)

        public KohonenNet()
        {

        }
        public void Init(List<Point2D> points, int K)
        {
            X = points.To2DArray(); // Matrix X
            X = Normalize(X); // приведение компонентов (по столбцу) к виду [0...1]

            this.K = K;
            W = new double[K, N];
            W = Tools.FillRandoms(W, 0.1, 0.3);
        }
        public void Learning()
        {
            while (lambda > 0)
            {
                while (iterations > 0)
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
        }
        public void Classify(List<Point2D> points)
        {
            for (int i = 0; i < X.GetLength(0); i++) // Проход по строкам Х
            {
                int index_nearest_w = GetNearest(i);
                points[i].k = index_nearest_w;
            }
        }
        int GetNearest(int i)
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
        double[,] Normalize(double[,] X)
        {
            var XT = Tools.T(X);

            for (int i = 0; i < XT.GetLength(0); i++)
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
    }
}
