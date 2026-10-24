using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web;
using System;
using System.Web.UI;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using System.Collections.Generic;

namespace IACMWeb.Comp.Policy.UC
{
    public partial class ListCompliancePolicyUC : UIBaseUC, IAccessControl
    {
        private static List<CompliancePolicy> list;
        #region Page and Control Events
        /// <summary>
        /// Method to load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                grdCompanyPolicies.Rebind();
            }
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

        /// <summary>
        /// Method to hadle any click event in the page
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
                grdCompanyPolicies.ExportSettings.FileName = "Policy";
                grdCompanyPolicies.Export(grdCompanyPolicies, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdCompanyPolicies.Export<List<CompliancePolicy>>(list, e.ActionType, "Policy");
            }
        }

        /// <summary>
        /// Method to handle grid row selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdCompanyPolicies_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = grdCompanyPolicies.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        /// <summary>
        /// Method to Initialise grdCompanyPolicies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdCompanyPolicies_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = grdCompanyPolicies.FilterMenu;
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

        /// <summary>
        /// Method to supplyy data for grdCompanyPolicies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdCompanyPolicies_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindGrid();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method to bind data for grdCompanyPolicies
        /// </summary>
        private void BindGrid()
        {
            list = DomainApiClient<CompliancePolicy>.GetAll();
            grdCompanyPolicies.DataSource = list;
        }

        /// <summary>
        /// Method to delete Compliance Policy
        /// </summary>
        private void delete()
        {
            if (!ContextObjectId.HasValue || ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("policyId", (this.ContextObjectId).ToString());
            bool isUsed = DomainApiClient<SODPolicyExecutionDefinition>.GetByAction<bool>
                            (DomainApiAction.SODPolicyExecutionDefinition.ExecutionDefinition.HasPolicy, parameter);
            if (isUsed)
            {
                MasterPage.ShowAlert(AlertType.Error, 
                    "Compliance policy could not be deleted because it is being referenced in Policy Execution.");
                return;
            }
            bool isExists = DomainApiClient<CompliancePolicy>.GetById((long)this.ContextObjectId) != null ? true : false;
            if (isExists)
            {
                try {
                    DomainApiClient<CompliancePolicy>.DeleteById((long)this.ContextObjectId.Value);
                    this.ContextObjectId = null;
                    grdCompanyPolicies.Rebind();
                    MasterPage.ShowAlert(AlertType.Success, "Compliance policy deleted successfully");
                }
                catch(Exception ex)
                {
                    MasterPage.ShowAlert(AlertType.Error,
                                      "Compliance policy could not be deleted because it is being referenced in Conflicts.");
                    return;
                }
            }
            else
                grdCompanyPolicies.Rebind();
        }
        #endregion   
    }
}