using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace DrawingVisualApp
{
    // Сделано по примеру программы: Р.В. Шамин. Лекция № 4. Классификация сетью Кохонена
    // https://github.com/rwsh/ClassK
    // https://www.youtube.com/watch?v=UUMKKtkzZWg

    public partial class MainWindow : Window
    {
        Random rnd = new Random();
        int width, height;

        DrawingVisual visual;
        DrawingContext dc;
        Point mouse;
        KohonenNet KohonenNet;
        int K;


        List<Point2D> points = new List<Point2D>();

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();

            width = (int)g.Width;
            height = (int)g.Height;

            Init();

            Drawing();
        }

        void Init()
        {
            points.Clear();
            lbText.Content = "";

            for (int i = 0; i < 100; i++)
            {
                var p = new Point(rnd.Next(10, width - 10), rnd.Next(10, height - 10));
                var point = new Point2D(p);
                points.Add(point);
            }

            KohonenNet = new KohonenNet();
            K = rnd.Next(2,6); // random class
            KohonenNet.Init(points, K);
        }

        void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                foreach (var p in points)
                {
                    p.Draw(dc);
                }
                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void g_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouse = e.GetPosition(g);
            points.Add(new Point2D(mouse));
            Drawing();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Init();
            Drawing();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count <= 0) return;

            KohonenNet.Learning();
            KohonenNet.Classify(points);

            lbText.Content = "Random K (classes) = " + K;

            Drawing();
        }

        private void g_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
