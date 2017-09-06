using RayTracer.DistanceFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public static class Scene
    {
        public static Vector WorldUp = new Vector(0, 1, 0);
        public static Vector Target = new Vector(3, 1.5, 0);
        public static Vector Eye = new Vector(-4, 1, -8);

        public static Vector Fog = Vector.One;

        public static MaterialSettings CheckerBoard = new MaterialSettings
        {
            GetColor = (Vector pos) =>
            {
                var sum = (int)Math.Floor(pos.X) + (int)Math.Floor(pos.Z);
                var white = (sum / 2) * 2 == sum;
                if (white)
                {
                    return Vector.One;
                }
                else
                {
                    return Vector.Zero;
                }
            }, Roughness = 1, Reflectance = 0.2
        };

        public static DistanceField Field =
            //(new Sphere(500) * CheckerBoard + new Vector(0, -500, 0)) +
            (new Sphere(1) * new MaterialSettings { Roughness = 0, Reflectance = 1 } + new Vector(0, 0.75, 0));
    }
}
