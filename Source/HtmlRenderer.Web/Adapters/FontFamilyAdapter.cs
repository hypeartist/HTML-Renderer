using System.Drawing;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace HtmlRenderer.Web.Adapters {

    /// <summary>
    /// Adapter for System.Drawing Font family object for core.
    /// </summary>
    internal sealed class FontFamilyAdapter : RFontFamily {
        /// <summary>
        /// the underline System.Drawing font.
        /// </summary>
        private readonly FontFamily _fontFamily;

        /// <summary>
        /// Init.
        /// </summary>
        public FontFamilyAdapter(FontFamily fontFamily) {
            _fontFamily = fontFamily;
        }

        /// <summary>
        /// the underline System.Drawing font family.
        /// </summary>
        public FontFamily FontFamily {
            get { return _fontFamily; }
        }

        public override string Name {
            get { return _fontFamily.Name; }
        }
    }
}