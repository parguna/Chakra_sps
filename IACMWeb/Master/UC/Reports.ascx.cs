using Application;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Master.UC
{
    public partial class Reports : DI.Chakra.Web.UIBaseUC
    {
        #region "Members"
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();     
        List<Dto.TabMenu> menuItems = new List<Dto.TabMenu>();
        #endregion

        #region "Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindMenu();
        }

        protected void AddReport_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                dynamic listitem = e.Item.DataItem as dynamic;
                long parentMenuID = listitem.Id;

                List<Dto.TabMenu> lstSubReports = menuItems.Where(it => it.ParentTabMenuId == parentMenuID).ToList();
                Repeater childRepeater = (Repeater)e.Item.FindControl("SubReports");
                if (lstSubReports != null && lstSubReports.Count > 0)
                {
                    childRepeater.DataSource = menuItems.Where(it => it.ParentTabMenuId == parentMenuID).ToList();
                    childRepeater.DataBind();
                }
                else
                {
                    childRepeater.DataSource = EmptyTabMenu();
                    childRepeater.DataBind();
                }
            }
        }

        protected void SubReports_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                dynamic listitem = e.Item.DataItem as dynamic;
                long parentMenuID = listitem.Id;
                Repeater childRepeater = (Repeater)e.Item.FindControl("inLineReports");

                List<Dto.TabMenu> inLineReports = menuItems.Where(it => it.ParentTabMenuId == parentMenuID).ToList();
                childRepeater.DataSource = menuItems.Where(it => it.ParentTabMenuId == parentMenuID).ToList();
                childRepeater.DataBind();
            }
        }
        #endregion

        #region "Methods"
        private void BindMenu()
        {
            if (sessionStatus.User != null)
            {
                menuItems = (List<Dto.TabMenu>)Session["ReportMenus"];
                if ((menuItems == null) || (menuItems != null && menuItems.Count == 0))
                    mReport.Text = string.Empty;
                else
                {
                    mReport.Text = "Report";

                    long reportID = (from TM in menuItems
                                     where TM.UniqueName == "REPORT"
                                     select TM.Id).FirstOrDefault();

                    Report.DataSource = menuItems.Where(s => s.ParentTabMenuId == reportID).ToList();
                    Report.DataBind();

                    AddReport.DataSource = menuItems.Where(s => s.ParentTabMenuId == reportID).ToList();
                    AddReport.DataBind();
                }
            }
        }

        private List<Dto.TabMenu> EmptyTabMenu()
        {
            List<Dto.TabMenu> emptyList = new List<Dto.TabMenu>();
            Dto.TabMenu menu = new Dto.TabMenu();
            menu.Id = 0;
            menu.MenuName = "";
            emptyList.Add(menu);
            return emptyList;
        }
        #endregion

        protected void mReport_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> reportparameter = new Dictionary<string, string>();
            reportparameter.Add("configKey", "ReportPageUrl");
            reportparameter.Add("product", Product.IACM.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, reportparameter);
            string reportPageUrl = config.ConfigValue.ToString();
            Response.Redirect(reportPageUrl);
            //mReport. = reportPageUrl;
        }
    }
}