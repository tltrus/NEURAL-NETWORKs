using System;


namespace NeuroConsole
{
    class Perceptron
    {
        public Matrix1d weights;
        public Matrix1d outputs;


        public Perceptron(int weights_num)
        {
            weights = new Matrix1d(weights_num);
        }

        // Sigmoid func
        private double Sigmoid(double x) => 1 / (1 + Math.Exp(-x));
        private Matrix1d Sigmoid(Matrix2d matrix)
        {
            var len = matrix.Length(0);
            Matrix1d result = new Matrix1d(len);
            for(int i = 0; i < len; i++)
            {
                result[i] = 1 / (1 + Math.Exp(-matrix[i, 0]));
            }
            return result;
        }

        // Derivative of Sigmoid
        private Matrix1d SigmoidDerivative(Matrix1d matrix)
        {
            var len = matrix.Length;
            Matrix1d result = new Matrix1d(len);
            for (int i = 0; i < len; i++)
            {
                result[i] = matrix[i] * (1.0 - matrix[i]);
            }
            return result;
        }

        public double Calculate(Matrix1d inputs)
        {
            var x = (inputs * weights).Sum();
            return Sigmoid(x);
        }

        // Learning
        public void Training(Matrix2d training_inputs, Matrix1d training_outputs)
        {
            // Back propagation
            for (int k = 0; k < 20000; k++)
            {
                var weightsT = weights.T;
                var aux_outputs = training_inputs * weightsT;
                outputs = Sigmoid(aux_outputs);
                var err = training_outputs - outputs;
                var aux_adjustments = err * SigmoidDerivative(outputs);
                
                var adjustments = aux_adjustments * training_inputs;
                weights += adjustments;
            }
        }
    }
}
