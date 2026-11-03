using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.CustomControl
{
    /// <summary>
    /// Replaces asp:ImageButton site-wide (via tagMapping in Web.config).
    /// Many screens draw ImageButton icons purely with a CSS background and leave ImageUrl empty,
    /// which renders &lt;input type="image" src=""&gt;. Modern browsers treat that as a broken image
    /// and do not paint the CSS background, so the icon disappears. When no image is set, render a
    /// transparent 1x1 GIF instead so the CSS background shows through.
    /// </summary>
    public class SafeImageButton : ImageButton
    {
        private const string TransparentPixel = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            // Leave alone buttons that have an image, either via ImageUrl or a raw src="" attribute in markup.
            if (!string.IsNullOrEmpty(ImageUrl) || Attributes["src"] != null)
            {
                base.AddAttributesToRender(writer);
                return;
            }

            // Set only for rendering; view state is already saved, so this is not persisted.
            ImageUrl = TransparentPixel;
            try
            {
                base.AddAttributesToRender(writer);
            }
            finally
            {
                ImageUrl = string.Empty;
            }
        }
    }
}
