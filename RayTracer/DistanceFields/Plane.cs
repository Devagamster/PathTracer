using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RayTracer.DistanceFields
{
    public class Plane : DistanceField
    {
        public Vector Normal { get; set; }
        public double Offset { get; set; }

        public Plane(Vector normal, double offset)
        {
            Normal = normal.Normalize();
            Offset = offset;
        }

        public override SampleResult Sample(Vector pos)
        {
            return new SampleResult {Distance = pos.Dot(Normal) - Offset, Normal = Normal };
        }
    }
}
