using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.DistanceFields
{
    public class Reflection : DistanceField
    {
        public DistanceField Field { get; set; }
        public double Reflectance { get; set; }
        public Reflection(DistanceField field, double reflectance)
        {
            Field = field;
            Reflectance = reflectance;
        }

        public override SampleResult Sample(Vector pos)
        {
            var sample = Field.Sample(pos);
            sample.Reflectance = Reflectance;
            return sample;
        }
    }
}
