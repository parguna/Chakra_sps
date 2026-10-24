using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.CustomControl
{
    public class IACMGridCommandItemTemplate:System.Web.UI.ITemplate
    {
        System.Web.UI.HtmlControls.HtmlGenericControl dynEm;
         public IACMGridCommandItemTemplate()
        {
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            dynEm = new System.Web.UI.HtmlControls.HtmlGenericControl("em");
            dynEm.Attributes.Add("class", "fa fa-cog");
            dynEm.Attributes.Add("onclick", "ShowFirstColumnHeaderMenu(this,event);");
            container.Controls.Add(dynEm);

            var clearButton = new LinkButton();
            clearButton.ID = "btnGridFilterClear";
            clearButton.Text = "&nbsp;";
            clearButton.CommandName = "gridFilterClear";
            clearButton.Attributes.Add("class", "fa fa-filter");
            clearButton.ToolTip = "Clear Filter";
            container.Controls.Add(clearButton);
        }
    }

    public class radGridCommandItemTemplate : System.Web.UI.ITemplate
    {
        public radGridCommandItemTemplate()
        {
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            var clearButton = new LinkButton();
            clearButton.ID = "btnGridFilterClear";
            clearButton.Text = "&nbsp;";
            clearButton.CommandName = "gridFilterClear";
            clearButton.Attributes.Add("class", "fa fa-filter");
            clearButton.ToolTip = "Clear Filter";
            container.Controls.Add(clearButton);
        }
    }
}