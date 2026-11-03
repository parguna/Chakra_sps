using System;
using Telerik.Web.UI;

namespace DI.Chakra.Web.CustomControl
{
    /// <summary>
    /// Replaces telerik:RadGrid site-wide (via tagMapping in Web.config).
    /// Embedded skins are disabled (skin.config), so with no ImagesPath Telerik resolves default
    /// command images (Delete.gif, Edit.gif, ...) relative to each page/control folder, where they
    /// don't exist. Point them at the IACMSkin grid images instead.
    /// </summary>
    public class SafeRadGrid : RadGrid
    {
        public const string DefaultImagesPath = "~/IACMSkin/Grid/";

        public static void ApplyDefaultImagesPath(RadGrid grid)
        {
            if (string.IsNullOrEmpty(grid.ImagesPath))
                grid.ImagesPath = DefaultImagesPath;
        }

        protected override void OnInit(EventArgs e)
        {
            ApplyDefaultImagesPath(this);
            base.OnInit(e);
        }
    }
}
