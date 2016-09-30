using RayTracer.DistanceFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RayTracer
{
    public class Ray
    {
        public Vector CurrentPosition { get; set; }
        public Vector Direction { get; set; }

        public double DistanceTraveled { get; set; }
        public long Jumps { get; set; }

        public Ray(Vector position, Vector direction)
        {
            CurrentPosition = position;
            Direction = direction.Normalize();
            DistanceTraveled = 0;
            Jumps = 0;
        }

        public SampleResult March(DistanceField field, double minimum, double maximum, Vector fog)
        {
            SampleResult result = field.Sample(CurrentPosition);
            while (true)
            {
                Jumps++;
                if (result.Distance < minimum)
                {
                    break;
                }
                if (DistanceTraveled > maximum)
                {
                    DistanceTraveled = maximum;
                    break;
                }
                CurrentPosition = CurrentPosition + Direction * result.Distance;
                DistanceTraveled += result.Distance;
                result = field.Sample(CurrentPosition);
            }

            var bouncedColor = Vector.Zero;
            if (result.Source)
            {
                bouncedColor = result.Color;
            }
            else
            {
                if (ThreadSafeRandom.NextDouble() > result.Absorbance)
                {
                    var scatterAmount = Vector.Random() * result.Roughness;
                    var reflectionTarget = Direction - 2 * result.Normal * result.Normal.Dot(Direction);
                    var scatterTarget = result.Normal;
                    var bounceTarget = reflectionTarget * (1 - result.Roughness) + scatterTarget * result.Roughness;
                    var scatteredBounce = bounceTarget + scatterAmount;

                    var target = scatteredBounce;

                    if (ThreadSafeRandom.NextDouble() >= result.Reflectance)
                    {
                        target = scatterTarget + Vector.Random();
                    }
                    var ray = new Ray(CurrentPosition + result.Normal * minimum * 2, target);

                    var reflectionResult = ray.March(field, minimum, maximum - DistanceTraveled, fog);
                    var rAmount = Utils.Interpolate(result.Color.X, 1, result.Reflectance);
                    var gAmount = Utils.Interpolate(result.Color.Y, 1, result.Reflectance);
                    var bAmount = Utils.Interpolate(result.Color.Z, 1, result.Reflectance);
                    bouncedColor = new Vector(
                        reflectionResult.Color.X * rAmount,
                        reflectionResult.Color.Y * gAmount,
                        reflectionResult.Color.Z * bAmount);
                }
            }

            var fogAmount = DistanceTraveled / maximum;
            if (maximum == 0)
            {
                fogAmount = 1;
            }
            result.Color = Utils.Interpolate(bouncedColor, fog, fogAmount);

            return result;
        }
    }
}
