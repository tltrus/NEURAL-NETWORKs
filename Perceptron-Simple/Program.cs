using System;


namespace NeuroConsole
{
    class Program
    {
        static Random rnd = new Random();
        static Matrix2d training_inputs;
        static Matrix1d training_outputs;

        static void Main(string[] args)
        {
            // ***********************************************************
            //                          INITIAL DATA
            // ***********************************************************

            // training inputs
            double[,] matrix = {
                                    { 0,0,0,1 },
                                    { 0,1,1,1 },
                                    { 0,1,0,1 },
                                    { 0,0,1,1 }
                                };
            training_inputs = new Matrix2d(matrix);
            Console.WriteLine("training inputs:");
            training_inputs.ToConsole();
            var neurons_num = training_inputs.Length(1);

            // training outputs
            training_outputs = new Matrix1d(new double[] { 0, 1, 1, 0 }); // vector
            Console.WriteLine("training outputs:");
            training_outputs.ToConsole();

            Perceptron neuron = new Perceptron(neurons_num);

            // random weights init from -1 to 1
            for (int i = 0; i < neurons_num; i++)
                neuron.weights[i] = rnd.Next(-1000000, 1000000) / 1000000f;

            Console.WriteLine("\nRandom weights:");
            neuron.weights.ToConsole();

            // ***********************************************************
            //                             LEARNING
            // ***********************************************************

            neuron.Training(training_inputs, training_outputs);

            Console.WriteLine("\nWeights after learning:");
            neuron.weights.ToConsole();

            Console.WriteLine("\nOutputs after learning:");
            neuron.outputs.ToConsole();

            // ***********************************************************
            //                             TEST
            // ***********************************************************

            double[] vector = { 0, 1, 0, 0 };
            var test_inputs = new Matrix1d(vector);
            Console.WriteLine("\nTest input: {" + test_inputs.ToString() + "}");
            Console.WriteLine("Result output: " + neuron.Calculate(test_inputs));

            Console.ReadKey();
        }
    }
}
