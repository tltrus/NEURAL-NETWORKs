using System;
using System.Collections.Generic;


namespace Neuro
{
    class Program
    {
        static int M; // {x0, x1, x2 ....xm}
        static int K; // {X0, X1, X2 ... Xk}

        static double[,] W1; // weights of layer 1
        static double[,] W2; // weights of layer 2

        static List<int[]> Xref = new List<int[]>(); // reference samples for layer 1
        static double[] X2; // inputs for layer 2
        static int[] x;     // searched vector

        static double[] S; // Layer 1 result
        static double[] Y;  // Layer 2 result

        static List<int> Z;

        static int L = 5;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Xref.Add(new int[] { -1, 1, -1, 1, -1 });
            Xref.Add(new int[] { -1, 1, 1, 1, -1 });
            Xref.Add(new int[] { 1, -1, -1, -1, 1 });

            x = new int[] { 1, -1, -1, 1, 1 };

            K = Xref.Count;         // number of reference samples
            M = Xref[0].Length;     // number of X elements (x)

            W1 = new double[K, M];
            W2 = new double[K, K];

            S = new double[K];
            X2 = new double[K];
            Y = new double[K];

            Console.Write("X =\n");
            Print(Xref);
            Console.Write("\nx =\n");
            Print(x);

            Learning();

            Console.WriteLine("\nW1 = ");
            Print(W1);
            Console.WriteLine("\nW2 = ");
            Print(W2);

            Z = new List<int>();

            Identify();

            Console.WriteLine("\nY = ");
            Print(Y);

            Console.Write("\nResult: ");
            int result = GetReferenceSample(Y);
            Console.Write("x corresponds to sample X[" + result.ToString() + "]");

            Console.ReadKey();
        }

        static void Learning()
        {
            //1st layer
            for (int i = 0; i < K; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    W1[i, j] = Xref[i][j] * 0.5;
                }
            }

            //2nd layer
            var e = rnd.NextDoubleRange(0.01, 1 / K, 3);
            for (int i = 0; i < K; i++)
            {
                for (int j = 0; j < K; j++)
                {
                    if (i == j)
                        W2[i, j] = 1;
                    else
                        W2[i, j] = -e;
                }
            }
        }

        static double f(double s)
        {
            double T = M / 2;
            
            if (s <= 0) 
                return 0; // s <= 0
            else
            if (s > 0 && s <= T)
                return s; // 0 < s <= T
            else 
                return T; // s > T
        }

        static void Identify()
        {
            // Layer 1
            for (int i = 0; i < K; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    S[i] += W1[i, j] * x[j];
                }
            }

            // activation f of layer 1
            for (int i = 0; i < K; i++)
            {
                X2[i] = f(S[i]);
            }

            // Layer 2
            for (int l = 0; l < L; l++) // iteration = 5
            {
                // Y
                for (int i = 0; i < K; i++)
                {
                    for (int j = 0; j < K; j++)
                    {
                        Y[i] += W2[i, j] * X2[j];
                    }
                }
                Array.Copy(Y, X2, K);
            }
        }

        static int GetReferenceSample(double[] Y)
        {
            double max = 0;
            int id = 0;

            for (int i = 0; i < Y.Length; ++i)
            {
                if (Y[i] > max)
                {
                    max = Y[i];
                    id = i;
                }
            }
            return id;
        }

        static void Print(int[] arr)
        {
            string s = "[";
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                s += arr[i] + "\t";
            }
            s += "]";
            Console.WriteLine(s);
        }
        static void Print(double[] arr)
        {
            string s = "[";
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                s += Math.Round(arr[i], 1) + "\t";
            }
            s += "]";
            Console.WriteLine(s);
        }
        static void Print(List<int[]> list)
        {
            for(int i = 0; i < list.Count; ++i)
            {
                Print(list[i]);
            }
        }
        static void Print(double[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                string s = "[";

                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    s += arr[i, j] + "\t";
                }

                s += "]";

                Console.WriteLine(s);
            }
        }

    }
}
