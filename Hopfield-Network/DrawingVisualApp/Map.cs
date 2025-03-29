using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace DrawingVisualApp
{
    class Map
    {
        public int rows;    // Количество строк матрицы
        public int cols;   // Количество столбцов матрицы

        public static int memoryCellindex;
        public int[,] map;
        public int cellWidth = 26;

        public Map(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;

            map = new int[rows, cols];
        }
        public void Reset()
        {
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    map[y, x] = -1;
        }
        public void Toggle(Point point)
        {
            int X = (int)point.X / cellWidth;
            int Y = (int)point.Y / cellWidth;

            if ((X > cols - 1) || (Y > rows - 1)) return; // завершаем работу если нажали за пределы сетки

            if (map[Y, X] == 1)
            {
                map[Y, X] = -1; // сбрасываем клетку
            }
            else
            {
                map[Y, X] = 1; // если нажатие на пустую клетку, то рисуется новая клетка
            }
        }
        public int GetCellIndex(Point point)
        {
            int x = (int)point.X / cellWidth;
            int y = (int)point.Y / cellWidth;

            return (y * cols) + x;
        }
        public int[] ToArray()
        {
            int[] arr = new int[rows * cols];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var index = y * cols + x;
                    arr[index] = map[y, x];
                }
            }
            return arr;
        }
        public void UpdateMapFromX(int[] arr)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var index = y * cols + x;

                    map[y, x] = arr[index];
                }
            }
        }
        public void Draw(DrawingContext dc)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Rect rect = new Rect()
                    {
                        X = x * cellWidth,
                        Y = y * cellWidth,
                        Width = cellWidth,
                        Height = cellWidth,
                    };
                    Brush brush = Brushes.Gray;
                    Pen pen = new Pen(Brushes.Black, 0.3);
                    switch (map[y, x])
                    {
                        case -1: // empty
                            brush = Brushes.White;
                            break;
                        case 1: // busy
                            brush = Brushes.Green;
                            break;
                    }
                    dc.DrawRectangle(brush, pen, rect);
                }
            }
        }
    }
}

