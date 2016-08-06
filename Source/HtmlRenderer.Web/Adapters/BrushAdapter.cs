using System.Drawing;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace HtmlRenderer.Web.Adapters {

    /// <summary>
    /// Adapter for System.Drawing brushes objects for core.
    /// </summary>
    internal sealed class BrushAdapter : RBrush {
        /// <summary>
        /// The actual System.Drawing brush instance.
        /// </summary>
        private readonly Brush _brush;

        /// <summary>
        /// If to dispose the brush when <see cref="Dispose"/> is called.<br/>
        /// Ignore dispose for cached brushes.
        /// </summary>
        private readonly bool _dispose;

        /// <summary>
        /// Init.
        /// </summary>
        public BrushAdapter(Brush brush, bool dispose) {
            _brush = brush;
            _dispose = dispose;
        }

        /// <summary>
        /// The actual System.Drawing brush instance.
        /// </summary>
        public Brush Brush {
            get { return _brush; }
        }

        public override void Dispose() {
            if (_dispose) {
                _brush.Dispose();
            }
        }
    }
}