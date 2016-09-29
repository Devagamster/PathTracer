using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RayTracer
{
    public static class Utils
    {
        public static Color Interpolate(Color a, Color b, double amount)
        {
            return Color.Add(Color.Multiply(a, 1 - (float)amount), Color.Multiply(b, (float)amount));
        }

        public static int Interpolate(int a, int b, double amount)
        {
            return (int)(a * (1 - amount) + b * amount);
        }

        public static double Interpolate(double a, double b, double amount)
        {
            return a * (1 - amount) + b * amount;
        }
    }
}
