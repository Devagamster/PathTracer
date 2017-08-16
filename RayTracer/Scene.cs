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
        public static Vector Target = new Vector(0, 0, 0);
        public static Vector Eye = new Vector(0, 2, -4);

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
            }
        };

        public static DistanceField ReflectenceTest = 
            (new Sphere(1) * new MaterialSettings { Source = true, GetColor = _ => new Vector(16, 16, 16)} + new Vector(-3, 3, -3)) +
            (new Sphere(1) * new MaterialSettings { Source = true, GetColor = _ => new Vector(16, 16, 16)} + new Vector(3, 3, -3)) +
            (new Sphere(0.5) * new MaterialSettings { Roughness = 0, Reflectance = 1 } + new Vector(1.5, 0, 0)) +
            (new Sphere(0.5) * new MaterialSettings { Roughness = 0, Reflectance = 0.25 } + new Vector(0, 0, 0.25)) +
            (new Sphere(0.5) * new MaterialSettings { Roughness = 0, Reflectance = 0 } + new Vector(-1.5, 0, 0)) +
            (new Sphere(0.25) * new MaterialSettings { Roughness = 1, Reflectance = 0, GetColor = _ => new Vector(1, 0, 0) } + new Vector(-1, -0.25, -0.85)) +
            (new Sphere(0.25) * new MaterialSettings { Roughness = 1, Reflectance = 0, GetColor = _ => new Vector(0, 1, 0) } + new Vector(0, -0.25, -1)) +
            (new Sphere(0.25) * new MaterialSettings { Roughness = 1, Reflectance = 0, GetColor = _ => new Vector(0, 0, 1) } + new Vector(1, -0.25, -0.85)) +
            (new Sphere(0.25) * new MaterialSettings { Roughness = 0.0, Reflectance = 0.5, GetColor = _ => new Vector(1, 1, 0) } + new Vector(0.8, 0.5, 0.5)) +
            (new Sphere(0.25) * new MaterialSettings { Roughness = 1, Reflectance = 0.5, GetColor = _ => new Vector(0, 1, 1) } + new Vector(-0.8, 0.5, 0.5)) +
            (new Sphere(0.25) * new MaterialSettings { Roughness = 0.5, Reflectance = 0.5, GetColor = _ => new Vector(1, 0, 1) } + new Vector(0, 0.75, 0.7)) +
            (((new Plane(new Vector(0, 0, -1), -1.5)) +
            (new Plane(new Vector(1, 0, 0), -3.25) * new MaterialSettings { GetColor = _ => new Vector(.2, .2, 1) }) +
            (new Plane(new Vector(-1, 0, 0), -3.25) * new MaterialSettings { GetColor = _ => new Vector(1, .2, .2) }) +
            (new Plane(new Vector(0, 1, 0), -0.5))) - 
            ((new Plane(new Vector(0, 0, 1), -4) + new Plane(new Vector(0, -1, 0), -4)) + 
            (new Sphere(2) * new MaterialSettings { Roughness = 0, Reflectance = 0.5 } + new Vector(0, 0.5, 1))));

        public static DistanceField Field =
            (new Plane(new Vector(0, 1, 0), 0) - new Sphere(2)) * CheckerBoard +
            (new Sphere(1) * new MaterialSettings { Roughness = 0, Reflectance = 1 });
    }
}
