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

        public SampleResult March(DistanceField field, int iterations, int bounces, double minimum, double maximum)
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

            if (bounces != 0)
            {
                var scatterAmount = Vector.Random() * (1 - result.Reflectance);
                var reflectionTarget = Direction - 2 * result.Normal * result.Normal.Dot(Direction);
                var scatterTarget = result.Normal;
                var bounceTarget = reflectionTarget * result.Reflectance + scatterTarget * (1 - result.Reflectance);
                var scatteredBounce = bounceTarget + scatterAmount;

                var ray = new Ray(CurrentPosition + result.Normal * minimum * 2, scatteredBounce);

                var reflectionResult = ray.March(field, iterations, bounces - 1, minimum, maximum - DistanceTraveled);
                var rAmount = result.Color.R / 255.0;
                var gAmount = result.Color.G / 255.0;
                var bAmount = result.Color.B / 255.0;
                result.Color = Color.FromRgb(
                    (byte)(reflectionResult.Color.R * rAmount * (1 - result.Absorbance)),
                    (byte)(reflectionResult.Color.G * gAmount * (1 - result.Absorbance)),
                    (byte)(reflectionResult.Color.B * bAmount * (1 - result.Absorbance)));
            }

            var fogAmount = DistanceTraveled / maximum;
            if (maximum == 0)
            {
                fogAmount = 1;
            }
            var fogColor = Color.FromRgb(255, 255, 255);
            result.Color = Utils.Interpolate(result.Color, fogColor, fogAmount);

            return result;
        }
    }
}
