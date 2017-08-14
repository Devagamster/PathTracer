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
        public Vector Color { get; set; }
        public double Reflectance { get; set; }
        public double Roughness { get; set; }
        public double Absorbance { get; set; }

        public SampleResult()
        {
            Color = Vector.One;
            Reflectance = 0.5;
            Roughness = 1;
            Absorbance = 0.05;
        }

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
