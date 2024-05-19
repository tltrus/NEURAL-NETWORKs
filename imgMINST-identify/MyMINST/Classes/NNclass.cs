using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMINST.Classes
{
    public class NNclass
    {
        int input_nodes;
        int[] hidden_nodes;
        int output_nodes;

        public MyMatrix weights_ih;    // между входами и первым скрытым слоем
        public MyMatrix[] weights_hh;  // между скрытыми слоями
        public MyMatrix weights_ho;    // между последним скрытым слоем и выходами

        public MyMatrix[] bias_h;
        public MyMatrix bias_o;

        public MyMatrix output;         // вектор-столбец [..., 0]
        public MyMatrix[] hiddens;

        public MyMatrix output_errors;

        double learning_rate;

        delegate double ActivationFuncHandler(double val);
        class ActivationFunction
        {
            public ActivationFuncHandler func, dfunc;
            public ActivationFunction(ActivationFuncHandler func, ActivationFuncHandler dfunc)
            {
                this.func = func;
                this.dfunc = dfunc;
            }
        }
        ActivationFunction activation_function;
        ActivationFunction[] activations = new ActivationFunction[]
        {
                new ActivationFunction(     // Sigmoid
                    x => 1 / (1 + Math.Exp(-x)),
                    y => y * (1 - y)
                    ),
                new ActivationFunction(     // Tangens
                    x => Math.Tanh(x),
                    y => 1 - y * y
                    ),
                new ActivationFunction(     // ReLU
                    x => Math.Max(0, x),
                    y => y > 0 ? 1 : 0    // return 1 if y > 0, else 0
                ),
                new ActivationFunction(     // LeakyReLU
                    x => Math.Max(0.01 * x, x),
                    y => y > 0 ? 1 : 0.01    // return 1 if y > 0, else 0.01
                )
        };

        private MyMatrix SoftMax(MyMatrix x)
        {
            MyMatrix result = x.Copy();

            double denom = 0.0;
            var max = x.T().Amax(0); // поиск максимального элемента в 0-й строке. Стабильный Softmax
            var shiftx = x - max;
            for (var i = 0; i < x.Rows; i++)
                denom += Math.Exp(shiftx[i, 0]);     // знаменатель

            double checksum = 0.0;
            for (var i = 0; i < shiftx.Rows; i++)
            {
                result[i, 0] = Math.Exp(shiftx[i, 0]) / denom; // делим каждый элемент на знаменатель
                checksum += result[i, 0];
            }
            return result;

        } // SoftMax
        private MyMatrix SoftMaxDeriv(MyMatrix layer)
        {
            MyMatrix result = layer.Copy();

            for (var i = 0; i < result.Rows; i++)
            {
                double y = layer[i, 0];
                result[i, 0] = y * (1 - y); // производная
            }
            return result;
        } // SoftMax derivative

        public enum ActivateFunctions
        {
            Sigmoid = 0,
            Tanh = 1,
            ReLU = 2,
            LeakyReLU = 3
        }

        public NNclass(int in_nodes, int[] hid_nodes, int out_nodes, Random rnd)
        {
            activation_function = new ActivationFunction(activations[0].func, activations[0].dfunc);

            input_nodes = in_nodes;
            hidden_nodes = hid_nodes; // в массиве хранится кол-во узлов каждого скрытого слоя
            output_nodes = out_nodes;

            weights_ih = new MyMatrix(hidden_nodes[0], input_nodes); // вес между входным и первым скрытым слоем
            weights_ih.Randomize(rnd);

            weights_hh = new MyMatrix[hid_nodes.Length - 1];
            for (int i = 0; i < weights_hh.Length; i++)
            {
                weights_hh[i] = new MyMatrix(hidden_nodes[i + 1], hidden_nodes[i]);
                weights_hh[i].Randomize(rnd);
            }

            weights_ho = new MyMatrix(output_nodes, hidden_nodes.Last());
            weights_ho.Randomize(rnd);

            hiddens = new MyMatrix[hid_nodes.Length];

            bias_h = new MyMatrix[hid_nodes.Length];
            for (int i = 0; i < bias_h.Length; i++)
            {
                bias_h[i] = new MyMatrix(hidden_nodes[i], 1);
                bias_h[i].Randomize(rnd);
            }

            bias_o = new MyMatrix(output_nodes, 1);
            bias_o.Randomize(rnd);

            SetActivationFunction(ActivateFunctions.Sigmoid);
            SetLearningRate();
        }

        public void SetLearningRate(double learning_rate = 0.1) => this.learning_rate = learning_rate;
        public void SetActivationFunction(ActivateFunctions func) => activation_function = activations[(int)func];
        public void Train(double[] input_array, double[] target_array)
        {
            MyMatrix inputs = MyMatrix.FromArray(input_array);
            MyMatrix targets = MyMatrix.FromArray(target_array);

            FeedForward(input_array);

            // --------------------------------------------------------------------------------

            // ERROR = TARGETS - OUTPUTS
            output_errors = targets - output;

            // Calculate gradient
            MyMatrix gradients = new MyMatrix(output.Rows, output.Cols);
            gradients = SoftMaxDeriv(output);
            gradients *= output_errors * learning_rate;

            weights_ho += gradients * hiddens.Last().T();
            bias_o += gradients;

            MyMatrix weights_next = weights_ho.Copy();
            MyMatrix errors_next = output_errors.Copy();

            for (int k = weights_hh.Length; k > 0; k--) // Цикл в обратную сторону
            {
                // Calculate the hidden layer errors
                MyMatrix hh_errors = weights_next.T() * errors_next;
                // Calculate gradient
                MyMatrix hh_gradient = new MyMatrix(hiddens[k].Rows, hiddens[k].Cols);
                hh_gradient.Map((i, j) => hh_gradient[i, j] = activation_function.dfunc(hiddens[k][i, j]));
                hh_gradient *= hh_errors * learning_rate;

                weights_hh[k - 1] += hh_gradient * hiddens[k - 1].T();
                bias_h[k] += hh_gradient;

                weights_next = weights_hh[k - 1].Copy();
                errors_next = hh_errors.Copy();
            }

            // Calculate the hidden 0 layer errors
            MyMatrix h1_errors = weights_next.T() * errors_next;
            // Calculate gradient
            MyMatrix h1_gradient = new MyMatrix(hiddens[0].Rows, hiddens[0].Cols);
            h1_gradient.Map((i, j) => h1_gradient[i, j] = activation_function.dfunc(hiddens[0][i, j]));
            h1_gradient *= h1_errors * learning_rate;

            weights_ih += h1_gradient * inputs.T();
            bias_h[0] += h1_gradient;
        }
        private MyMatrix FeedForward(double[] input_array)
        {
            MyMatrix inputs = MyMatrix.FromArray(input_array);

            // HIDDEN 0
            hiddens[0] = weights_ih * inputs;
            hiddens[0] += bias_h[0];
            hiddens[0].Map((i, j) => hiddens[0][i, j] = activation_function.func(hiddens[0][i, j]));

            // HIDDEN 1...n
            for (int k = 1; k < hiddens.Length; k++)
            {
                hiddens[k] = weights_hh[k - 1] * hiddens[k - 1];
                hiddens[k] += bias_h[k];
                hiddens[k].Map((i, j) => hiddens[k][i, j] = activation_function.func(hiddens[k][i, j]));
            }

            // OUT LAYER
            output = weights_ho * hiddens.Last();
            output += bias_o;
            output.Map((i, j) => output[i, j] = activation_function.func(output[i, j]));

            return output;
        }
        public List<double> Calculate(double[] input_array) => FeedForward(input_array).ToArray();
    }
}
