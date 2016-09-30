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
        public static Vector Interpolate(Vector a, Vector b, double amount)
        {
            return a * (1 - amount) + b * amount;
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
