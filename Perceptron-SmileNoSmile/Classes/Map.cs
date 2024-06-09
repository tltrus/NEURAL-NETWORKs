using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Security.RightsManagement;


namespace NN
{
    class Map
    {
        public int rows, cols;

        int[,] matrix;

        public int cell_width = 15;
        int marginTop = 16;
        int marginLeft = 16;
        int offs_x = 0;
        int offs_y = 0;

        public Map(int rows = 6, int cols = 6)
        {
            this.rows = rows;
            this.cols = cols;
            matrix = new int[rows, cols];
        }

        public int this[int i, int j]
        {
            get => matrix[i, j];
            set => matrix[i, j] = value;
        }

        public int GetLength(int a)
        {
            if (a == 0)
                return matrix.GetLength(0);
            else if (a == 1)
                return matrix.GetLength(1);
            return -1;
        }


        public void Draw(Canvas canvas)
        {
            for (int y = 0; y < rows; y++)
            {
                offs_y = (y - 1) * cell_width + marginTop;
                for (int x = 0; x < cols; x++)
                {
                    offs_x = (x - 1) * cell_width + marginLeft;

                    Rectangle cell = new Rectangle()
                    {
                        Width = cell_width,
                        Height = cell_width,
                        Fill = Brushes.Gray,
                        Stroke = Brushes.Black,
                        StrokeThickness = 0.1
                    };

                    if (matrix[y, x] == 0)
                    {
                        cell.Fill = Brushes.White;
                    }
                    else

                    if (matrix[y, x] == 1)
                    {
                        cell.Fill = Brushes.Green;
                    }

                    canvas.Children.Add(cell);

                    Canvas.SetLeft(cell, offs_x);
                    Canvas.SetTop(cell, offs_y);
                }
            }
        }

        public void Clear() => matrix = new int[rows, cols];
    }
}

