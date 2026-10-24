using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using DI.Chakra.Dto;
using System.Web.UI;
using Telerik.Web.UI;
using System.Collections.Generic;

namespace DI.Chakra.Web.Comp.Mitigation.UC
{
    public partial class ListMitigationUC : UIBaseUC, IAccessControl
    {
        private static List<SODMitigation> list;
        #region Helpers
        private void delete()
        {
            if (!ContextObjectId.HasValue || ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                return;
            }
            DomainApiClient<SODMitigation>.DeleteById(this.ContextObjectId.Value);
            RadMitigation.Rebind();
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "SOD Mitigation Deleted Successfully");
        }

        private void BindGrid()
        {
            list = DomainApiClient<SODMitigation>.GetAll();
            RadMitigation.DataSource = list;
        }
        #endregion

        #region Page and Control Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;

            if (!Page.IsPostBack)
            {
                RadMitigation.DataBind();
            }
        }

        protected void RadMitigation_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindGrid();
        }

        protected void RadMitigation_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem item = RadMitigation.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)item.GetDataKeyValue("Id");
        }
        #endregion

        #region Delegates
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                RadMitigation.ExportSettings.FileName = "Mitigation";
                RadMitigation.Export(RadMitigation, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RadMitigation.Export<List<SODMitigation>>(list, e.ActionType, "Mitigation");
            }
        }
        #endregion

        #region Initialise Section

        protected void RadMitigation_Init(object sender, EventArgs e)
        {

            GridFilterMenu menu = RadMitigation.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || menu.Items[i].Text == "EqualTo" ||
                    menu.Items[i].Text == "GreaterThan" || menu.Items[i].Text == "LessThan" || menu.Items[i].Text == "StartsWith" || 
                    menu.Items[i].Text == "EndsWith")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}