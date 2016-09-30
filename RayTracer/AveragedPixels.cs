﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RayTracer
{
    public class AveragedPixels : DisplayMethod
    {
        private struct Point
        {
            public Vector Color { get; set; }
            public int Count { get; set; }
        }

        private Point[,] pixels;
        private double pixelSize;
        private double diameter;

        public AveragedPixels(double diameter)
        {
            this.diameter = diameter;
        }

        public override void AddPoint(ColoredPoint point)
        {
            if (pixels != null)
            {
                var x = (int)(point.X / pixelSize) + pixels.GetLength(0) / 2;
                var y = (int)(point.Y / pixelSize) + pixels.GetLength(1) / 2;

                if (x >= 0 && x < pixels.GetLength(0) && y >= 0 && y < pixels.GetLength(1))
                {
                    var currentPoint = pixels[x, y];
                    currentPoint.Color = new Vector(
                        ((currentPoint.Color.X * currentPoint.Count) + point.Color.X) / (currentPoint.Count + 1),
                        ((currentPoint.Color.Y * currentPoint.Count) + point.Color.Y) / (currentPoint.Count + 1),
                        ((currentPoint.Color.Z * currentPoint.Count) + point.Color.Z) / (currentPoint.Count + 1));
                    currentPoint.Count++;
                    pixels[x, y] = currentPoint;
                }
            }
        }

        int currentX = 0;
        int currentY = 0;
        public override void DrawPiece(WriteableBitmap bitmap)
        {
            var pointX = currentX + (pixels.GetLength(0) - bitmap.PixelWidth) / 2;
            var pointY = currentY + (pixels.GetLength(1) - bitmap.PixelHeight) / 2;
            var point = pixels[pointX, pointY];
            bitmap.SetPixel(currentX, currentY, Color.FromRgb((byte)(point.Color.X * 255.999), (byte)(point.Color.Y * 255.999), (byte)(point.Color.Z * 255.999)));
            currentX++;
            if (currentX >= bitmap.PixelWidth)
            {
                currentX = 0;
                currentY++;
                if (currentY >= bitmap.PixelHeight)
                {
                    currentY = 0;
                }
            }
        }

        public override void Reset(double pixelSize)
        {
            this.pixelSize = pixelSize;
            pixels = new Point[(int)(diameter / pixelSize), (int)(diameter / pixelSize)];
        }
    }
}