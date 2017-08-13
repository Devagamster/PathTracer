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
        public static double FocalLength = 1;
        public static int OverScan = 1;


        public MainWindow()
        {
            Forward = (Scene.Target - Scene.Eye).Normalize();
            Right = Scene.WorldUp.Cross(Forward).Normalize();
            Up = Forward.Cross(Right).Normalize();

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
            var atmosRendering = new AtmosphereRendering(new Vector(0, 1, 1), 1);
            while (true)
            {
                Parallel.For(0, 250, (_) =>
                {
                    var x = ThreadSafeRandom.NextDouble() - 0.5;
                    var y = ThreadSafeRandom.NextDouble() - 0.5;

                    var pixel = Scene.Eye + x * Right + y * Up;
                    var source = Scene.Eye - Forward * FocalLength;
                    var ray = new Ray(source, pixel - source);

                    var result = ray.March(Scene.Field, 0.01, 50, atmosRendering.CalculateSkyColor);
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
