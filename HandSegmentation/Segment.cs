using System;
using AForge.Imaging.Filters;
using System.Drawing;

namespace HandSegmentation
{
    public class Segment
    {
        private bool isSkin(Color c)
        {
            return (c.R > 95 && c.G > 40 && c.B > 20) &&
                (Math.Max(c.R, Math.Max(c.G, c.R)) -
                Math.Min(c.R, Math.Min(c.G, c.B))) > 15 &&
                            (Math.Abs(c.R - c.G) > 15 && c.R > c.G && c.R > c.G);
        }

    }
}
