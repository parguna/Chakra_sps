using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Conflict.UC
{
    public partial class ListConflictsUC : UIBaseUC, IAccessControl
    {
        private static List<SODConflictFunctions> list;

        #region Page and Control Events
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                RadConflicts.DataBind();
            }
        }

        /// <summary>
        /// Method to handle selection event for RadConflicts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadConflicts_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RadConflicts.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }
        /// <summary>
        /// Method to supply data source for RadConflicts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadConflicts_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindGrid();
        }

        /// <summary>
        /// Method to handle click event for btncancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btncancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Method to handle any click event in the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                RadConflicts.ExportSettings.FileName = "Conflict";
                RadConflicts.Export(RadConflicts, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RadConflicts.Export<List<SODConflictFunctions>>(list, e.ActionType, "Conflict");
            }

        }
        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to initialise RadConflicts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadConflicts_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = RadConflicts.FilterMenu;
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

        #region Helper
        /// <summary>
        /// Method to bind data for RadConflicts
        /// </summary>
        private void BindGrid()
        {
            list = DomainApiClient<Dto.SODConflictFunctions>.GetByAction<List<SODConflictFunctions>>
                   (DomainApiAction.SODFunction.SODConflictFunction.GetAllByIncludeSodRiskCode, null);
            RadConflicts.DataSource = list;
        }

        /// <summary>
        /// Method to delete SODConflictFunction
        /// </summary>
        private void delete()
        {
            if (!ContextObjectId.HasValue || ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("conflictFunctionId", (this.ContextObjectId).ToString());
            bool isUsed = DomainApiClient<CompliancePolicy>.GetByAction<bool>
                            (DomainApiAction.Complaince.CompliancePolicy.Policy.HasConflictFunction, parameter);
            if (isUsed)
            {
                MasterPage.ShowAlert(AlertType.Error, "SOD Conflict Function could not be deleted because it is being referenced in Policy.");
                return;
            }
            DomainApiClient<SODConflictFunctions>.DeleteById(this.ContextObjectId.Value);
            RadConflicts.Rebind();
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "SOD Conflict Function Deleted Successfully");
        }
        #endregion
    }
}