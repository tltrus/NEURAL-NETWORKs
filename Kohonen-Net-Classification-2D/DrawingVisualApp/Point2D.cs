using System.Windows.Media;
using System.Windows;

namespace DrawingVisualApp
{
    public class Point2D
    {
        public Point pos;
        public Brush brush;
        public int k; // class

        public Point2D(Point pos)
        {
            this.pos = pos;
        }
        public void SetColor(Brush brush) => this.brush = brush;
        public void SetClass(int k) => this.k = k;
        public void Draw(DrawingContext dc)
        {
            switch(k)
            {
                case 0:
                    brush = Brushes.LimeGreen;
                    break;
                case 1:
                    brush = Brushes.DeepPink;
                    break;
                case 2:
                    brush = Brushes.Blue;
                    break;
                case 3:
                    brush = Brushes.White;
                    break;
                case 4:
                    brush = Brushes.Green;
                    break;
                default:
                    brush = Brushes.Black;
                    break;
            }

            dc.DrawEllipse(brush, null, pos, 5, 5);
        }
    }
}
