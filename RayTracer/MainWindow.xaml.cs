using RayTracer.DistanceFields;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RayTracer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WriteableBitmap bitmap;
        DistanceField field;
        DisplayMethod displayMethod;

        public static Vector Up = new Vector(0, 1, 0);
        public static Vector Right = new Vector(1, 0, 0);
        public static Vector Eye = Vector.Zero;
        public static double FocalLength = 1;
        public static Random Random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            UpdateSize();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            field = (new Sphere(0.5) + new Vector(0, 0, 2) + Color.FromRgb(255, 0, 0)) / 0.5 + (new Plane(Up, -0.5) + ((Vector pos) => {
                var sum = (int)pos.X + (int)pos.Z;
                var white = (sum / 2) * 2 == sum;
                if (white)
                {
                    return Color.FromRgb(255, 255, 255);
                }
                else
                {
                    return Color.FromRgb(0, 0, 0);
                }
            }));
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if ((int)bitmap.Width != (int)Content.ActualWidth * 2 || (int)bitmap.Height != (int)Content.ActualHeight * 2)
            {
                UpdateSize();
            }

            bitmap.Lock();
            for (var i = 0; i < 1000; i++)
            {
                DrawPoint(bitmap, field, Random.NextDouble() - 0.5, Random.NextDouble() - 0.5);
            }
            bitmap.Unlock();
        }

        private void DrawPoint(WriteableBitmap bitmap, DistanceField field, double x, double y)
        {
            var pixel = Eye + x * Right + y * Up;
            var source = Eye + Up.Cross(Right).Normalize() * FocalLength;
            var ray = new Ray(source, pixel - source);

            var result = ray.March(field, 128, 5, 0.01, 50);
            
            displayMethod.DrawPoint(new ColoredPoint(result.Color, x, -y), bitmap);
        }

        private void UpdateSize()
        {
            bitmap = BitmapFactory.New((int)Content.ActualWidth * 2, (int)Content.ActualHeight * 2);
            ImageContainer.Source = bitmap;
            var pixelSize = 1.0 / Math.Max(bitmap.PixelWidth, bitmap.PixelHeight);
            if (displayMethod == null)
            {
                displayMethod = new QuadTree(pixelSize, 1, 0, 0, new List<ColoredPoint>());
            }
            else
            {
                displayMethod.Reset(pixelSize);
            }
        }
    }
}
