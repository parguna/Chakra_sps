using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Exception.UC
{
    public partial class ListExceptionUC : UIBaseUC, IAccessControl
    {
        private static List<SODException> list;
        #region Helper
        /// <summary>
        /// Method to bind the data to Grid
        /// </summary>
        private void BindGrid()
        {
            list = DomainApiClient<SODException>.GetByAction<List<SODException>>
                (DomainApiAction.SODException.Exception.GetAllIncludingRiskOwner, null);
            RadException.DataSource = list;
        }

        /// <summary>
        /// Method to Delete the Exception items
        /// </summary>
        private void delete()
        {
            if (!ContextObjectId.HasValue || ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("exceptionId", (this.ContextObjectId).ToString());
            bool isUsed = DomainApiClient<SODPolicyExecutionDefinition>.GetByAction<bool>
                            (DomainApiAction.SODPolicyExecutionDefinition.ExecutionDefinition.HasException, parameter);
            if (isUsed)
            {
                MasterPage.ShowAlert(AlertType.Error, "SOD Exception could not be deleted because it is being referenced in Policy Execution.");
                return;
            }
            var selectedException = DomainApiClient<SODException>.GetById(this.ContextObjectId.Value, true);
            foreach (SODExceptionItem item in selectedException.ExceptionItems)
            {
                DomainApiClient<SODExceptionItem>.DeleteById(item.Id);
            }
            DomainApiClient<SODException>.DeleteById(this.ContextObjectId.Value);
            RadException.Rebind();
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "SOD Exception Deleted Successfully");
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Mehthod to Load the Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                RadException.DataBind();
            }
        }

        /// <summary>
        /// Event to Handle load the Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadException_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindGrid();
        }

        /// <summary>c
        /// Event to handle the Grid Selected Index changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadException_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem item = RadException.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)item.GetDataKeyValue("Id");
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
        /// Method to handle to event to  Add, Delete , Edit,View
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
                RadException.ExportSettings.FileName = "Exception";
                RadException.Export(RadException, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RadException.Export<List<SODException>>(list, e.ActionType, "Exception");
            }
        }
        #endregion

        #region Initialise Section
        /// <summary>
        /// Event to Initialize the Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadException_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = RadException.FilterMenu;
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