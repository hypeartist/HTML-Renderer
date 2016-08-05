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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Adapters;


namespace HtmlRenderer.Test {

    /// <summary>
    /// Test Adapter for tests.
    /// </summary>
    internal sealed class TestAdapter : RAdapter {

        /// <summary>
        /// Singleton instance of global adapter.
        /// </summary>
        private static readonly TestAdapter _instance = new TestAdapter();

        /// <summary>
        /// Init installed font families and set default font families mapping.
        /// </summary>
        private TestAdapter() {
            AddFontFamilyMapping("monospace", "Courier New");
            AddFontFamilyMapping("Helvetica", "Arial");

            foreach (var family in FontFamily.Families) {
                AddFontFamily(new FontFamilyAdapter(family));
            }
        }

        /// <summary>
        /// Singleton instance of global adapter.
        /// </summary>
        public static TestAdapter Instance {
            get { return _instance; }
        }

        protected override RColor GetColorInt(string colorName) {
            var color = Color.FromName(colorName);
            return Utils.Convert(color);
        }

        protected override RPen CreatePen(RColor color) {
            return new PenAdapter(new Pen(Utils.Convert(color)));
        }

        protected override RBrush CreateSolidBrush(RColor color) {
            Brush solidBrush;
            if (color == RColor.White)
                solidBrush = Brushes.White;
            else if (color == RColor.Black)
                solidBrush = Brushes.Black;
            else if (color.A < 1)
                solidBrush = Brushes.Transparent;
            else
                solidBrush = new SolidBrush(Utils.Convert(color));

            return new BrushAdapter(solidBrush, false);
        }

        protected override RBrush CreateLinearGradientBrush(RRect rect, RColor color1, RColor color2, double angle) {
            return new BrushAdapter(new LinearGradientBrush(Utils.Convert(rect), Utils.Convert(color1), Utils.Convert(color2), (float)angle), true);
        }

        protected override RImage ConvertImageInt(object image) {
            return image != null ? new ImageAdapter((Image)image) : null;
        }

        protected override RImage ImageFromStreamInt(Stream memoryStream) {
            return new ImageAdapter(Image.FromStream(memoryStream));
        }

        protected override RFont CreateFontInt(string family, double size, RFontStyle style) {
            var fontStyle = (FontStyle)((int)style);
            return new FontAdapter(new Font(family, (float)size, fontStyle));
        }

        protected override RFont CreateFontInt(RFontFamily family, double size, RFontStyle style) {
            var fontStyle = (FontStyle)((int)style);
            return new FontAdapter(new Font(((FontFamilyAdapter)family).FontFamily, (float)size, fontStyle));
        }

        //protected override object GetClipboardDataObjectInt(string html, string plainText) {
        //    return ClipboardHelper.CreateDataObject(html, plainText);
        //}

        //protected override void SetToClipboardInt(string text) {
        //    ClipboardHelper.CopyToClipboard(text);
        //}

        //protected override void SetToClipboardInt(string html, string plainText) {
        //    ClipboardHelper.CopyToClipboard(html, plainText);
        //}

        //protected override void SetToClipboardInt(RImage image) {
        //    Clipboard.SetImage(((ImageAdapter)image).Image);
        //}

        //protected override RContextMenu CreateContextMenuInt() {
        //    return new ContextMenuAdapter();
        //}

        //protected override void SaveToFileInt(RImage image, string name, string extension, RControl control = null) {
          
        //            ((ImageAdapter)image).Image.Save(@"C:\Temp\" + name);
          
        //}
    }
}