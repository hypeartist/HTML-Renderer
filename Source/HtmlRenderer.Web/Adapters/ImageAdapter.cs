using System.Drawing;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace HtmlRenderer.Web.Adapters {

    /// <summary>
    /// Adapter for System.Drawing Image object for core.
    /// </summary>
    internal sealed class ImageAdapter : RImage {
        /// <summary>
        /// the underline System.Drawing image.
        /// </summary>
        private readonly Image _image;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ImageAdapter(Image image) {
            _image = image;
        }

        /// <summary>
        /// the underline win-forms image.
        /// </summary>
        public Image Image {
            get { return _image; }
        }

        public override double Width {
            get { return _image.Width; }
        }

        public override double Height {
            get { return _image.Height; }
        }

        public override void Dispose() {
            _image.Dispose();
        }
    }
}