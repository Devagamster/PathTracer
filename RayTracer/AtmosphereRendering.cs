using RayTracer.DistanceFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class AtmosphereRendering
    {
        public AtmosphereRendering(
            Vector sunDirection, 
            double planetRadius = 6360e3, 
            double atmosphereRadius = 6420e3,
            double scaleHeightRayleigh = 7994,
            double scaleHeightMei = 1200,
            double sunIntensity = 20)
        {
            SunDirection = sunDirection;
            ScaleHeightRayleigh = scaleHeightRayleigh;
            ScaleHeightMei = scaleHeightMei;
            PlanetRadius = planetRadius;
            AtmosphereRadius = atmosphereRadius;
            SunIntensity = sunIntensity;
        }

        public Vector SunDirection { get; set; }
        public double ScaleHeightRayleigh { get; set; }
        public double ScaleHeightMei { get; set; }
        public double PlanetRadius { get; set; }
        public double AtmosphereRadius { get; set; }
        public double SunIntensity { get; set; }
        
        public Vector CalculateSkyColor(Vector rayDirection, Vector cameraPosition)
        {
            cameraPosition = new Vector(cameraPosition.X, cameraPosition.Y + PlanetRadius, cameraPosition.Z);
            var viewIntersect = AtmosphereIntersection(cameraPosition, rayDirection);
            var mu = rayDirection.Dot(SunDirection);
            var rayleighPhase = RayleighPhaseFunction(mu);
            var meiPhase = MeiPhaseFunction(mu);
            if (viewIntersect == null)
            {
                return Vector.Zero;
            }
            return NumericalIntegration(cameraPosition, viewIntersect.Value, 16,
                (pos) =>
                {
                    var sunIntersect = AtmosphereIntersection(pos, SunDirection);
                    if (sunIntersect == null)
                    {
                        return Vector.Zero;
                    }
                    var atmosphereHeight = pos.Length() - PlanetRadius;

                    var transCameraToPos = Transmittance(cameraPosition, pos);
                    var transPosToSky = Transmittance(pos, sunIntersect.Value);
                    var rayExtinction = rayleighPhase * RayleighExtinctionCoefficients(atmosphereHeight);
                    var meiExtinction = meiPhase * MeiExtinctionCoefficients(atmosphereHeight);
                    return SunIntensity * transCameraToPos * transPosToSky * (rayExtinction + meiExtinction);
                });
        }

        public Vector Transmittance(Vector a, Vector b)
        {
            var result = NumericalIntegration(a, b, 16, (pos) =>
            {
                var atmosphereDistance = pos.Length() - PlanetRadius;
                var rayleighExtinction = RayleighExtinctionCoefficients(atmosphereDistance);
                var meiExtinction = MeiExtinctionCoefficients(atmosphereDistance);
                return rayleighExtinction + meiExtinction;
            });
            return new Vector(Math.Exp(-result.X), Math.Exp(-result.Y), Math.Exp(-result.Z));
        }

        public Vector NumericalIntegration(Vector a, Vector b, int sampleCount, Func<Vector, Vector> body)
        {
            var currentPos = a;
            var diff = b - a;
            var dir = diff.Normalize();
            var totalDistance = diff.Length();
            var sampleDistance = totalDistance / sampleCount;   
            var sampleDelta = dir * sampleDistance;
            currentPos += sampleDelta / 2;
            var sum = Vector.Zero;
            for (int i = 0; i < sampleCount; i++)
            {
                sum += body(currentPos) * sampleDistance;
                currentPos += sampleDelta;
            }
            return sum;
        }

        public Vector RayleighExtinctionCoefficientsAtSeaLevel = new Vector(
            3.8e-6,
            13.5e-6,
            33.1e-6
        );
        public Vector RayleighExtinctionCoefficients(double height)
        {
            return RayleighExtinctionCoefficientsAtSeaLevel * Math.Exp(-height / ScaleHeightRayleigh);
        }

        public double RayleighPhaseFunction(double angleBetweenLightAndViewing)
        {
            double mu = angleBetweenLightAndViewing;
            return 3.0 / (16.0 * Math.PI) * (1.0 + mu * mu);
        }

        public Vector MeiScatteringCoefficientsAtSeaLevel = new Vector(
            21e-6,
            21e-6,
            21e-6
        );
        public Vector MeiScatteringCoefficients(double height)
        {
            return MeiScatteringCoefficientsAtSeaLevel * Math.Exp(-height / ScaleHeightMei);
        }

        public Vector MeiExtinctionCoefficients(double height)
        {
            return MeiScatteringCoefficients(height) * 1.1;
        }

        public double MeiPhaseFunction(double angleBetweenLightAndViewing)
        {
            double mu = angleBetweenLightAndViewing;
            double g = 0.76;
            return 3.0 / (8.0 * Math.PI) * ((1.0 - g * g) * (1.0 + mu * mu)) / ((2.0 + g * g) * Math.Pow(1.0 + g * g - 2.0 * g * mu, 3.0 / 2.0));
        }

        public Vector? AtmosphereIntersection(Vector pos, Vector dir)
        {
            var intersection = RaySphereIntersection(pos, dir, PlanetRadius);
            if (intersection == null)
            {
                return RaySphereIntersection(pos, dir, AtmosphereRadius);
            }
            return intersection;
        }

        public Vector? RaySphereIntersection(Vector origin, Vector dir, double radius)
        {
            var a = dir.Dot(dir);
            var b = 2 * dir.Dot(origin);
            var c = origin.Dot(origin) - radius * radius;

            var desc = b * b - 4 * a * c;
            if (desc < 0)
            {
                return null;
            }

            var t1 = (-b + Math.Sqrt(desc)) / (2 * a);
            var t2 = (-b - Math.Sqrt(desc)) / (2 * a);

            if (t1 > t2)
            {
                var temp = t1;
                t1 = t2;
                t2 = temp;
            }
            var t = t1;
            if (t < 0) t = t2;
            if (t < 0) return null;

            return origin + t * dir;
        }
    }
}
