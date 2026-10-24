using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Function.UC
{
    /// <summary>
    /// Class responsible for listing all the Functions availabe in the database.
    /// Author : Harihara - 2/11/2015
    /// </summary>
    public partial class ListFunctionUC : UIBaseUC, IAccessControl
    {
        private static List<SODFunction> list;
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
                RadFunctions.DataBind();
            }
        }

        /// <summary>
        /// Mthod to supply data for RadFunctions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadFunctions_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindGrid();
        }

        /// <summary>
        /// Method to handle selection event for RadFunctions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadFunctions_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RadFunctions.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        /// <summary>
        /// Method to handle initialisation for RadFunctions grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadFunctions_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = RadFunctions.FilterMenu;
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
        /// Method to handle click event for btncancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btncancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        #endregion

        #region Helper
        /// <summary>
        /// Method to bind data to grid
        /// </summary>
        private void BindGrid()
        {
             list = DomainApiClient<SODFunction>.GetByAction<List<SODFunction>>(DomainApiAction.SODFunction.Function.
                GetAllIncludeSodFunctionItems, null);
            RadFunctions.DataSource = list;
               
        }

        /// <summary>
        /// Method to delete SOD Functions
        /// </summary>
        private void delete()
        {
            if (!ContextObjectId.HasValue || ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("functionId", (this.ContextObjectId).ToString());
            bool isUsedInConflict = DomainApiClient<SODConflictFunctions>.GetByAction<bool>
                            (DomainApiAction.SODFunction.SODConflictFunction.HasFunction, parameter);
            bool isUsedInException = DomainApiClient<SODException>.GetByAction<bool>
                            (DomainApiAction.SODException.Exception.HasFunction, parameter);

           // DomainApiClient<SODConflict>.

            if (isUsedInConflict || isUsedInException)
            {
                MasterPage.ShowAlert(AlertType.Error, "SOD Function could not be deleted because it is being referenced in Conflict Functions and/or Exception.");
                return;
            }
            var selectedFunction = DomainApiClient<SODFunction>.GetById(this.ContextObjectId.Value,true);
            foreach (SODFunctionItem item in selectedFunction.SodFunctionItems)
            {
                DomainApiClient<SODFunctionItem>.DeleteById(item.Id);
            }
            Dictionary<string, string> functionid = new Dictionary<string, string>();
            functionid.Add("functionId", this.ContextObjectId.ToString());
            DomainApiClient<SODConflict>.GetByAction<object>(DomainApiAction.SODFunction.SODConflict.DeleteConflictByFunction, functionid);
            DomainApiClient<SODFunction>.DeleteById(this.ContextObjectId.Value);
            RadFunctions.Rebind();
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "SOD Function Deleted Successfully");
        }
        #endregion

        #region Delegate
        /// <summary>
        /// Method to handle click event in the page
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
                RadFunctions.ExportSettings.FileName = "Function";
                RadFunctions.Export(RadFunctions, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RadFunctions.Export<List<SODFunction>>(list, e.ActionType, "Function");
            }
        }
        #endregion
    }
}