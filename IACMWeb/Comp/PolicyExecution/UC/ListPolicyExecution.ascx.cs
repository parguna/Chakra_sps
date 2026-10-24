using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.PolicyExecution.UC
{
    public partial class ListPolicyExecution : UIBaseUC, IAccessControl
    {
        private static List<SODPolicyExecutionDefinition> list;
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
                RadPolicyExecution.DataBind();
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
        /// Method to supply data for RadPolicyExecution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadPolicyExecution_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindGrid();
        }

        /// <summary>
        /// Method to handle selection in grid RadPolicyExecution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadPolicyExecution_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RadPolicyExecution.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        /// <summary>
        /// Method to initialse grid RadPolicyExecution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadPolicyExecution_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = RadPolicyExecution.FilterMenu;
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
                RadPolicyExecution.ExportSettings.FileName = "PolicyExecution";
                RadPolicyExecution.Export(RadPolicyExecution, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RadPolicyExecution.Export<List<SODPolicyExecutionDefinition>>(list, e.ActionType, "PolicyExecution");
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method to bind data to grid RadPolicyExecution
        /// </summary>
        private void BindGrid()
        {
            List<SODPolicyExecutionDefinition> list = DomainApiClient<SODPolicyExecutionDefinition>.
                GetByAction<List<SODPolicyExecutionDefinition>>
                (DomainApiAction.SODPolicyExecutionDefinition.ExecutionDefinition.GetAllByIncludeCompliancePolicy, null);
            RadPolicyExecution.DataSource = list;
        }

        /// <summary>
        /// Method to perform deletion of Policy Executions
        /// </summary>
        private void delete()
        {
            if (!ContextObjectId.HasValue || ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                return;
            }

            bool isExists = DomainApiClient<SODPolicyExecutionDefinition>.GetById((long)this.ContextObjectId) != null ? true : false;
            if (isExists)
            {
                try {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("sodPolicyExecutionId", this.ContextObjectId.ToString());
                    DomainApiClient<SODPolicyExecutionDefinition>.GetByAction<object>(DomainApiAction.SODPolicyExecutionDefinition.ExecutionDefinition.SODPolicyDeleteById, parameter);
                    // DomainApiClient<SODPolicyExecutionDefinition>.DeleteById((long)this.ContextObjectId.Value);

                    RadPolicyExecution.Rebind();
                    this.ContextObjectId = null;
                    MasterPage.ShowAlert(AlertType.Success, "SOD Policy Execution Deleted Successfully");
                }
                catch(System.Exception ex)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Policy Execution is  mapped with Policy");
                }
            }
            else
                RadPolicyExecution.Rebind();
        }
        #endregion
    }
}