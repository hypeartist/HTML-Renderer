using System;
using System.Drawing;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace HtmlRenderer.Web.Adapters {

    /// <summary>
    /// Adapter for System.Drawing Font object for core.
    /// </summary>
    internal sealed class FontAdapter : RFont {
        #region Fields and Consts

        /// <summary>
        /// the underline System.Drawing font.
        /// </summary>
        private readonly Font _font;

        /// <summary>
        /// a handle to this Font.
        /// </summary>
        private IntPtr _hFont;

        /// <summary>
        /// the vertical offset of the font underline location from the top of the font.
        /// </summary>
        private float _underlineOffset = -1;

        /// <summary>
        /// Cached font height.
        /// </summary>
        private float _height = -1;

        /// <summary>
        /// Cached font whitespace width.
        /// </summary>
        private double _whitespaceWidth = -1;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        public FontAdapter(Font font) {
            _font = font;
        }

        /// <summary>
        /// the underline win-forms font.
        /// </summary>
        public Font Font {
            get { return _font; }
        }

        /// <summary>
        /// Get the handle to this Font.
        /// </summary>
        public IntPtr HFont {
            get {
                if (_hFont == IntPtr.Zero)
                    _hFont = _font.ToHfont();
                return _hFont;
            }
        }

        public override double Size {
            get { return _font.Size; }
        }

        public override double UnderlineOffset {
            get { return _underlineOffset; }
        }

        public override double Height {
            get { return _height; }
        }

        public override double LeftPadding {
            get { return _height / 6f; }
        }

        public override double GetWhitespaceWidth(RGraphics graphics) {
            if (_whitespaceWidth < 0) {
                _whitespaceWidth = graphics.MeasureString(" ", this).Width;
            }
            return _whitespaceWidth;
        }

        /// <summary>
        /// Set font metrics to be cached for the font for future use.
        /// </summary>
        /// <param name="height">the full height of the font</param>
        /// <param name="underlineOffset">the vertical offset of the font underline location from the top of the font.</param>
        internal void SetMetrics(int height, int underlineOffset) {
            _height = height;
            _underlineOffset = underlineOffset;
        }
    }
}