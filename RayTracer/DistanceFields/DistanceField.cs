using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RayTracer.DistanceFields
{
    public abstract class DistanceField
    {
        public abstract SampleResult Sample(Vector pos);

        public static Union operator+ (DistanceField first, DistanceField second)
        {
            return new Union(first, second);
        }

        public static Subtraction operator- (DistanceField first, DistanceField second)
        {
            return new Subtraction(first, second);
        }

        public static Intersection operator* (DistanceField first, DistanceField second)
        {
            return new Intersection(first, second);
        }

        public static Translation operator+ (DistanceField field, Vector translation)
        {
            return new Translation(field, translation);
        }

        public static Translation operator+ (Vector translation, DistanceField field)
        {
            return field + translation;
        }

        public static MaterialChange operator* (DistanceField field, MaterialSettings settings)
        {
            return new MaterialChange(field, settings);
        }

        public static MaterialChange operator* (MaterialSettings settings, DistanceField field)
        {
            return field * settings;
        }

        public static Repitition operator* (DistanceField field, Vector distance)
        {
            return new Repitition(field, distance);
        }

        public static Repitition operator* (Vector distance, DistanceField field)
        {
            return field * distance;
        }
    }
}
