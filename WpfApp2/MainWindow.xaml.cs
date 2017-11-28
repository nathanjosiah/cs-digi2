using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClipperLib;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WriteableBitmap writeableBmp;
        bool isDrawing = true;
        Point lastPoint;
        List<Point> runningPoints = new List<Point>();
        int plotPointCount = 0;
        List<Point> poly = new List<Point> {

                    /*
                    new Point(100,100),
                    new Point(200,100),
                    new Point(200,120),
                    new Point(120,280),
                    new Point(250,160),
                    new Point(250,100),
                    new Point(300,100),
                    new Point(300,300),
                    new Point(100,300),
                    new Point(100,100),

                    new Point(270,270),
                    new Point(280,270),
                    new Point(280,280),
                    new Point(270,280),
                    new Point(270,270),*/

                };

        private Point rotatePoint(Point p, int deg, Point origin)
        {
            var x = p.X - origin.X;
            var y = p.Y - origin.Y;
            var c = Math.Cos(deg * Math.PI / 180);
            var s = Math.Sin(deg * Math.PI / 180);
            var rx = x * c - y * s;
            var ry = x * s + y * c;

            return new Point(rx + origin.X, ry + origin.Y);
        }
        int deg = 1;
        public MainWindow()
        {
            InitializeComponent();

            writeableBmp = BitmapFactory.New(512, 512);
            image.Source = writeableBmp;
            int i = 0;
            DrawPolygon(poly);
            //d();

            //other2(deg);
        }

        private bool IsPointInPolygon(List<Point> polygon, Point point)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Count() - 1; i < polygon.Count(); j = i++)
            {
                if (((polygon.ElementAt(i).Y > point.Y) != (polygon.ElementAt(j).Y > point.Y)) &&
                (point.X < (polygon.ElementAt(j).X - polygon.ElementAt(i).X) * (point.Y - polygon.ElementAt(i).Y) / (polygon.ElementAt(j).Y - polygon.ElementAt(i).Y) + polygon.ElementAt(i).X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        /*
        private void d()
        {
            if (writeableBmp == null)
            {
                return;
            }
            var path = new Path()
            {
                new IntPoint(100,100),
                new IntPoint(200,100),
                new IntPoint(200,200),
                new IntPoint(100,200),
                new IntPoint(100,100),
            };


            using (writeableBmp.GetBitmapContext())
            {
                writeableBmp.Clear(Colors.Transparent);
                IntPoint? lp = null;
                foreach (IntPoint pp in path)
                {
                    var rp = rotatePoint(pp, deg, new Point(256, 256));
                    writeableBmp.DrawEllipse((int)rp.X - 2, (int)rp.Y - 2, (int)rp.X + 2, (int)rp.Y + 2, Colors.Black);
                    if (lp != null)
                    {
                        writeableBmp.DrawLine((int)((IntPoint)lp).X, (int)((IntPoint)lp).Y, (int)rp.X, (int)rp.Y, Colors.Black);
                    }
                    lp = (IntPoint)rp;
                }
            }
        }*/

        private void other2(double deg)
        {

            int space = 10;
            var ipRows = new List<List<Line>>();
            var maxIp = 0;
            grid.Children.Clear(); ;
            plotPointCount = 0;
            for (int y = 0; y <= 512 - space; y += space)
            {
                var p1 = rotatePoint(new Point(0, y), (int)deg, new Point(256, 256));
                var p2 = rotatePoint(new Point(512, y), (int)deg, new Point(256, 256));
                //DrawPolygon(new List<Point> { p1, p2 });

                //var p1 = new Point(0,y);
                //var p2 = new Point(512,y);

                var l = new Line {
                    X1 = p1.X,
                    Y1 = p1.Y,
                    X2 = p2.X,
                    Y2 = p2.Y
                };
                var ip = IntersectPolygon(poly, l);

                PlotPoints(ip, Colors.Red);
                //continue;
                maxIp = Math.Max(ip.Count(), maxIp);
                if(ip.Count() == 0)
                {
                    continue;
                }
                ipRows.Add(LinesInsidePolygon(poly, ip));

                /*
                var lines = LinesInsidePolygon(poly, ip);
                foreach (Line li in lines)
                {
                    PlotPoints(new List<Point> { new Point { X = li.X1, Y = li.Y1 } }, Colors.Red);
                    PlotPoints(new List<Point> { new Point { X = li.X2, Y = li.Y2 } }, Colors.Blue);
                }
                foreach(Point ppp in ip)
                {
                    PlotPoints(new List<Point> { ppp }, Colors.Red);
                }*/
            }

           var stitches = new List<Point>();
            for(int i = 0; i < maxIp-1; i++)
            {
                foreach(List<Line> ip in ipRows)
                {

                    if(i > ip.Count() - 1)
                    {
                        continue;
                    }

                    var p1 = new Point { X = ip[i].X1, Y = ip[i].Y1 };
                    var p2 = new Point { X = ip[i].X2, Y = ip[i].Y2 };

                    PlotPoints(new List<Point> { p1 }, Colors.Red);
                    PlotPoints(new List<Point> { p2 }, Colors.Blue);

                    stitches.Add(p1);
                    stitches.Add(p2);
                }
            }

            //PlotPoints(stitches, Colors.Red);
            DrawPolygon(poly);
            DrawPolygon(stitches);
        }
        /*
        private void other()
        {
            Paths subj = new Paths()
            {
                new Path()
                {
                    new IntPoint(100,100),
                    new IntPoint(200,100),
                    new IntPoint(200,120),
                    new IntPoint(120,280),
                    new IntPoint(250,160),
                    new IntPoint(250,100),
                    new IntPoint(300,100),
                    new IntPoint(300,300),
                    new IntPoint(100,300),
                    new IntPoint(100,100),
                }
            };

            Paths clip = new Paths();
            var p = new Path();
            int space = 15;
            for (int y = 0; y <= 512 - space; y += (space * 2))
            {
                p.Add(new IntPoint(0, y));
                p.Add(new IntPoint(512, y));
                p.Add(new IntPoint(512, y + space));
                p.Add(new IntPoint(0, y + space));
            }

            p.Add(new IntPoint(0, 512));
            p.Add(new IntPoint(0, 0));
            clip.Add(p);

            //DrawPolygons(subj, Color.FromArgb(0x20, 0, 0, 0xFF), Color.FromArgb(0x20, 0, 0, 0xFF));
            //DrawPolygons(clip, Color.FromArgb(0xAA, 0, 0xFF, 0), Color.FromArgb(0xAA, 0, 0xFF, 0));

            Paths solution = new Paths();

            Clipper c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctIntersection, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            DrawPolygons(solution, Color.FromArgb(0xFF, 0xFF, 0, 0), Color.FromArgb(0xFF, 0xFF, 0, 0), true, false);
        }

        private void DrawPolygons(Paths paths, Color fillColor, Color strokeColor, bool label = false, bool connect = false)
        {
            IntPoint? lastPoint = null;
            using (writeableBmp.GetBitmapContext())
            {
                int i = 0;
                foreach (Path path in paths)
                {
                    var points = PathToPoints(path);
                    writeableBmp.DrawPolyline(points, strokeColor);
                    //writeableBmp.FillPolygon(points,fillColor);

                    foreach (IntPoint p in path)
                    {


                        if (label)
                        {

                            var l = new TextBlock();
                            l.Text = "" + i++;// + ":" + p.X + "," + p.Y;
                            l.Margin = new Thickness(p.X, p.Y, 0, 0);
                            l.HorizontalAlignment = HorizontalAlignment.Left;
                            l.VerticalAlignment = VerticalAlignment.Top;
                            grid.Children.Add(l);
                        }
                        //writeableBmp.DrawEllipse((int)p.X - 2, (int)p.Y - 2, (int)p.X + 2, (int)p.Y + 2, Colors.Black);

                        if (connect && lastPoint != null)
                        {
                            writeableBmp.DrawLine((int)((IntPoint)lastPoint).X, (int)((IntPoint)lastPoint).Y, (int)p.X, (int)p.Y, Colors.Black);
                        }
                        lastPoint = (IntPoint)p;
                    }

                }
            }
        }

        private int[] PathToPoints(Path p)
        {
            var points = new int[p.Count() * 2];
            int i = 0;
            foreach (IntPoint point in p)
            {
                points[i++] = (int)point.X;
                points[i++] = (int)point.Y;
            }
            return points;
        }
    */

        private Point Interpolate(Point p1, Point p2, double t)
        {
            return new Point
            {
                X = p1.X + (p2.X - p1.X) * t,
                Y = p1.Y + (p2.Y - p1.Y) * t,
            };
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            lastPoint = e.GetPosition(image);
            //lastPoint = new Point(256, 256);
            poly.Add(lastPoint);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing)
            {
                return;

            }
            var point = e.GetPosition(image);
            var l = (Math.Sqrt(Math.Pow((int)(point.X - lastPoint.X),2) + Math.Pow((int)(point.Y - lastPoint.Y), 2)));
            //var deg = (180 / Math.PI) * Math.Asin((point.X - lastPoint.X) / l);
            var deg = (180 / Math.PI) * Math.Atan2((point.Y - lastPoint.Y), (point.X - lastPoint.X));

            ClearScreen();
            other2(deg);


            label.Content = "" + deg + "deg";
            //lastPoint = point;

        }

        public void PlotPoints(List<Point> points, Color? color = null)
        {
            if (color == null)
            {
                color = Colors.Black;
            }
            using (writeableBmp.GetBitmapContext())
            {
                foreach (Point p in points)
                {
                    writeableBmp.DrawEllipse((int)p.X - 2, (int)p.Y - 2, (int)p.X + 2, (int)p.Y + 2, (Color)color);
                    //var l = new Label();
                    //l.Content = plotPointCount++;
                    //l.Margin = new Thickness(Math.Round(p.X),Math.Round(p.Y) - 15,0,0);
                    //grid.Children.Add(l);
                }
            }
        }

        public void ClearScreen()
        {
            using (writeableBmp.GetBitmapContext())
            {
                writeableBmp.Clear(Colors.Transparent);
            }
        }

        public void DrawPolygon(List<Point> points)
        {
            Point? lp = null;
            using (writeableBmp.GetBitmapContext())
            {
                foreach (Point p in points)
                {
                    if (lp != null)
                    {
                        writeableBmp.DrawLine((int)((Point)lp).X, (int)((Point)lp).Y, (int)p.X, (int)p.Y, Colors.Black);
                    }
                    lp = p;
                }
            }
        }

        public List<Point> IntersectPolygon(List<Point> lines, Line testLine)
        {
            var l = new List<Point>();
            Point? lp = null;
            foreach (Point p in lines)
            {
                if (lp != null)
                {
                    var i = Intersect(new Line { X1 = ((Point)lp).X, Y1 = ((Point)lp).Y, X2 = p.X, Y2 = p.Y }, testLine);
                    if (i != null)
                    {
                        l.Add((Point)i);
                    }
                }
                lp = p;
            }
            return (List<Point>)l.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
        }

        public List<Line> LinesInsidePolygon(List<Point> polygon, List<Point>points) {
            var l = new List<Line>();
            int i = 0;

            while (i < points.Count())
            {
                var p1 = points.ElementAt(i);
                Point p2;
                try
                {
                    p2 = points.ElementAt(i + 1);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    break;
                }
                var p = Interpolate(p1, p2, 0.5);
                //PlotPoints(new List<Point> { p},Colors.Green);
                if (IsPointInPolygon(polygon,p))
                {
                    l.Add(new Line {
                        X1 = p1.X,
                        Y1 = p1.Y,
                        X2 = p2.X,
                        Y2 = p2.Y
                    });
                    i += 2;
                }
                else
                {
                    i++;
                }
            }
            return l;
        }

        // http://silverbling.blogspot.com/2010/06/2d-line-segment-intersection-detection.html
        // Returns true if the lines intersect, otherwise false. If the lines 
        // intersect, intersectionPoint holds the intersection point.
        public Point? Intersect(Line otherLineSegment, Line testLine)
        {
            double firstLineSlopeX, firstLineSlopeY, secondLineSlopeX, secondLineSlopeY;

            firstLineSlopeX = testLine.X2 - testLine.X1;
            firstLineSlopeY = testLine.Y2 - testLine.Y1;

            secondLineSlopeX = otherLineSegment.X2 - otherLineSegment.X1;
            secondLineSlopeY = otherLineSegment.Y2 - otherLineSegment.Y1;

            double s, t;
            s = (-firstLineSlopeY * (testLine.X1 - otherLineSegment.X1) + firstLineSlopeX * (testLine.Y1 - otherLineSegment.Y1)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);
            t = (secondLineSlopeX * (testLine.Y1 - otherLineSegment.Y1) - secondLineSlopeY * (testLine.X1 - otherLineSegment.X1)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                double intersectionPointX = testLine.X1 + (t * firstLineSlopeX);
                double intersectionPointY = testLine.Y1 + (t * firstLineSlopeY);

                // Collision detected
                return new Point(intersectionPointX, intersectionPointY);
            }

            return null;
        }

        private static bool IsInsideLine(Line line, double x, double y)
        {
            return ((x >= line.X1 && x <= line.X2)
                        || (x >= line.X2 && x <= line.X1))
                   && ((y >= line.Y1 && y <= line.Y2)
                        || (y >= line.Y2 && y <= line.Y1));
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            runningPoints.Add(e.GetPosition(image));
            //isDrawing = false;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = (Slider)sender;
            deg = (int)s.Value;
            //d();
        }
    }
}
