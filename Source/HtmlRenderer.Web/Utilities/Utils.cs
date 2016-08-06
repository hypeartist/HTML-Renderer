using System;
using System.Drawing;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace HtmlRenderer.Web.Utilities {
    /// <summary>
    /// Utilities for converting System.Drawing entities to HtmlRenderer core entities.
    /// </summary>
    internal static class Utils {
        /// <summary>
        /// Convert from WinForms point to core point.
        /// </summary>
        public static RPoint Convert(PointF p) {
            return new RPoint(p.X, p.Y);
        }

        /// <summary>
        /// Convert from System.Drawing point to core point.
        /// </summary>
        public static PointF[] Convert(RPoint[] points) {
            PointF[] myPoints = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
                myPoints[i] = Convert(points[i]);
            return myPoints;
        }

        /// <summary>
        /// Convert from core point to System.Drawing point.
        /// </summary>
        public static PointF Convert(RPoint p) {
            return new PointF((float)p.X, (float)p.Y);
        }

        /// <summary>
        /// Convert from core point to System.Drawing point.
        /// </summary>
        public static Point ConvertRound(RPoint p) {
            return new Point((int)Math.Round(p.X), (int)Math.Round(p.Y));
        }

        /// <summary>
        /// Convert from System.Drawing size to core size.
        /// </summary>
        public static RSize Convert(SizeF s) {
            return new RSize(s.Width, s.Height);
        }

        /// <summary>
        /// Convert from core size to System.Drawing size.
        /// </summary>
        public static SizeF Convert(RSize s) {
            return new SizeF((float)s.Width, (float)s.Height);
        }

        /// <summary>
        /// Convert from core size to System.Drawing size.
        /// </summary>
        public static Size ConvertRound(RSize s) {
            return new Size((int)Math.Round(s.Width), (int)Math.Round(s.Height));
        }

        /// <summary>
        /// Convert from System.Drawing rectangle to core rectangle.
        /// </summary>
        public static RRect Convert(RectangleF r) {
            return new RRect(r.X, r.Y, r.Width, r.Height);
        }

        /// <summary>
        /// Convert from core rectangle to System.Drawing rectangle.
        /// </summary>
        public static RectangleF Convert(RRect r) {
            return new RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
        }

        /// <summary>
        /// Convert from core rectangle to System.Drawing rectangle.
        /// </summary>
        public static Rectangle ConvertRound(RRect r) {
            return new Rectangle((int)Math.Round(r.X), (int)Math.Round(r.Y), (int)Math.Round(r.Width), (int)Math.Round(r.Height));
        }

        /// <summary>
        /// Convert from System.Drawing color to core color.
        /// </summary>
        public static RColor Convert(Color c) {
            return RColor.FromArgb(c.A, c.R, c.G, c.B);
        }

        /// <summary>
        /// Convert from core color to System.Drawing color.
        /// </summary>
        public static Color Convert(RColor c) {
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }
    }
}