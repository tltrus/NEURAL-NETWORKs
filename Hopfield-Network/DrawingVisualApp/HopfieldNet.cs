using System;
using System.Collections.Generic;


namespace DrawingVisualApp
{
    class HopfieldNet
    {
        double[,] W; // weights
        List<int[]> X = new List<int[]>(); // images store like inputs
        int[] Y; // local input for compare
        int N;

        public HopfieldNet(int size)
        {
            N = size * size;
            W = new double[N, N];
            Y = new int[N];
        }
        public void AddImage(Map map)
        {
            var x = new int[N];
            Array.Copy(map.ToArray(), x, N);
            X.Add(x);
        }
        public int Find(Map map)
        {
            var map_ = map.ToArray();
            Array.Copy(map_, Y, N);

            bool isEq = false;

            for (int t = 0; t < 50; t++)
            {
                List<int> Z = new List<int>();

                for (int i = 0; i < N; i++)
                {
                    double d = 0;
                    for (int j = 0; j < N; j++)
                    {
                        d += (W[i, j] * Y[j]);
                    }
                    if (d > 0)
                        Z.Add(1);
                    else
                        Z.Add(-1);
                }

                foreach (var x in X)
                {
                    isEq = Tools.IsEqual(x, Z);
                    if (isEq)
                    {
                        int result = X.IndexOf(x);
                        return result;
                    }
                }
                if (isEq)
                {
                    return -2; // TBC
                }
            }
            return -1;
        }
        public string Teach()
        {
            if (X.Count == 0) return "No images!";
            
            foreach (var x in X) // пробегаем по всем образам
            {
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (i == j)
                            W[i, j] = 0; // обнуление диагональных элементов
                        else
                            W[i, j] += (x[i] * x[j]); // W = X(транспонир) * X
                    }
                }
            }

            // Можно и не делить. Результат тот же. Используется для лучшей сходимости.
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    W[i, j] /= N;
                }
            }

            return "Teached!";
        }
        public int[] GetImage(int index)
        {
            if (index < X.Count)
                return X[index];

            return null;
        }

        public int GetInputsCount() => X.Count;
    }
}
