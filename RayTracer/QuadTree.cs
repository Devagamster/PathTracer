using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RayTracer
{
    public enum NodeState
    {
        Branch,
        Sprout,
        Leaf
    }

    public class QuadTree : DisplayMethod
    {
        public const int MAX_CHILDREN = 4;
        
        public double Diameter { get; set; }

        public double CenterX { get; set; }
        public double CenterY { get; set; }

        public NodeState State { get; set; }

        public double PixelSize { get; set; }
        public int PointCount { get; set; }
        public List<ColoredPoint>[] Points { get; set; }
        public QuadTree[] Children { get; set; }
        public Color CurrentColor { get; set; }

        public QuadTree(double pixelSize, double diameter, double centerX, double centerY, List<ColoredPoint> points)
        {
            PixelSize = pixelSize;
            Diameter = diameter;
            CenterX = centerX;
            CenterY = centerY;

            if (Diameter <= PixelSize)
            {
                State = NodeState.Leaf;
                CurrentColor = Color.FromRgb(0, 0, 0);
            }
            else
            {
                Reset(pixelSize);
            }

            foreach (var p in points)
            {
                AddPoint(p);
            }

        }

        public override void Reset(double pixelSize)
        {
            State = NodeState.Sprout;
            PixelSize = pixelSize;
            Points = new List<ColoredPoint>[4];
            Points[0] = new List<ColoredPoint>();
            Points[1] = new List<ColoredPoint>();
            Points[2] = new List<ColoredPoint>();
            Points[3] = new List<ColoredPoint>();
            Children = null;
            PointCount = 0;
        }

        public void SplitIfNecessary()
        {
            if (State == NodeState.Sprout && Points[0].Any() && Points[1].Any() && Points[2].Any() && Points[3].Any())
            {
                State = NodeState.Branch;
                Children = new QuadTree[4];
                var radius = Diameter / 2;
                var halfRadius = radius / 2;
                Children[0] = new QuadTree(PixelSize, radius, CenterX - halfRadius, CenterY + halfRadius, Points[0]);
                Children[1] = new QuadTree(PixelSize, radius, CenterX + halfRadius, CenterY + halfRadius, Points[1]);
                Children[2] = new QuadTree(PixelSize, radius, CenterX - halfRadius, CenterY - halfRadius, Points[2]);
                Children[3] = new QuadTree(PixelSize, radius, CenterX + halfRadius, CenterY - halfRadius, Points[3]);
                Points = null;
            }
        }
        
        public List<QuadTree> AddPoint(ColoredPoint p)
        {
            var dirtyList = new List<QuadTree>();
            int quadIndex = 0;
            if (p.X > CenterX)
            {
                if (p.Y > CenterY)
                {
                    quadIndex = 1;
                }
                else
                {
                    quadIndex = 3;
                }
            }
            else
            {
                if (p.Y > CenterY)
                {
                    quadIndex = 0;
                }
                else
                {
                    quadIndex = 2;
                }
            }

            if (State == NodeState.Sprout)
            {
                Points[quadIndex].Add(p);
                SplitIfNecessary();
                if (State == NodeState.Branch)
                {
                    dirtyList.AddRange(Children);
                }
                else
                {
                    PointCount++;
                    dirtyList.Add(this);
                }
            }
            else if (State == NodeState.Leaf)
            {
                var r = CurrentColor.R * PointCount;
                var g = CurrentColor.G * PointCount;
                var b = CurrentColor.B * PointCount;
                PointCount++;
                r += p.Color.R;
                g += p.Color.G;
                b += p.Color.B;

                CurrentColor = Color.FromRgb((byte)(r / PointCount), (byte)(g / PointCount), (byte)(b / PointCount));
            }
            else
            {
                dirtyList.AddRange(Children[quadIndex].AddPoint(p));
            }
            return dirtyList;
        }

        public void Draw(WriteableBitmap bitmap)
        {
            if (State == NodeState.Leaf || State == NodeState.Sprout)
            {
                var radiusInPixels = Diameter / PixelSize / 2;
                var centerXInPixels = CenterX / PixelSize + bitmap.Width / 2;
                var centerYInPixels = CenterY / PixelSize + bitmap.Height / 2;

                bitmap.FillRectangle(
                    (int)(centerXInPixels - radiusInPixels),
                    (int)(centerYInPixels - radiusInPixels),
                    (int)(centerXInPixels + radiusInPixels),
                    (int)(centerYInPixels + radiusInPixels),
                    GetColor());
            }
            else
            {
                foreach(var child in Children)
                {
                    child.Draw(bitmap);
                }
            }
        }

        public Color GetColor()
        {
            if (State == NodeState.Sprout)
            {
                int r = 0;
                int g = 0;
                int b = 0;

                foreach (var list in Points)
                {
                    foreach (var p in list)
                    {
                        r += p.Color.R;
                        g += p.Color.G;
                        b += p.Color.B;
                    }
                }

                if (PointCount == 0)
                {
                    return Color.FromRgb(0, 0, 0);
                }

                return Color.FromRgb((byte)(r / PointCount), (byte)(g / PointCount), (byte)(b / PointCount));
            }
            else if (State == NodeState.Leaf)
            {
                return CurrentColor;
            }
            else
            {
                throw new InvalidOperationException("Quad tree does not have a color when it is a branch.");
            }
        }

        public override void DrawPoint(ColoredPoint point, WriteableBitmap bitmap)
        {
            var dirty = AddPoint(point);
            foreach (var dirtyQuad in dirty)
            {
                dirtyQuad.Draw(bitmap);
            }
        }
    }
}
