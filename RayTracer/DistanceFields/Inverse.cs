using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.DistanceFields
{
    public class Inverse : DistanceField
    {
        public DistanceField Field { get; private set; }

        public Inverse(DistanceField field)
        {
            Field = field;
        }

        public override SampleResult Sample(Vector pos)
        {
            var sample = Field.Sample(pos);
            sample.Distance *= -1;
            return sample;
        }
    }
}
