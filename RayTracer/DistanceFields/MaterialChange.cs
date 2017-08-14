using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.DistanceFields
{
    public class MaterialChange : DistanceField
    {
        public DistanceField Field { get; set; }
        public MaterialSettings MaterialSettings { get; set; }

        public MaterialChange(DistanceField field, MaterialSettings materialSettings)
        {
            Field = field;
            MaterialSettings = materialSettings;
        }

        public override SampleResult Sample(Vector pos)
        {
            var result = Field.Sample(pos);
            result.Roughness = MaterialSettings.Roughness;
            result.Color = MaterialSettings.GetColor(pos);
            result.Reflectance = MaterialSettings.Reflectance;
            result.Absorbance = MaterialSettings.Absorbance;
            return result;
        }
    }
}
