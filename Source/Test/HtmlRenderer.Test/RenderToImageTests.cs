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
<p>Test, TEST, test!</p>
<h2>Structure, Organization and Hierarchy</h2>
<p>Spaces automagically organizes items into apps. It should be be possible to&nbsp;re-organize manually into a content hierarchy.
You can discuss everything.</p>
<p>Tasks is a core feature and can be attached to everything (create from and link to item where it was created)</p>
<ul>
<li>Individual <strong>items</strong> and <em>files</em>.</li>
<li><span style=""background-color: #ffff00;"">Apps</span></li>
<li><u>Spaces</u></li>
<li>Discussions</li>
</ul>
<p>We should have a couple of different views for tasks (list, agenda, calendar). We also need aggregated views so that you can see all tasks attached to an item, app, or entire space. There should also be a view for all your tasks regardless of space, app or item where it is attached.&nbsp;</p>
<h2>UI/UX</h2>
<p>Creating content should always be possible with the click of a button (1-click publish). Ask to save in a space. If you don't select a space it is created in your ""personal"" space?</p>
<p>We need a visible search form on all pages. In navbar?</p>
<p>Flat, colorful, bold and graphic interface (Material Design). Replace boring font-awesome with multi-color icons.</p>
<img src=""https://d13yacurqjgara.cloudfront.net/users/452514/screenshots/1853419/education-flat-colored-icons.jpg"" alt="""" style=""max-width:100%"">
<h2>Document collaboration</h2>
<ul>
<li>Full screen editor</li>
<li>Should look like a page in both edit and view mode</li>
<li>Inline editing instead of separate edit view</li>
<li>Toolbar always visible (fixed and/or floating)</li>
<li>Tasks and discussions (comments) always visible in sidebar (realtime updates)</li>
<li>See who is watching/editing the page</li>
<li>Better revision history with diff view</li>
<li>Inline comments?</li>
<li>Autocomplete for @mentions, #hashtags and :emoji:</li>
<li>Multiple people editing simultaneously?</li>
<li>Save to Word, PDF etc.</li>
<li>Collaborative editing? Or at least see live edits?</li>
<li>Lock vs optimistic concurrency?</li>
<li>Organize documents in folders</li>
<li>View document thumbnails or list view</li>
<li>Markdown support</li>
</ul>";


        [TestMethod]
        public void HtmlToImage() {
            var timer = new Stopwatch();
            var iter = 10;

            // warmup
            var image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage("<h1>HtmlRenderer.WinForms</h1>" + html, new Size(794, 1123), Color.White);
            image = HtmlRender.RenderToImage("<h1>HtmlRenderer.Web</h1>" + html, 794, 1123, 94, Color.White);

            timer.Start();
            for (int i = 0; i < iter; i++) {
                image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage("<h1>HtmlRenderer.WinForms</h1>" + html, new Size(794, 1123), Color.White);
                image.Save(@"C:\Temp\html1.png", ImageFormat.Png);
            }
            timer.Stop();
            var time1 = timer.ElapsedMilliseconds / iter;
         
            
            timer.Restart();
            for (int i = 0; i < iter; i++) {
                image = HtmlRender.RenderToImage("<h1>HtmlRenderer.Web</h1>" + html, 794, 1123, 94, Color.White);
                image.Save(@"C:\Temp\html2.png", ImageFormat.Png);
            }
            timer.Stop();
            var time2 = timer.ElapsedMilliseconds / iter;

            Assert.IsTrue(time1 < 60);
            Assert.IsTrue(time2 < 60);
        }

    }
}
