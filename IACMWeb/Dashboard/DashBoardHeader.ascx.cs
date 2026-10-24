using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Dashboard
{
    public partial class DashBoardHeader : DI.Chakra.Web.UIBaseUC
    {
        public bool IsChartType { get; set; }

        public string CssClass { get; set; }

        public string DashBoardTitle
        {
            get { return dashBoardTitle.InnerText; }
            set { dashBoardTitle.InnerText = value; }
        }
        public string DashBoardCaption
        {
            get { return dashboardCaption.InnerText; }
            set { dashboardCaption.InnerText = value; }
        }
        public string DashBoardRowCount
        {
            get { return dashBoardRowCount.InnerText; }
            set { dashBoardRowCount.InnerText = value; }
        }
        public string NavigateURL
        {
            set { navigateUrl.HRef = value; }
        }
        public string HeaderImageUrl
        {
            set { imgHeader.ImageUrl = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (IsChartType)
                {
                    imageSpan.Visible = true;
                    rowCountSpan.Visible = false;
                }
                else if (!string.IsNullOrEmpty(DashBoardRowCount) && DashBoardRowCount.Length > 0)
                {
                    imageSpan.Visible = false;
                    rowCountSpan.Visible = true;
                }
                else
                {
                    imageSpan.Visible = true;
                    rowCountSpan.Visible = false;

                }

                if(rowCountSpan.Visible && !string.IsNullOrEmpty(CssClass))
                {
                    rowCountSpan.Attributes["class"] = CssClass;
                }

                //dashboardCaption.InnerText = Caption;
                //dashBoardTitle.InnerText = Title;
            }
            //imgHeader.ImageUrl = HeaderImage;

        }
    }
}