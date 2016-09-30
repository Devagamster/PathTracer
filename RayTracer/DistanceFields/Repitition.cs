using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.DistanceFields
{
    public class Repitition : DistanceField
    {
        public DistanceField Field { get; set; }
        public Vector Distance { get; set; }

        public Repitition(DistanceField field, Vector distance)
        {
            Field = field;
            Distance = distance;
        }

        public override SampleResult Sample(Vector pos)
        {
            var x = pos.X;
            var y = pos.Y;
            var z = pos.Z;
            if (Distance.X != 0)
            {
                x = x - Distance.X * Math.Floor(x / Distance.X);
            }
            if (Distance.Y != 0)
            {
                y = y - Distance.Y * Math.Floor(y / Distance.Y);
            }
            if (Distance.Z != 0)
            {
                z = z - Distance.Z * Math.Floor(z / Distance.Z);
            }

            var newVector = new Vector(
                x,
                y,
                z) - 0.5 * Distance;
            return Field.Sample(newVector);
        }
    }
}
