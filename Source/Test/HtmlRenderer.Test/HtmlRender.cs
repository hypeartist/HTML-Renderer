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
using System.Drawing.Imaging;
using System.Drawing.Text;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;


namespace HtmlRenderer.Test {
    /// <summary>
    /// Standalone static class for simple and direct HTML rendering.<br/>
    /// For WinForms UI prefer using HTML controls: <see cref="HtmlPanel"/> or <see cref="HtmlLabel"/>.<br/>
    /// For low-level control and performance consider using <see cref="HtmlContainer"/>.<br/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>GDI vs. GDI+ text rendering</b><br/>
    /// Windows supports two text rendering technologies: GDI and GDI+.<br/> 
    /// GDI is older, has better performance and looks better on standard monitors but doesn't support alpha channel for transparency.<br/> 
    /// GDI+ is newer, device independent so work better for printers but is slower and looks worse on monitors.<br/>
    /// HtmlRender supports both GDI and GDI+ text rendering to accommodate different needs, GDI+ text rendering methods have "GdiPlus" suffix
    /// in their name where GDI do not.<br/>
    /// </para>
    /// <para>
    /// <b>Rendering to image</b><br/>
    /// See https://htmlrenderer.codeplex.com/wikipage?title=Image%20generation <br/>
    /// Because of GDI text rendering issue with alpha channel clear type text rendering rendering to image requires special handling.<br/>
    /// <u>Solid color background -</u> generate an image where the background is filled with solid color and all the html is rendered on top
    /// of the background color, GDI text rendering will be used. (RenderToImage method where the first argument is html string)<br/>
    /// <u>Image background -</u> render html on top of existing image with whatever currently exist but it cannot have transparent pixels, 
    /// GDI text rendering will be used. (RenderToImage method where the first argument is Image object)<br/>
    /// <u>Transparent background -</u> render html to empty image using GDI+ text rendering, the generated image can be transparent.
    /// Text rendering can be controlled using <see cref="TextRenderingHint"/>, note that <see cref="TextRenderingHint.ClearTypeGridFit"/>
    /// doesn't render well on transparent background. (RenderToImageGdiPlus method)<br/>
    /// </para>
    /// <para>
    /// <b>Overwrite stylesheet resolution</b><br/>
    /// Exposed by optional "stylesheetLoad" delegate argument.<br/>
    /// Invoked when a stylesheet is about to be loaded by file path or URL in 'link' element.<br/>
    /// Allows to overwrite the loaded stylesheet by providing the stylesheet data manually, or different source (file or URL) to load from.<br/>
    /// Example: The stylesheet 'href' can be non-valid URI string that is interpreted in the overwrite delegate by custom logic to pre-loaded stylesheet object<br/>
    /// If no alternative data is provided the original source will be used.<br/>
    /// </para>
    /// <para>
    /// <b>Overwrite image resolution</b><br/>
    /// Exposed by optional "imageLoad" delegate argument.<br/>
    /// Invoked when an image is about to be loaded by file path, URL or inline data in 'img' element or background-image CSS style.<br/>
    /// Allows to overwrite the loaded image by providing the image object manually, or different source (file or URL) to load from.<br/>
    /// Example: image 'src' can be non-valid string that is interpreted in the overwrite delegate by custom logic to resource image object<br/>
    /// Example: image 'src' in the html is relative - the overwrite intercepts the load and provide full source URL to load the image from<br/>
    /// Example: image download requires authentication - the overwrite intercepts the load, downloads the image to disk using custom code and provide 
    /// file path to load the image from.<br/>
    /// If no alternative data is provided the original source will be used.<br/>
    /// Note: Cannot use asynchronous scheme overwrite scheme.<br/>
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// <b>Simple rendering</b><br/>
    /// HtmlRender.Render(g, "<![CDATA[<div>Hello <b>World</b></div>]]>");<br/>
    /// HtmlRender.Render(g, "<![CDATA[<div>Hello <b>World</b></div>]]>", 10, 10, 500, CssData.Parse("body {font-size: 20px}")");<br/>
    /// </para>
    /// <para>
    /// <b>Image rendering</b><br/>
    /// HtmlRender.RenderToImage("<![CDATA[<div>Hello <b>World</b></div>]]>", new Size(600,400));<br/>
    /// HtmlRender.RenderToImage("<![CDATA[<div>Hello <b>World</b></div>]]>", 600);<br/>
    /// HtmlRender.RenderToImage(existingImage, "<![CDATA[<div>Hello <b>World</b></div>]]>");<br/>
    /// </para>
    /// </example>
    public static class HtmlRender {



        /// <summary>
        /// Renders the specified HTML into a new image of the requested size.<br/>
        /// The HTML will be layout by the given size but will be clipped if cannot fit.<br/>
        /// <p>
        /// Limitation: The image cannot have transparent background, by default it will be white.<br/>
        /// See "Rendering to image" remarks section on <see cref="HtmlRender"/>.<br/>
        /// </p>
        /// </summary>
        /// <param name="html">HTML source to render</param>
        /// <param name="size">The size of the image to render into, layout html by width and clipped by height</param>
        /// <param name="backgroundColor">optional: the color to fill the image with (default - white)</param>
        /// <param name="cssData">optional: the style to use for html rendering (default - use W3 default style)</param>
        /// <param name="stylesheetLoad">optional: can be used to overwrite stylesheet resolution logic</param>
        /// <param name="imageLoad">optional: can be used to overwrite image resolution logic</param>
        /// <returns>the generated image of the html</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="backgroundColor"/> is <see cref="Color.Transparent"/></exception>.
        public static Image RenderToImage1(string html, Size size, Color backgroundColor = new Color(), CssData cssData = null,
            EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null) {
            if (backgroundColor == Color.Transparent)
                throw new ArgumentOutOfRangeException("backgroundColor", "Transparent background in not supported");

            // create the final image to render into
            var image = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);

            if (!string.IsNullOrEmpty(html)) {
                // create memory buffer from desktop handle that supports alpha channel
                IntPtr dib;
                var memoryHdc = Win32Utils.CreateMemoryHdc(IntPtr.Zero, image.Width, image.Height, out dib);
                try {
                    // create memory buffer graphics to use for HTML rendering
                    using (var memoryGraphics = Graphics.FromHdc(memoryHdc)) {
                        memoryGraphics.Clear(backgroundColor != Color.Empty ? backgroundColor : Color.White);

                        // render HTML into the memory buffer
                        RenderHtml(memoryGraphics, html, PointF.Empty, size, cssData, true, stylesheetLoad, imageLoad);
                    }

                    // copy from memory buffer to image
                    CopyBufferToImage(memoryHdc, image);
                } finally {
                    Win32Utils.ReleaseMemoryHdc(memoryHdc, dib);
                }
            }

            return image;
        }




        /// <summary>
        /// Renders the specified HTML into a new image of the requested size.<br/>
        /// The HTML will be layout by the given size but will be clipped if cannot fit.<br/>
        /// <p>
        /// Limitation: The image cannot have transparent background, by default it will be white.<br/>
        /// See "Rendering to image" remarks section on <see cref="HtmlRender"/>.<br/>
        /// </p>
        /// </summary>
        /// <param name="html">HTML source to render</param>
        /// <param name="size">The size of the image to render into, layout html by width and clipped by height</param>
        /// <param name="backgroundColor">optional: the color to fill the image with (default - white)</param>
        /// <returns>the generated image of the html</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="backgroundColor"/> is <see cref="Color.Transparent"/></exception>.
        public static Image RenderToImage2(string html, Size size, Color backgroundColor = new Color()) {
            if (backgroundColor == Color.Transparent)
                throw new ArgumentOutOfRangeException("backgroundColor", "Transparent background in not supported");

            CssData cssData = null;
            EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null;
            EventHandler<HtmlImageLoadEventArgs> imageLoad = null;

            var image = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            if (!string.IsNullOrEmpty(html)) {
                using (var g = Graphics.FromImage(image)) {
                    g.Clear(backgroundColor);
                    RenderHtml(g, html, PointF.Empty, size, cssData, true, stylesheetLoad, imageLoad);
                }
            }
            return image;

        }


        /// <summary>
        /// Renders the specified HTML into a new image of the requested size.<br/>
        /// The HTML will be layout by the given size but will be clipped if cannot fit.<br/>
        /// <p>
        /// Limitation: The image cannot have transparent background, by default it will be white.<br/>
        /// See "Rendering to image" remarks section on <see cref="HtmlRender"/>.<br/>
        /// </p>
        /// </summary>
        /// <param name="html">HTML source to render</param>
        /// <param name="size">The size of the image to render into, layout html by width and clipped by height</param>
        /// <param name="backgroundColor">optional: the color to fill the image with (default - white)</param>
        /// <returns>the generated image of the html</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="backgroundColor"/> is <see cref="Color.Transparent"/></exception>.
        public static Image RenderToImage3(string html, Size size, Color backgroundColor = new Color()) {
            if (backgroundColor == Color.Transparent)
                throw new ArgumentOutOfRangeException("backgroundColor", "Transparent background in not supported");

            CssData cssData = null;
            EventHandler<HtmlImageLoadEventArgs> imageLoad = null;

            var image = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            if (!string.IsNullOrEmpty(html)) {
                using (var g = Graphics.FromImage(image)) {
                    g.Clear(backgroundColor);

                    using (var container = new HtmlContainer()) {
                        container.Location = PointF.Empty;
                        container.MaxSize = size;
                        container.AvoidAsyncImagesLoading = true;
                        container.AvoidImagesLateLoading = true;
                        container.UseGdiPlusTextRendering = true;

                        if (imageLoad != null)
                            container.ImageLoad += imageLoad;

                        container.SetHtml(html, cssData);
                        container.PerformLayout(g);
                        container.PerformPaint(g);
                    }

                }
            }
            return image;

        }

        public static Image RenderToImage4(string html, int width, int height, int margin = 0, Color backgroundColor = new Color()) {
            if (string.IsNullOrWhiteSpace(html)) {
                return null;
            }

            if (backgroundColor == Color.Transparent) {
                throw new ArgumentOutOfRangeException(nameof(backgroundColor), "Transparent background in not supported");
            }


            // TODO: use same static css every time we render image to avoid css parsing overhead
            CssData cssData = null;
            EventHandler<HtmlImageLoadEventArgs> imageLoad = null;
            bool useGdiPlusTextRendering = true;

            var image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(image)) {
                g.Clear(backgroundColor);

                using (var container = new HtmlContainerInt(TestAdapter.Instance)) {

                    //container.SetMargins(0);
                    //container.PageSize = new TheArtOfDev.HtmlRenderer.Adapters.Entities.RSize(99999, 99999);
                    container.PageSize = new TheArtOfDev.HtmlRenderer.Adapters.Entities.RSize(width, height);

                    //container.Location = Utils.Convert(new PointF(94, 94));
                    container.Location = new TheArtOfDev.HtmlRenderer.Adapters.Entities.RPoint(margin, margin);
                    container.MaxSize = new TheArtOfDev.HtmlRenderer.Adapters.Entities.RSize(width - margin * 2, height - margin * 2);
                    container.AvoidAsyncImagesLoading = true;
                    container.AvoidImagesLateLoading = true;


                    // TODO: add custom image loading here
                    if (imageLoad != null) {
                        container.ImageLoad += imageLoad;
                    }

                    container.SetHtml(html, cssData);

                    using (var ig = new GraphicsAdapter(g, useGdiPlusTextRendering)) {
                        container.PerformLayout(ig);
                        container.PerformPaint(ig);
                    }
                }

            }
            return image;

        }




        #region Private methods


        /// <summary>
        /// Renders the specified HTML source on the specified location and max size restriction.<br/>
        /// If <paramref name="maxSize"/>.Width is zero the html will use all the required width, otherwise it will perform line 
        /// wrap as specified in the html<br/>
        /// If <paramref name="maxSize"/>.Height is zero the html will use all the required height, otherwise it will clip at the
        /// given max height not rendering the html below it.<br/>
        /// Returned is the actual width and height of the rendered html.<br/>
        /// </summary>
        /// <param name="g">Device to render with</param>
        /// <param name="html">HTML source to render</param>
        /// <param name="location">the top-left most location to start render the html at</param>
        /// <param name="maxSize">the max size of the rendered html (if height above zero it will be clipped)</param>
        /// <param name="cssData">optional: the style to use for html rendering (default - use W3 default style)</param>
        /// <param name="useGdiPlusTextRendering">true - use GDI+ text rendering, false - use GDI text rendering</param>
        /// <param name="stylesheetLoad">optional: can be used to overwrite stylesheet resolution logic</param>
        /// <param name="imageLoad">optional: can be used to overwrite image resolution logic</param>
        /// <returns>the actual size of the rendered html</returns>
        private static SizeF RenderHtml(Graphics g, string html, PointF location, SizeF maxSize, CssData cssData, bool useGdiPlusTextRendering, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad, EventHandler<HtmlImageLoadEventArgs> imageLoad) {
            SizeF actualSize = SizeF.Empty;

            if (!string.IsNullOrEmpty(html)) {
                using (var container = new HtmlContainer()) {
                    container.Location = location;
                    container.MaxSize = maxSize;
                    container.AvoidAsyncImagesLoading = true;
                    container.AvoidImagesLateLoading = true;
                    container.UseGdiPlusTextRendering = useGdiPlusTextRendering;

                    if (stylesheetLoad != null)
                        container.StylesheetLoad += stylesheetLoad;
                    if (imageLoad != null)
                        container.ImageLoad += imageLoad;

                    container.SetHtml(html, cssData);
                    container.PerformLayout(g);
                    container.PerformPaint(g);

                    actualSize = container.ActualSize;
                }
            }

            return actualSize;
        }


        /// <summary>
        /// Copy all the bitmap bits from memory bitmap buffer to the given image.
        /// </summary>
        /// <param name="memoryHdc">the source memory bitmap buffer to copy from</param>
        /// <param name="image">the destination bitmap image to copy to</param>
        private static void CopyBufferToImage(IntPtr memoryHdc, Image image) {
            using (var imageGraphics = Graphics.FromImage(image)) {
                var imgHdc = imageGraphics.GetHdc();
                Win32Utils.BitBlt(imgHdc, 0, 0, image.Width, image.Height, memoryHdc, 0, 0, Win32Utils.BitBltCopy);
                imageGraphics.ReleaseHdc(imgHdc);
            }
        }


        #endregion
    }
}