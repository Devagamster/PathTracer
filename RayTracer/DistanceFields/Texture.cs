using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RayTracer.DistanceFields
{
    public class Texture : DistanceField
    {
        public DistanceField Field { get; set; }
        public Func<Vector, Color> GetColor { get; set; }
        public Texture(DistanceField field, Func<Vector, Color> getColor)
        {
            Field = field;
            GetColor = getColor;
        }

        public Texture(DistanceField field, Color color)
            : this(field, (_) => color) { }

        public override SampleResult Sample(Vector pos)
        {
            var sample = Field.Sample(pos);
            sample.Color = GetColor(pos);
            return sample;
        }
    }
}
