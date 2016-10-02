using RayTracer.DistanceFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public static class Model
    {
        public static Vector WorldUp = new Vector(0, 1, 0);
        public static Vector Target = new Vector(0, 0, 0);
        public static Vector Eye = new Vector(0, 2, -4);

        public static Vector Fog = Vector.Zero;

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
            }
        };

        public static DistanceField Field =
            (new Sphere(1) * new MaterialSettings { Source = true, GetColor = _ => new Vector(4, 4, 4)} + new Vector(-5, 5, -5)) +
            (new Sphere(0.5) * new MaterialSettings { Roughness = 0, Reflectance = 1 } + new Vector(1.5, 0, 0)) +
            (new Sphere(0.5) * new MaterialSettings { Roughness = 0, Reflectance = 0.5 } + new Vector(0, 0, 0)) +
            (new Sphere(0.5) * new MaterialSettings { Roughness = 0, Reflectance = 0 } + new Vector(-1.5, 0, 0)) +
            (new Sphere(0.25) * new MaterialSettings { Roughness = 1, Reflectance = 0, GetColor = _ => new Vector(1, 0, 0) } + new Vector(-1, -0.25, -0.75)) +
            (new Sphere(0.25) * new MaterialSettings { Roughness = 1, Reflectance = 0, GetColor = _ => new Vector(0, 1, 0) } + new Vector(0, -0.25, -0.75)) +
            (new Sphere(0.25) * new MaterialSettings { Roughness = 1, Reflectance = 0, GetColor = _ => new Vector(0, 0, 1) } + new Vector(1, -0.25, -0.75)) +
            (new Plane(new Vector(0, 0, -1), -2)) +
            (new Plane(WorldUp, -0.5));

    }
}
