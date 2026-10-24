using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.CustomControl
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:IACMRadGridControl runat=server></{0}:IACMRadGridControl>")]
    public class IACMRadGridControl : RadGrid
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null)? "[" + this.ID + "]" : s);
            }
 
            set
            {
                ViewState["Text"] = value;
            }
        }
        public IACMRadGridControl() : base() { }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridLines = GridLines.None;
            AutoGenerateColumns = false;
            AllowMultiRowSelection = false;
            AllowFilteringByColumn = true;
            AllowSorting = true;
            this.Culture.DateTimeFormat.LongTimePattern = string.Empty;
            AllowPaging = true;
            AllowCustomPaging = true;
            PageSize = 10;
            PagerStyle.Mode = GridPagerMode.NextPrevAndNumeric;
            PagerStyle.AlwaysVisible = true;
            MasterTableView.AutoGenerateColumns = false;
            //ClientSettings.Scrolling.SaveScrollPosition = true;
            ClientSettings.Selecting.AllowRowSelect = true;
            ClientSettings.Scrolling.UseStaticHeaders = true;
            ClientSettings.Scrolling.AllowScroll = true;
            ClientSettings.Scrolling.EnableVirtualScrollPaging = true;
            ClientSettings.EnablePostBackOnRowClick = true;
            ClientSettings.Selecting.AllowRowSelect = true;
            MasterTableView.CommandItemSettings.ShowAddNewRecordButton = false;
            MasterTableView.CommandItemSettings.ShowExportToCsvButton = false;
            MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
            MasterTableView.CommandItemSettings.ShowExportToPdfButton = false;
            MasterTableView.CommandItemSettings.ShowRefreshButton = false;

            GridFilterMenu menu = this.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || menu.Items[i].Text == "EqualTo" ||
                    menu.Items[i].Text == "GreaterThan" || menu.Items[i].Text == "LessThan" || menu.Items[i].Text == "StartsWith" || menu.Items[i].Text == "EndsWith")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }
        public override GridTableView MasterTableView
        {
            get
            {
                return base.MasterTableView;
            }
        }
       
    }
}
