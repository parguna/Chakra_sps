using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Conflict.Common
{
    /// <summary>
    /// Usercontrol responsible for capturing Function Details in Compliance Conflict Wizard.
    /// Author : Harihara - 16/10/2015.
    /// </summary>
    public partial class FucntionDetailsUC : DI.Chakra.Web.UIBaseUC
    {
        #region Class Member Variables
        private SODConflictFunctions conflictFunction = null;
        private const string V_SELECTEDFUNCTIONS = "SELECTEDFUNCTIONS";
        private UIMode uiMode;
        #endregion

        #region Public Properties
        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
        }

        public SODConflictFunctions SODConflictFunction
        {
            set { conflictFunction = value; }
            get { return conflictFunction; }
        }
        public Dictionary<long, string> SelectedFunctions
        {
            get
            {
                if (ViewState[V_SELECTEDFUNCTIONS] != null)
                {
                    return (Dictionary<long, string>)ViewState[V_SELECTEDFUNCTIONS];
                }
                return new Dictionary<long, string>();
            }
            set
            {
                ViewState[V_SELECTEDFUNCTIONS] = value;
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method to add functions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="functionName"></param>
        private void AddFunctions(long id, string functionName)
        {
            var functions = this.SelectedFunctions;
            if (!functions.ContainsKey(id))
            {
                functions.Add(id, functionName);
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Warning, "Function already exists");
                return;
            }
            this.SelectedFunctions = functions;
        }

        /// <summary>
        /// Method to bind data to grdFunctions
        /// </summary>
        private void BindFunctionGrid()
        {
            grdFunctions.DataSource = this.SelectedFunctions;
            grdFunctions.DataBind();
            functionSearch.Clear();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to set mode of display
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);  
        }
        #endregion

        #region Intialize Section
        /// <summary>
        /// Method to initialise page
        /// </summary>
        public void InitilizePage()
        {
            if (UIMode==UIMode.Edit||UIMode==UIMode.View)
            {
                if (SODConflictFunction != null && SODConflictFunction.Functions.Count > 0)
                {
                    foreach (SODFunction func in SODConflictFunction.Functions)
                    {
                        AddFunctions(func.Id, func.SodFunctionCode + " - " + func.SodFunctionDescription);
                    }
                }
            }
            BindFunctionGrid();
        }
        #endregion

        #region Control Events
        /// <summary>
        /// Method to handle click event for btnFunction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFunction_Click(object sender, EventArgs e)
        {
            SODFunction function = functionSearch.SelectedFunction;
            if (function != null)
            {
                AddFunctions(function.Id, function.SodFunctionCode + " - " + function.SodFunctionDescription);
            }
            BindFunctionGrid();
        }

        /// <summary>
        /// Method to handle itemcommand event for grdFunctions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdFunctions_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long key = (long)Convert.ChangeType(item.GetDataKeyValue("Key"), typeof(long));
                var fucntions = this.SelectedFunctions;
                fucntions.Remove(key);
                this.SelectedFunctions = fucntions;
                BindFunctionGrid();
            }
        }
        #endregion
    }
}