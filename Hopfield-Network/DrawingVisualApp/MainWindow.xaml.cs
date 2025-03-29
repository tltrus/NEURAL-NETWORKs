using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace DrawingVisualApp
{
    public partial class MainWindow : Window
    {
        DrawingVisual visual;
        DrawingContext dc;

        int width, height;

        Map map;
        HopfieldNet HopfieldNet;
        int mapSize = 10;
        int showcounter;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();

            width = (int)g.Width;
            height = (int)g.Height;

            map = new Map(mapSize, mapSize);
            HopfieldNet = new HopfieldNet(mapSize);
            map.Reset();
            Drawing();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            HopfieldNet.AddImage(map);
            lbl1.Content = HopfieldNet.GetInputsCount().ToString();
        }

        private void btnTeach_Click(object sender, RoutedEventArgs e)
        {
            var result = HopfieldNet.Teach();
            lb2.Content = result;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            int result = HopfieldNet.Find(map);
            if (result >= 0)
            {
                lb2.Content = "Image " + (result + 1) + " [" + result + "] was found!";
                map.Reset();
                int[] input = HopfieldNet.GetImage(result);
                map.UpdateMapFromX(input);
                Drawing();
            }
            else
            {
                lb2.Content = "Image was NOT found!";
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            map.Reset();
            Drawing();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            map.Reset();
            int[] x = HopfieldNet.GetImage(showcounter);
            if (x != null)
            {
                map.UpdateMapFromX(x);
                Drawing();

                lbl1.Content = (showcounter + 1).ToString();

                if (showcounter == (HopfieldNet.GetInputsCount() - 1))
                    showcounter = 0;
                else
                    showcounter++;
            }
        }

        private void CellToggle(MouseEventArgs e)
        {
            Point p = e.GetPosition(g);
            map.Toggle(p);
            Drawing();
        }

        private void g_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(g);

                var index = map.GetCellIndex(p);
                if (index != Map.memoryCellindex)
                {
                    CellToggle(e);
                    Map.memoryCellindex = index; // запоминаем индекс клетки
                }
            }
        }

        private void g_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(g);
            Map.memoryCellindex = map.GetCellIndex(p);
            CellToggle(e);
        }

        private void Drawing()
        {
            g.RemoveVisual(visual);

            using (dc = visual.RenderOpen())
            {
                map.Draw(dc);

                dc.Close();
                g.AddVisual(visual);
            }
        }
    }
}
