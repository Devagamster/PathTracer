using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.DistanceFields
{
    public class Sphere : DistanceField
    {
        public double Radius { get; set; }

        public Sphere(double radius)
        {
            Radius = radius;
        }

        public override SampleResult Sample(Vector pos)
        {
            return new SampleResult(pos.Length() - Radius, pos.Normalize());
        }
    }
}
