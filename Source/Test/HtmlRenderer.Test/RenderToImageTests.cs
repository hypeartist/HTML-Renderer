using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing.Imaging;
using System.Drawing;

namespace HtmlRenderer.Test {
    [TestClass]
    public class RenderToImageTests {

        const string html = @"
<h2>Structure, Organization and Hierarchy</h2>
<p>Spaces automagically organizes items into apps. It should be be possible to&nbsp;re-organize manually into a content hierarchy.
You can discuss everything:</p>
<ul>
<li>Individual items and files (replaces comments). Likes, Follows etc can be posted in this discussion, e.g. ""@johan liked the document"".</li>
<li>Apps. Info can be posted in this discussion, e.g. ""@rickard created a document"".</li>
<li>Spaces. You can post messages directly into the space (replaces activity stream?).</li>
<li>Should we aggregate info from the apps and item discussions? Perhaps optional?</li>
<li>Chat rooms</li>
<li>Private discussions with other people</li>
</ul>
<p>Tasks is a core feature and can be attached to everything (create from and link to item where it was created)</p>
<ul>
<li>Individual items and files.</li>
<li><span style=""background-color: #ffff00;"">Apps</span></li>
<li>Spaces</li>
<li>Discussions</li>
</ul>
<p>We should have a couple of different views for tasks (list, agenda, calendar). We also need aggregated views so that you can see all tasks attached to an item, app, or entire space. There should also be a view for all your tasks regardless of space, app or item where it is attached.&nbsp;</p>
<h2>UI/UX</h2>
<p>Creating content should always be possible with the click of a button (1-click publish). Ask to save in a space. If you don't select a space it is created in your ""personal"" space?</p>
<p>We need a visible search form on all pages. In navbar?</p>
<p>Flat, colorful, bold and graphic interface (Material Design). Replace boring font-awesome with multi-color icons.</p>
<img src=""https://d13yacurqjgara.cloudfront.net/users/452514/screenshots/1853419/education-flat-colored-icons.jpg"" alt="""">
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
            var image = HtmlRender.RenderToImage(html, new Size(794, 1123), Color.White);
            image.Save(@"C:\Temp\html.png", ImageFormat.Png);
        }
    }
}
