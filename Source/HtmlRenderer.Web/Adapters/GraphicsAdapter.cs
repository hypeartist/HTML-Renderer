// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;
using TheArtOfDev.HtmlRenderer.Adapters;
using HtmlRenderer.Web.Utilities;

namespace HtmlRenderer.Web.Adapters{

    /// <summary>
    /// Adapter for WinForms Graphics for core.
    /// </summary>
    internal sealed class GraphicsAdapter : RGraphics {

        /// <summary>
        /// used for <see cref="MeasureString(string,RFont,double,out int,out double)"/> calculation.
        /// </summary>
        private static readonly int[] _charFit = new int[1];

        /// <summary>
        /// used for <see cref="MeasureString(string,RFont,double,out int,out double)"/> calculation.
        /// </summary>
        private static readonly int[] _charFitWidth = new int[1000];

        /// <summary>
        /// Used for GDI+ measure string.
        /// </summary>
        private static readonly CharacterRange[] _characterRanges = new CharacterRange[1];

        /// <summary>
        /// The string format to use for measuring strings for GDI+ text rendering
        /// </summary>
        private static readonly StringFormat _stringFormat;

        /// <summary>
        /// The string format to use for rendering strings for GDI+ text rendering
        /// </summary>
        private static readonly StringFormat _stringFormat2;

        /// <summary>
        /// The wrapped graphics object
        /// </summary>
        private readonly Graphics _g;

        /// <summary>
        /// if to release the graphics object on dispose
        /// </summary>
        private readonly bool _releaseGraphics;

        /// <summary>
        /// If text alignment was set to RTL
        /// </summary>
        private bool _setRtl;

        /// <summary>
        /// Init static resources.
        /// </summary>
        static GraphicsAdapter() {
            _stringFormat = new StringFormat(StringFormat.GenericTypographic);
            _stringFormat.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.MeasureTrailingSpaces;
            _stringFormat2 = new StringFormat(StringFormat.GenericTypographic);
        }

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="g">the win forms graphics object to use</param>
        /// <param name="releaseGraphics">optional: if to release the graphics object on dispose (default - false)</param>
        public GraphicsAdapter(Graphics g, bool releaseGraphics = false)
            : base(WebAdapter.Instance, Utils.Convert(g.ClipBounds)) {
            ArgChecker.AssertArgNotNull(g, "g");

            _g = g;
            _releaseGraphics = releaseGraphics;
        }

        public override void PopClip() {
            _clipStack.Pop();
            _g.SetClip(Utils.Convert(_clipStack.Peek()), CombineMode.Replace);
        }

        public override void PushClip(RRect rect) {
            _clipStack.Push(rect);
            _g.SetClip(Utils.Convert(rect), CombineMode.Replace);
        }

        public override void PushClipExclude(RRect rect) {
            _clipStack.Push(_clipStack.Peek());
            _g.SetClip(Utils.Convert(rect), CombineMode.Exclude);
        }

        public override Object SetAntiAliasSmoothingMode() {
            var prevMode = _g.SmoothingMode;
            _g.SmoothingMode = SmoothingMode.AntiAlias;
            return prevMode;
        }

        public override void ReturnPreviousSmoothingMode(Object prevMode) {
            if (prevMode != null) {
                _g.SmoothingMode = (SmoothingMode)prevMode;
            }
        }

        public override RSize MeasureString(string str, RFont font) {
            var fontAdapter = (FontAdapter)font;
            var realFont = fontAdapter.Font;
            _characterRanges[0] = new CharacterRange(0, str.Length);
            _stringFormat.SetMeasurableCharacterRanges(_characterRanges);
            var size = _g.MeasureCharacterRanges(str, realFont, RectangleF.Empty, _stringFormat)[0].GetBounds(_g).Size;

            if (font.Height < 0) {
                var height = realFont.Height;
                var descent = realFont.Size * realFont.FontFamily.GetCellDescent(realFont.Style) / realFont.FontFamily.GetEmHeight(realFont.Style);
                fontAdapter.SetMetrics(height, (int)Math.Round((height - descent + .5f)));
            }

            return Utils.Convert(size);

        }

        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth) {
            charFit = 0;
            charFitWidth = 0;

            var size = MeasureString(str, font);

            for (int i = 1; i <= str.Length; i++) {
                charFit = i - 1;
                RSize pSize = MeasureString(str.Substring(0, i), font);
                if (pSize.Height <= size.Height && pSize.Width < maxWidth)
                    charFitWidth = pSize.Width;
                else
                    break;
            }

        }

        public override void DrawString(string str, RFont font, RColor color, RPoint point, RSize size, bool rtl) {
            SetRtlAlignGdiPlus(rtl);
            var brush = ((BrushAdapter)_adapter.GetSolidBrush(color)).Brush;
            _g.DrawString(str, ((FontAdapter)font).Font, brush, (int)(Math.Round(point.X) + (rtl ? size.Width : 0)), (int)Math.Round(point.Y), _stringFormat2);

        }

        public override RBrush GetTextureBrush(RImage image, RRect dstRect, RPoint translateTransformLocation) {
            var brush = new TextureBrush(((ImageAdapter)image).Image, Utils.Convert(dstRect));
            brush.TranslateTransform((float)translateTransformLocation.X, (float)translateTransformLocation.Y);
            return new BrushAdapter(brush, true);
        }

        public override RGraphicsPath GetGraphicsPath() {
            return new GraphicsPathAdapter();
        }

        public override void Dispose() {
            if (_releaseGraphics)
                _g.Dispose();

            if (_setRtl)
                _stringFormat2.FormatFlags ^= StringFormatFlags.DirectionRightToLeft;
        }



        public override void DrawLine(RPen pen, double x1, double y1, double x2, double y2) {
            _g.DrawLine(((PenAdapter)pen).Pen, (float)x1, (float)y1, (float)x2, (float)y2);
        }

        public override void DrawRectangle(RPen pen, double x, double y, double width, double height) {
            _g.DrawRectangle(((PenAdapter)pen).Pen, (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawRectangle(RBrush brush, double x, double y, double width, double height) {
            _g.FillRectangle(((BrushAdapter)brush).Brush, (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawImage(RImage image, RRect destRect, RRect srcRect) {
            _g.DrawImage(((ImageAdapter)image).Image, Utils.Convert(destRect), Utils.Convert(srcRect), GraphicsUnit.Pixel);
        }

        public override void DrawImage(RImage image, RRect destRect) {
            _g.DrawImage(((ImageAdapter)image).Image, Utils.Convert(destRect));
        }

        public override void DrawPath(RPen pen, RGraphicsPath path) {
            _g.DrawPath(((PenAdapter)pen).Pen, ((GraphicsPathAdapter)path).GraphicsPath);
        }

        public override void DrawPath(RBrush brush, RGraphicsPath path) {
            _g.FillPath(((BrushAdapter)brush).Brush, ((GraphicsPathAdapter)path).GraphicsPath);
        }

        public override void DrawPolygon(RBrush brush, RPoint[] points) {
            if (points != null && points.Length > 0) {
                _g.FillPolygon(((BrushAdapter)brush).Brush, Utils.Convert(points));
            }
        }

        /// <summary>
        /// Change text align to Left-to-Right or Right-to-Left if required.
        /// </summary>
        private void SetRtlAlignGdiPlus(bool rtl) {
            if (_setRtl) {
                if (!rtl)
                    _stringFormat2.FormatFlags ^= StringFormatFlags.DirectionRightToLeft;
            } else if (rtl) {
                _stringFormat2.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }
            _setRtl = rtl;
        }

    }
}