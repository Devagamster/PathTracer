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
            Planet = new Sphere(planetRadius);
            Atmosphere = !(new Sphere(atmosphereRadius));
            ScaleHeightRayleigh = scaleHeightRayleigh;
            ScaleHeightMei = scaleHeightMei;
            PlanetRadius = planetRadius;
            SunIntensity = sunIntensity;
        }

        public Vector SunDirection { get; set; }
        public DistanceField Planet { get; set; }
        public DistanceField Atmosphere { get; set; }
        public double ScaleHeightRayleigh { get; set; }
        public double ScaleHeightMei { get; set; }
        public double PlanetRadius { get; set; }
        public double SunIntensity { get; set; }

        const int GatheringSamples = 4;
        public Vector CalculateSkyColor(Vector rayDirection, double cameraHeight)
        {
            var cameraPosition = new Vector(0, cameraHeight + PlanetRadius + 3000, 0);
            var viewIntersect = AtmosphereIntersection(cameraPosition, rayDirection);
            var mu = rayDirection.Dot(SunDirection);
            var rayleighPhase = RayleighPhaseFunction(mu);
            var meiPhase = MeiPhaseFunction(mu);
            if (viewIntersect == null)
            {
                return Vector.Zero;
            }
            var a = cameraPosition;
            var b = viewIntersect.Value;
            var rayleighSum = Vector.Zero;
            var meiSum = Vector.Zero;
            var currentPos = cameraPosition;
            var diff = b - a;
            currentPos += rayDirection * diff.Length() / (GatheringSamples * 2);
            while ((currentPos - a).Length() < diff.Length())
            {
                var sunIntersect = AtmosphereIntersection(currentPos, SunDirection);
                if (sunIntersect != null)
                {
                    var atmosphereHeight = currentPos.Length() - PlanetRadius;
                    var transmittance =
                        TransmittanceFromAToB(a, currentPos) *
                        TransmittanceFromAToB(currentPos, sunIntersect.Value);
                    rayleighSum +=
                        transmittance *
                        RayleighExtinctionCoefficients(atmosphereHeight);
                    meiSum +=
                        transmittance *
                        MeiExtinctionCoefficients(atmosphereHeight);
                    currentPos += rayDirection * diff.Length() / GatheringSamples;
                }
            }

            return SunIntensity * (rayleighPhase * rayleighSum * 0 + meiPhase * meiSum);
        }

        const int NumberOfTransmittanceSamples = 4;
        public Vector TransmittanceFromAToB(Vector a, Vector b)
        {
            var sum = Vector.Zero;
            var currentPos = a;
            var diff = a - b;
            var dir = diff.Normalize();
            currentPos += dir * diff.Length() / (NumberOfTransmittanceSamples * 2);
            while ((currentPos - a).Length() < diff.Length())
            {
                var atmosphereHeight = currentPos.Length() - PlanetRadius;
                sum += (MeiExtinctionCoefficients(atmosphereHeight) + RayleighExtinctionCoefficients(atmosphereHeight)) * diff.Length() / NumberOfTransmittanceSamples;
                currentPos += dir * diff.Length() / NumberOfTransmittanceSamples;
            }
            return new Vector(
                Math.Exp(-sum.X),
                Math.Exp(-sum.Y),
                Math.Exp(-sum.Z)
            );
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
            var field = Planet + Atmosphere;
            while (true)
            {
                var sample = field.Sample(pos);
                if (sample.Distance < 100)
                {
                    return pos;
                }

                var amount = sample.Distance;
                if (amount < 100) amount = 100;
                pos += amount * dir;
            }
        }
    }
}
