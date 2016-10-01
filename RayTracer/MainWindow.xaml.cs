using RayTracer.DistanceFields;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Concurrent;

namespace RayTracer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WriteableBitmap bitmap;
        DisplayMethod displayMethod;
        Thread backgroundThread;

        public static Vector Up;
        public static Vector Forward;
        public static Vector Right;

        public static Vector WorldUp = new Vector(0, 1, 0);
        public static Vector Target = new Vector(0, 0, 0);
        public static Vector Eye = new Vector(0, 0.5, -2);
        public static double FocalLength = 1;
        public static int OverScan = 2;

        public static Vector Fog = Vector.One;

        public static MaterialSettings CheckerBoard = new MaterialSettings
        {
            GetColor = (Vector pos) =>
            {
                var sum = (int)Math.Floor(pos.X) + (int)Math.Floor(pos.Z);
                var white = (sum / 2) * 2 == sum;
                if (white)
                {
                    return Vector.One;
                }
                else
                {
                    return Vector.Zero;
                }
            }
        };

        public static DistanceField Field =
            (new Sphere(0.5) * new MaterialSettings { GetColor = _ => new Vector(0, 1, 0), Reflectance = 0.8, Roughness = 0.5 }) + 
            (new Sphere(0.5) * new MaterialSettings { GetColor = _ => new Vector(1, 0, 0), Reflectance = 0.8, Roughness = 0 } + new Vector(-1, 0, 0)) +
            (new Sphere(0.5) * new MaterialSettings { GetColor = _ => new Vector(0, 0, 1), Reflectance = 0.8, Roughness = 1 } + new Vector(1, 0, 0)) +
            (new Plane(WorldUp, -0.5) * CheckerBoard);

        public MainWindow()
        {
            Forward = (Target - Eye).Normalize();
            Right = Forward.Cross(WorldUp).Normalize();
            Up = Right.Cross(Forward).Normalize();

            InitializeComponent();
            UpdateSize();
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            this.Closed += Stop;

            backgroundThread = new Thread(CalculateRays);
            backgroundThread.Start();
        }

        private void Stop(object sender, EventArgs e)
        {
            backgroundThread.Abort();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (bitmap.PixelWidth != (int)Content.ActualWidth * OverScan || bitmap.PixelHeight != (int)Content.ActualHeight * OverScan)
            {
                UpdateSize();
            }

            var start = DateTime.Now;
            while ((DateTime.Now - start).Milliseconds < 16)
            {
                displayMethod.DrawPiece(bitmap);
            }
        }

        private void CalculateRays()
        {
            while (true)
            {
                Parallel.For(0, 1000, (_) =>
                {
                    var x = ThreadSafeRandom.NextDouble() - 0.5;
                    var y = ThreadSafeRandom.NextDouble() - 0.5;

                    var pixel = Eye + x * Right + y * Up;
                    var source = Eye - Forward * FocalLength;
                    var ray = new Ray(source, pixel - source);

                    var result = ray.March(Field, 0.01, 1000, Fog);
                    displayMethod.AddPoint(new ColoredPoint(result.Color, x, -y));
                });
            }
        }

        private void UpdateSize()
        {
            bitmap = BitmapFactory.New((int)Content.ActualWidth * OverScan, (int)Content.ActualHeight * OverScan);
            ImageContainer.Source = bitmap;
            var pixelSize = 1.0 / Math.Max(bitmap.PixelWidth, bitmap.PixelHeight);
            if (displayMethod == null)
            {
                displayMethod = new AveragedPixels(1);
            }
            else
            {
                displayMethod.Reset(pixelSize);
            }
        }
    }
}
