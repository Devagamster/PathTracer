using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RayTracer.DistanceFields
{
    public class SampleResult
    {
        public double Distance { get; set; }
        public Vector Normal { get; set; }
        public Color Color { get; set; }
        public double Reflectance { get; set; }
        public double Absorbance { get; set; }

        public SampleResult(double distance, Vector normal, Color color, double reflectance, double absorbance)
        {
            Distance = distance;
            Normal = normal;
            Color = color;
            Reflectance = reflectance;
            Absorbance = absorbance;
        }

        public SampleResult(double distance, Vector normal, Color color, double reflectance)
            : this(distance, normal, color, reflectance, 0) { }

        public SampleResult(double distance, Vector normal, Color color)
            : this(distance, normal, color, 0) { }

        public SampleResult(double distance, Vector normal)
            : this(distance, normal, Color.FromRgb(255, 255, 255)) { }

        public static SampleResult Min(SampleResult first, SampleResult second)
        {
            if (first.Distance < second.Distance)
            {
                return first;
            }
            else
            {
                return second;
            }
        }

        public static SampleResult Max(SampleResult first, SampleResult second)
        {
            if (first.Distance > second.Distance)
            {
                return first;
            }
            else
            {
                return second;
            }
        }
    }
}
