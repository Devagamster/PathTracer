using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.DistanceFields
{
    public class Translation : DistanceField
    {
        public DistanceField Field { get; set; }
        public Vector Amount { get; set; }

        public Translation(DistanceField field, Vector amount)
        {
            Field = field;
            Amount = amount;
        }

        public override SampleResult Sample(Vector pos)
        {
            return Field.Sample(pos - Amount);
        }
    }
}
