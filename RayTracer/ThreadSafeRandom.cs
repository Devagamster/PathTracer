using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class ThreadSafeRandom
    {
        private static Random global = new Random();
        [ThreadStatic]
        private static Random local;
        public static double NextDouble()
        {
            Random inst = local;
            if (inst == null)
            {
                int seed;
                lock (global) seed = global.Next();
                local = inst = new Random(seed);
            }
            return inst.NextDouble();
        }
    }
}
