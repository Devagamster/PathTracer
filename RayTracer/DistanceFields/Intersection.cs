using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.DistanceFields
{
    public class Intersection : DistanceField
    {
        public DistanceField First { get; set; }
        public DistanceField Second { get; set; }
        public Intersection(DistanceField first, DistanceField second)
        {
            First = first;
            Second = second;
        }

        public override SampleResult Sample(Vector pos)
        {
            return SampleResult.Max(First.Sample(pos), Second.Sample(pos));
        }
    }
}
