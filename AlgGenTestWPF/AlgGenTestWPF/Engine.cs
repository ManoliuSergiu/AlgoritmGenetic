using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AlgGenTestWPF
{
    static class Engine
    {
        public static double scale = 1;
        public static double widthA, heightA;
        public static double basemeter = 200;
        public static float g = 9.8f;
        public static int nodeSize = 20;
        public static int tickrate = 60;
        public static int left = 0;
        public static int seconds = 15;
        public static int tick = 15;
        public static long interval = (long)1e7 / tickrate;
        public static bool testing = true;
        public static Shape shape;
        public static Node selected;
        public static Random rnd = new Random();
        public static List<Entity> population = new List<Entity>();

        public static Image InitializeBackground(double width, double height)
        {
            widthA = width;
            heightA = (height * 3) / 4;
            DrawingVisual drawingVisual = new DrawingVisual();
            Pen penny = new Pen(Brushes.Black, 5);
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.SkyBlue, null, new Rect(0, 0, width, (height * 3) / 4));
                dc.DrawRectangle(Brushes.Green, null, new Rect(0, (height * 3) / 4, width, height / 4));

            }
            Image toReturn = new Image
            {
                Source = new DrawingImage(drawingVisual.Drawing)
            };
            return toReturn;
        }

        #region GraphicsRegion
        public static Image Frame()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                double size = shape.Size();
                if (size > 200)
                {
                    scale = basemeter / size;
                }
                Pen pen = new Pen(Brushes.Black, 1 * scale);
                BackgroundDrawing(dc);
                MuscleDrawing(dc, pen);
                NodeDrawing(dc, pen);

                FormattedText tf = new FormattedText(Math.Round(shape.center / 200, 2) + " m",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight, new Typeface("impact"),
                    35, Brushes.Black,
                    96);
                FormattedText af = new FormattedText(Math.Round(((double)tick / tickrate), 2) + " s",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight, new Typeface("impact"),
                    15, Brushes.Black,
                    96);

                dc.DrawGeometry(Brushes.White, pen, tf.BuildGeometry(new Point(widthA / 2 - tf.Width / 2, 20)));
                dc.DrawGeometry(Brushes.White, pen, af.BuildGeometry(new Point(widthA - tf.Width, 15)));
            }
            Image toReturn = new Image
            {
                Source = new DrawingImage(drawingVisual.Drawing)
            };

            return toReturn;
        }

        private static void NodeDrawing(DrawingContext dc, Pen pen)
        {
            foreach (var node in shape.nodes)
            {

                SolidColorBrush scb1 = new SolidColorBrush(Color.FromRgb((byte)(255 * (1 - node.frictionPercentage)), (byte)(255 * (1 - node.frictionPercentage)), (byte)(255 * (1 - node.frictionPercentage))));
                dc.DrawRoundedRectangle(scb1, pen, new Rect(new Point(node.GetNormalPoint().X, node.GetNormalPoint().Y), new Size(nodeSize * scale, nodeSize * scale)), 3 * scale, 3 * scale);

            }
        }

        private static void MuscleDrawing(DrawingContext dc, Pen pen)
        {
            foreach (var muscle in shape.muscles)
            {
                Color color;
                if (muscle.currentTick < muscle.timeToContract)
                {
                    color = Colors.Red;
                    color.A = (byte)(255 * muscle.contractionSizePrecentage * (1 - ((double)muscle.timeToContract / muscle.cylceDuration)));
                }
                else if (muscle.currentTick < muscle.timeToExtend + muscle.timeToContract)
                {
                    color = Colors.Blue;
                    color.A = (byte)(255 - 255 * ((double)muscle.timeToExtend / muscle.cylceDuration));
                }
                else
                {
                    color = Colors.Gray;
                    color.A = (byte)(255 - 255 * ((double)muscle.timeIdle / muscle.cylceDuration));
                }
                SolidColorBrush scb = new SolidColorBrush(color);
                StreamGeometry streamGeometry = new StreamGeometry();
                using (StreamGeometryContext geometryContext = streamGeometry.Open())
                {
                    double angle = Angle(muscle);
                    PointCollection muscly = new PointCollection();
                    geometryContext.BeginFigure(new Point(muscle.nodes[0].GetNormalPoint().X + (nodeSize / 2) * scale - 5 * Math.Sin(angle) * scale, muscle.nodes[0].GetNormalPoint().Y + (nodeSize / 2) * scale - 5 * Math.Cos(angle) * scale), true, true);
                    muscly.Add(new Point(muscle.nodes[0].GetNormalPoint().X + (nodeSize / 2) * scale + 5 * Math.Sin(angle) * scale, muscle.nodes[0].GetNormalPoint().Y + (nodeSize / 2) * scale + 5 * Math.Cos(angle) * scale));
                    muscly.Add(new Point(muscle.nodes[1].GetNormalPoint().X + (nodeSize / 2) * scale + 5 * Math.Sin(angle) * scale, muscle.nodes[1].GetNormalPoint().Y + (nodeSize / 2) * scale + 5 * Math.Cos(angle) * scale));
                    muscly.Add(new Point(muscle.nodes[1].GetNormalPoint().X + (nodeSize / 2) * scale - 5 * Math.Sin(angle) * scale, muscle.nodes[1].GetNormalPoint().Y + (nodeSize / 2) * scale - 5 * Math.Cos(angle) * scale));
                    geometryContext.PolyLineTo(muscly, true, true);
                }
                dc.DrawGeometry(scb, pen, streamGeometry);
            }
        }

        private static void BackgroundDrawing(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.SkyBlue, null, new Rect(0, 0, widthA, heightA));
            dc.DrawRectangle(Brushes.Green, null, new Rect(0, heightA, widthA, heightA / 3));
            Pen penny2 = new Pen(Brushes.Black, 0.5 * scale);
            double origin = widthA / 2 - shape.center * scale;
            left = (int)(shape.center / basemeter / scale);

            int step = (int)(1 / scale);
            int linecount = (int)((widthA / (basemeter*scale )))*step;
            int i = -linecount;
            double a;
            do
            {
                int step2 = (left + i) - (left + i) % step;
                Pen penny = new Pen(Brushes.Black, 5 * scale * step);
                a = origin + ((left + i) - (left + i) % step) * basemeter * scale;
                if (a > penny.Thickness)
                {
                    dc.DrawLine(penny, new Point(a, heightA), new Point(a, (heightA * 4) / 3));
                    FormattedText ft = new FormattedText(step2 + " m", System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("impact"), 30 * scale * step, Brushes.Goldenrod, 96);
                    if (a >= ft.Height)
                    {
                        double x = a + 10;
                        double y = heightA + 10;
                        RotateTransform RT = new RotateTransform(90, x, y);
                        dc.PushTransform(RT);
                        Geometry geo = ft.BuildGeometry(new Point(x + ft.Width / 2, y + 10));
                        dc.DrawGeometry(Brushes.White, penny2, geo);
                        dc.Pop();
                    }
                }
                i += step;

            } while (a < widthA - basemeter * scale);
        }
        #endregion
        #region Physics
        public static void Physics(Shape shape)
        {
            foreach (var muscle in shape.muscles)
            {
                for (int i = 0; i < 2; i++)
                {


                    muscle.nodes[i].gCount++;
                    muscle.nodes[i].SetSpeed(muscle.nodes[i].GetSpeed() + muscle.force*(Math.Pow(-1,i+2)));
                    if ((int)muscle.nodes[i].GetPoint().Y <= 0)
                    {
                        muscle.nodes[i].gCount /= 2;
                    }
                    
                }
                int time;
                if (muscle.state == -1)
                    time = muscle.timeToContract;
                else if (muscle.state == 1)
                    time = muscle.timeToExtend;
                else
                    time = 1;
                muscle.force = muscle.force - muscle.force / time;

                    muscle.Tick();
            }
            foreach (var node in shape.nodes)
               node.MovePoint();
            shape.CalculateCenter();
        }
        #endregion
        #region Calculation helpers
        public static double Angle(Muscle muscle)
        {
            double diffY = muscle.nodes[1].GetPoint().Y - muscle.nodes[0].GetPoint().Y;
            double diffX = muscle.nodes[1].GetPoint().X - muscle.nodes[0].GetPoint().X;


            return Math.Atan(diffY / diffX);
        }
        internal static double GetLength(Node a, Node b)
        {
            double x1 = a.GetPoint().X;
            double x2 = b.GetPoint().X;
            double y1 = a.GetPoint().Y;
            double y2 = b.GetPoint().Y;
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
        internal static double GetLength(Muscle musc)
        {
            Node a = musc.nodes[0];
            Node b = musc.nodes[1];
            double x1 = a.GetPoint().X;
            double x2 = b.GetPoint().X;
            double y1 = a.GetPoint().Y;
            double y2 = b.GetPoint().Y;
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
        internal static double GetLength(Point a, Point b)
        {
            double x1 = a.X;
            double x2 = b.X;
            double y1 = a.Y;
            double y2 = b.Y;
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
        #endregion
        #region NaturalAlgorithm
        internal static double Predict(Shape shape)
        {
            int i;
            for (i = 0; i < seconds * tickrate; i++)
            {
                Physics(shape);
            }
            //Engine.shape = shape;
            return shape.center / 200;
        }
        public static void IterateShapes()
        {
            foreach (var entity in population)
            {
                entity.score = Predict(new Shape(entity.shape));
            }
            population.Sort((score1, score2) => score2.score.CompareTo(score1.score));
        }
        #endregion
    }
}
