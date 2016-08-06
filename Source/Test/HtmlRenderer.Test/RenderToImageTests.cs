using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing.Imaging;
using System.Drawing;
using System.Diagnostics;
using HtmlRenderer.Web;

namespace HtmlRenderer.Test {
    [TestClass]
    public class RenderToImageTests {

        const string html = @"
<p>Folders are used to organize content in Spaces. It should be be possible to re-organize manually into a content hierarchy.</p>
<p><pre><code>public static void Main(string[] args){
    Console.WriteLine(""Hello World"");
}</code></pre></p>
<p>Tasks is a core feature and can be attached to everything:</p>
<ul>
<li>Individual <strong>items</strong> and <em>files</em>.</li>
<li><span style=""background-color: #ffff00;"">Apps</span></li>
<li><u>Spaces</u></li>
</ul>
<table style=""width:100%;"">
<tr>
<th>One</th>
<th>Two</th>
<th>Three</th>
<th>Four</th>
</tr>
<tr>
<td>One</td>
<td>Two</td>
<td>Three</td>
<td>Four</td>
</tr>
</table>
<p>We should have a couple of different views for tasks (list, agenda, calendar). We also need aggregated views so that you can see all tasks attached to an item, app, or entire space.</p>
<h2>UI/UX</h2>
<p>Flat, colorful, bold and graphic interface (Material Design). Replace boring font-awesome with multi-color icons.</p>
<img src=""https://d13yacurqjgara.cloudfront.net/users/452514/screenshots/1853419/education-flat-colored-icons.jpg"" alt="""" style=""max-width:100%"">
";


        [TestMethod]
        public void HtmlToImage() {
            var timer = new Stopwatch();
            var iter = 10;

            // warmup
            var image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage(html, new Size(794, 1123), Color.White);
            image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImageGdiPlus(html, new Size(794, 1123));
            image = HtmlRender.RenderToImage(html, 794, 1123, 94, Color.White);
            
            timer.Start();
            for (int i = 0; i < iter; i++) {
                image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage("<h1>WinForms.RenderToImage</h1>" + html, new Size(794, 1123), Color.White);
                image.Save(@"C:\Temp\WinForms.RenderToImage.png", ImageFormat.Png);
            }
            timer.Stop();
            var time1 = timer.ElapsedMilliseconds / iter;

            timer.Start();
            for (int i = 0; i < iter; i++) {
                image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImageGdiPlus("<h1>WinForms.RenderToImageGdiPlus</h1>" + html, new Size(794, 1123));
                image.Save(@"C:\Temp\WinForms.RenderToImageGdiPlus.png", ImageFormat.Png);
            }
            timer.Stop();
            var time2 = timer.ElapsedMilliseconds / iter;
         
            
            timer.Restart();
            for (int i = 0; i < iter; i++) {
                image = HtmlRenderer.Web.HtmlRender.RenderToImage("<h1>Web.RenderToImage</h1>" + html, 794, 1123, 94, Color.WhiteSmoke);
                image.Save(@"C:\Temp\Web.RenderToImage.png", ImageFormat.Png);
            }
            timer.Stop();
            var time3 = timer.ElapsedMilliseconds / iter;

            Assert.IsTrue(time2 < 100);
            Assert.IsTrue(time1 < time2);
            Assert.IsTrue(time3 < time1);
        }

    }
}
