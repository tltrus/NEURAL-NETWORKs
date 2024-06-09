using System;
using System.Windows;
using System.Windows.Input;


namespace NN
{

    public partial class MainWindow : Window
    {
        int oldnumcell = 0;
        Perceptron perceptron;
        int inputs_num = 36; // inputs 6х6
        Map Map = new Map();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            Map.Clear();

            perceptron = new Perceptron(inputs_num);

            Files.FileRead(Map, "data.txt");

            MapDrawing();
        }

        private void MapDrawing()
        {
            canvas.Children.Clear();
            Map.Draw(canvas);
        }

        void CellBotOnOff(MouseEventArgs e)
        {
            Point p = e.GetPosition(canvas);

            int X = (int)p.X / Map.cell_width;
            int Y = (int)p.Y / Map.cell_width;

            if ((X > Map.cols - 1) || (Y > Map.rows - 1)) return;

            Map[Y, X] = Map[Y, X] == 1 ? 0: 1; // switch on\off

            MapDrawing();
        }


        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(canvas);

                int X = (int)p.X / Map.cell_width;
                int Y = (int)p.Y / Map.cell_width;

                int index = (Y * Map.cols) + X;

                if (index != oldnumcell)
                {
                    CellBotOnOff(e);
                    oldnumcell = index; // запоминаем индекс клетки
                }
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Point p = e.GetPosition(canvas);

            int X = (int)p.X / Map.cell_width;
            int Y = (int)p.Y / Map.cell_width;

            oldnumcell = (Y * Map.cols) + X;

            CellBotOnOff(e);
        }

        private void BtnIdent_Click(object sender, RoutedEventArgs e)
        {
            int[] inputs = new int[inputs_num];
            for (int y = 0; y < Map.GetLength(0); y++)
            {
                for (int x = 0; x < Map.GetLength(1); x++)
                {
                    int indx = y * 6 + x;
                    inputs[indx] = Map[y, x];
                }
            }

            double rez = perceptron.Identify(inputs);
            if (rez > 0.9) lbl1.Content = "Smile";
                else lbl1.Content = "No smile";
        }
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Map.Clear();
            Map.Draw(canvas);
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e) => Files.SaveToFile(Map, "data.txt");
        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            Files.FileRead(Map, "data.txt");
            MapDrawing();
        }
        private void BtnTeach_Click(object sender, RoutedEventArgs e) => perceptron.Teaching();

        private void BtnAddSmile_Click(object sender, RoutedEventArgs e)
        {
            int[] inputs = TrainingInputsFill();
            perceptron.Fill(inputs, 1); // 1 - smile
            lblList.Content = Perceptron.traininginputsNum;
        }
        private void BtnAddNone_Click(object sender, RoutedEventArgs e)
        {
            int[] inputs = TrainingInputsFill();
            perceptron.Fill(inputs, 0); // 0 - no smile
            lblList.Content = Perceptron.traininginputsNum;
        }
        private int[] TrainingInputsFill()
        {
            int[] inputs = new int[inputs_num];
            for (int y = 0; y < Map.GetLength(0); y++)
            {
                for (int x = 0; x < Map.GetLength(1); x++)
                {
                    int indx = y * 6 + x;
                    inputs[indx] = Map[y, x];
                }
            }

            return inputs;
        }
    }
}
