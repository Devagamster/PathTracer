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

        public static DistanceField operator+ (DistanceField first, DistanceField second)
        {
            return new Union(first, second);
        }

        public static Translation operator+ (DistanceField field, Vector translation)
        {
            return new Translation(field, translation);
        }

        public static Translation operator+ (Vector translation, DistanceField field)
        {
            return field + translation;
        }

        public static Texture operator+ (DistanceField field, Color color)
        {
            return new Texture(field, color);
        }

        public static Texture operator+ (Color color, DistanceField field)
        {
            return field + color;
        }

        public static Texture operator+ (DistanceField field, Func<Vector, Color> getColor)
        {
            return new Texture(field, getColor);
        }

        public static Texture operator+ (Func<Vector, Color> getColor, DistanceField field)
        {
            return field + getColor;
        }

        public static Reflection operator/ (DistanceField field, double reflectance)
        {
            return new Reflection(field, reflectance);
        }
    }
}
