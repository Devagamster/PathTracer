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
            double cameraHeight, 
            double planetRadius = 6360e3, 
            double atmosphereRadius = 6420e3,
            double scaleHeightRayleigh = 7994,
            double scaleHeightMei = 1200)
        {
            SunDirection = sunDirection;
            CameraHeight = cameraHeight + planetRadius;
            Planet = new Sphere(planetRadius);
            Atmosphere = !(new Sphere(atmosphereRadius));
            ScaleHeightRayleigh = scaleHeightRayleigh;
            ScaleHeightMei = scaleHeightMei;
            PlanetRadius = planetRadius;
        }

        public Vector SunDirection { get; set; }
        public double CameraHeight { get; set; }
        public DistanceField Planet { get; set; }
        public DistanceField Atmosphere { get; set; }
        public double ScaleHeightRayleigh { get; set; }
        public double ScaleHeightMei { get; set; }
        public double PlanetRadius { get; set; }

        const int GatheringSamples = 16;
        public Vector CalculateSkyColor(Vector rayDirection)
        {
            var cameraPosition = new Vector(0, CameraHeight, 0);
            var atmosphereIntersect = AtmosphereIntersection(cameraPosition, rayDirection);
            if (atmosphereIntersect == null)
            {
                return Vector.Zero;
            }
            var a = cameraPosition;
            var b = atmosphereIntersect.Value;
            var sum = Vector.Zero;
            var currentPos = cameraPosition;
            var diff = b - a;
            currentPos += rayDirection * diff.Length() / GatheringSamples;
            while ((currentPos - a).Length() < diff.Length())
            {
                var rayTrans = TransmittanceFromAToB(a, currentPos, RayleighExtinctionCoefficients);
                var rayLight = LightScatteringAlongDirection(rayDirection, currentPos,
                    RayleighExtinctionCoefficients,
                    RayleighExtinctionCoefficients, RayleighPhaseFunction);
                var meiTrans = TransmittanceFromAToB(a, currentPos, MeiExtinctionCoefficients);
                var meiLight = LightScatteringAlongDirection(rayDirection, currentPos,
                    MeiScatteringCoefficients,
                    MeiExtinctionCoefficients,
                    MeiPhaseFunction);
                sum += (rayTrans * rayLight + meiTrans * meiLight) * diff.Length() / GatheringSamples;
                currentPos += rayDirection * diff.Length() / GatheringSamples;
            }

            return sum;
        }

        const double SunIntensity = 2000;
        public Vector LightScatteringAlongDirection(
            Vector viewDir, Vector X,
            Func<double, Vector> scatteringAtHeight,
            Func<double, Vector> extinctionAtHeight,
            Func<double, double> phaseFunction)
        {
            var atmosphereIntersect = AtmosphereIntersection(X, SunDirection);
            if (atmosphereIntersect == null)
            {
                return Vector.Zero;
            }
            return SunIntensity *
                TransmittanceFromAToB(X, atmosphereIntersect.Value, scatteringAtHeight) *
                phaseFunction(viewDir.Dot(SunDirection)) *
                scatteringAtHeight(X.Length() - PlanetRadius);
        }

        const int NumberOfTransmittanceSamples = 16;
        public Vector TransmittanceFromAToB(Vector a, Vector b, Func<double, Vector> extinctionAtHeight)
        {
            var sum = Vector.Zero;
            var currentPos = a;
            var diff = a - b;
            var dir = diff.Normalize();
            currentPos += dir * diff.Length() / (NumberOfTransmittanceSamples * 2);
            while ((currentPos - a).Length() < diff.Length())
            {
                sum += extinctionAtHeight(currentPos.Length() - PlanetRadius) * diff.Length() / NumberOfTransmittanceSamples;
                currentPos += dir * diff.Length() / NumberOfTransmittanceSamples;
            }
            return new Vector(
                Math.Exp(-sum.X),
                Math.Exp(-sum.Y),
                Math.Exp(-sum.Z)
            );
        }

        public Vector RayleighExtinctionCoefficientsAtSeaLevel = new Vector(
            33.1e-6,
            13.5e-6,
            5.8e-6
        );
        public Vector RayleighExtinctionCoefficients(double height)
        {
            return RayleighExtinctionCoefficientsAtSeaLevel * Math.Exp(-height / ScaleHeightRayleigh);
        }

        public double RayleighPhaseFunction(double angleBetweenLeightAndViewing)
        {
            double mu = angleBetweenLeightAndViewing;
            return (3.0 / (16.0 * Math.PI)) * (1.0 + mu * mu);
        }

        public Vector MeiScatteringCoefficientsAtSeaLevel = new Vector(
            210e-5,
            210e-5,
            210e-5
        );
        public Vector MeiScatteringCoefficients(double height)
        {
            return MeiScatteringCoefficientsAtSeaLevel * Math.Exp(-height / ScaleHeightMei);
        }

        public Vector MeiExtinctionCoefficients(double height)
        {
            return MeiScatteringCoefficients(height) * 1.1;
        }

        public double MeiPhaseFunction(double angleBetweenLeightAndViewing)
        {
            double mu = angleBetweenLeightAndViewing;
            double g = 0.76;
            return (3.0 / (8.0 * Math.PI)) * ((1.0 - g * g) * (1.0 + mu * mu)) / ((2.0 + g * g) * Math.Pow(1.0 + g * g + 2.0 * g * mu, 3.0 / 2.0));
        }

        public Vector? AtmosphereIntersection(Vector pos, Vector dir)
        {
            var field = Planet + Atmosphere;
            while (true)
            {
                var sample = field.Sample(pos);
                if (sample.Distance < 0.01)
                {
                    if (Planet.Sample(pos).Distance < Atmosphere.Sample(pos).Distance)
                    {
                        return null;
                    }
                    else
                    {
                        return pos;
                    }
                }

                pos += sample.Distance * dir;
            }
        }
    }
}
