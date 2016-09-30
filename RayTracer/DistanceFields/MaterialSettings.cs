using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.DistanceFields
{
    public class MaterialSettings
    {
        public Func<Vector, Vector> GetColor { get; set; }
        public double Reflectance { get; set; }
        public double Roughness { get; set; }
        public bool Source { get; set; }
        public double Absorbance { get; set; }

        public MaterialSettings()
        {
            GetColor = _ => Vector.One;
            Reflectance = 0;
            Roughness = 1;
            Absorbance = 0.1;
            Source = false;
        }
    }
}
