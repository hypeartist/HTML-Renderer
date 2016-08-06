using HtmlRenderer.Web.Adapters;
using HtmlRenderer.Web.Utilities;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;


namespace HtmlRenderer.Web {
    /// <summary>
    /// Standalone static class for rendering HTML to image.
    /// </summary>
    /// <example>
    /// HtmlRender.RenderToImage("<![CDATA[<div>Hello <b>World</b></div>]]>", 600, 400);
    /// </example>
    public static class HtmlRender {

        /// <summary>
        /// Renders the specified HTML into a new image of the requested size. The HTML will be layout by the given size but will be clipped if it cannot fit.
        /// </summary>
        /// <param name="html">HTML source to render.</param>
        /// <param name="width">The width of the image to render into.</param>
        /// <param name="height">The height of the image to render into.</param>
        /// <param name="margin">The margin used when rendering the HTML into the image.</param>
        /// <param name="backgroundColor">The color to fill the image with (the image cannot have transparent background, by default it will be white).</param>
        /// <param name="cssData">The style to use for html rendering (uses W3 default style if not specified)</param>
        /// <param name="stylesheetLoad">Can be used to overwrite stylesheet resolution logic.</param>
        /// <param name="imageLoad">Can be used to overwrite image resolution logic.</param>
        /// <returns>The generated image of the html.</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="backgroundColor"/> is <see cref="Color.Transparent"/></exception>.
        public static Image RenderToImage(string html, int width, int height, int margin = 0, Color backgroundColor = new Color(), CssData cssData = null,
            EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null) {
            if (string.IsNullOrWhiteSpace(html)) {
                return null;
            }

            if (backgroundColor == Color.Transparent) {
                throw new ArgumentOutOfRangeException(nameof(backgroundColor), "Transparent background in not supported");
            }

            var image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(image)) {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.Clear(backgroundColor);

                using (var container = new HtmlContainerInt(WebAdapter.Instance)) {
                    container.PageSize = Utils.Convert(new SizeF(width, height));
                    container.Location = Utils.Convert(new PointF(margin, margin));
                    container.MaxSize = Utils.Convert(new SizeF(width - margin * 2, height - margin * 2));
                    container.AvoidAsyncImagesLoading = true;
                    container.AvoidImagesLateLoading = true;

                    if (stylesheetLoad != null) {
                        container.StylesheetLoad += stylesheetLoad;
                    }
                    if (imageLoad != null) {
                        container.ImageLoad += imageLoad;
                    }

                    container.SetHtml(html, cssData);

                    using (var ig = new GraphicsAdapter(g)) {
                        container.PerformLayout(ig);
                        container.PerformPaint(ig);
                    }
                }

            }
            return image;
        }
    }
}