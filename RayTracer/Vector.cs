using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public struct Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector Normalize()
        {
            var length = Length();
            return new Vector(X / length, Y / length, Z / length);
        }

        public double Length()
        {
            return Distance(this, Zero);
        }

        public double Dot(Vector other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        public Vector Cross(Vector other)
        {
            return new Vector(Y * other.Z - Z * other.Y, Z * other.X - X * other.Z, X * other.Y - Y * other.X);
        }

        public static Vector operator + (Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector operator -(Vector v)
        {
            return -1 * v;
        }

        public static Vector operator *(Vector v, double a)
        {
            return new Vector(v.X * a, v.Y * a, v.Z * a);
        }

        public static Vector operator *(double a, Vector v)
        {
            return v * a;
        }

        public static Vector operator *(Vector a, Vector b)
        {
            return new Vector(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector operator /(Vector v, double a)
        {
            return new Vector(v.X / a, v.Y / a, v.Z / a);
        }

        public static Vector Zero = new Vector(0, 0, 0);
        public static Vector One = new Vector(1, 1, 1);
        
        public static double Distance(Vector a, Vector b)
        {
            return Math.Sqrt(DistanceSquared(a, b));
        }

        public static double DistanceSquared(Vector a, Vector b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            var dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        public static Vector Random()
        {
            var vector = new Vector(ThreadSafeRandom.NextDouble() * 2 - 1, ThreadSafeRandom.NextDouble() * 2 - 1, ThreadSafeRandom.NextDouble() * 2 - 1);
            if (vector.Length() > 1)
            {
                return Random();
            }
            return vector;
        }
    }
}
