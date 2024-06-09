using System;
using System.Collections.Generic;


namespace NN
{
    class Perceptron
    {
        Random rnd = new Random();
        int Xn; // Количество вводов (ширина массива training_inputs)
        int Rows; // Количество строк примеров для обучения (высота массива training_inputs)

        List<int[]> training_inputs = new List<int[]> { };
        List<int> training_outputs = new List<int> { };
        double[] synaptic_weights;

        public static int traininginputsNum;

        public Perceptron(int _Xn)
        {
            Xn = _Xn;

            synaptic_weights = new double[Xn];
            // инициализация весов случайными значениями от -1 до 1
            for (int i = 0; i < Xn; i++)
                synaptic_weights[i] = rnd.Next(-1000000, 1000000) / 1000000f;
        }

        static double Sigmoid(double x) => 1 / (1 + Math.Exp(-x));
        public void Fill(int[] inputs, int status)
        {
            training_inputs.Add(inputs);
            training_outputs.Add(status);
            traininginputsNum = training_inputs.Count;
            Rows = training_inputs.Count;
        }
        public void Teaching()
        {
            double[] outputs = new double[Rows];
            double[] err = new double[Rows];
            double[] adjustments = new double[Xn];
            double[] aux_adjustments = new double[Rows];

            // Back propagation
            for (int k = 1; k <= 20000; k++)
            {
                for (int i = 0; i < Rows; i++)
                {
                    double aux_outputs = 0;
                    for (int j = 0; j < Xn; j++)
                    {
                        aux_outputs += training_inputs[i][j] * synaptic_weights[j];
                    }
                    outputs[i] = Sigmoid(aux_outputs);

                    err[i] = training_outputs[i] - outputs[i];
                    aux_adjustments[i] = err[i] * (outputs[i] * (1 - outputs[i]));
                }

                // Не делаем транспонирование. Просто пробегаем циклами по матрицам 
                // и находим регулироовку, чтобы сложить ее с текущими весами
                for (int j = 0; j < Xn; j++)
                {
                    double temp = 0;
                    for (int i = 0; i < Rows; i++)
                    {
                        temp += training_inputs[i][j] * aux_adjustments[i];
                    }
                    adjustments[j] = temp;

                    // Обновляем веса
                    synaptic_weights[j] += adjustments[j];
                }
            } // k <= 20000
        } // Teaching()
        public double Identify(int[] new_inputs)
        {
            double output;
            double x = 0;
            for (int i = 0; i < Xn; i++)
            {
                x += new_inputs[i] * synaptic_weights[i];
            }
            output = Sigmoid(x);

            return output;
        }
    }
}
