using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.DistanceFields
{
    public class Subtraction : DistanceField
    {
        public DistanceField First { get; set; }
        public DistanceField Second { get; set; }
        public Subtraction(DistanceField first, DistanceField second)
        {
            First = first;
            Second = second;
        }

        public override SampleResult Sample(Vector pos)
        {
            var secondResult = Second.Sample(pos);
            secondResult.Distance = -secondResult.Distance;
            secondResult.Normal = -secondResult.Normal;
            return SampleResult.Max(First.Sample(pos), secondResult);
        }
    }
}
