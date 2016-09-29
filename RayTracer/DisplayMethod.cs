using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RayTracer
{
    public abstract class DisplayMethod
    {
        public abstract void DrawPoint(ColoredPoint point, WriteableBitmap bitmap);
        public abstract void Reset(double pixelSize);
    }
}
