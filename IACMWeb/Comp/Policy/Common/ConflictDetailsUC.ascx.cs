using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using DI.Chakra.Web;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace IACMWeb.Comp.Policy.Common
{
    public partial class ConflictDetailsUC : UIBaseUC
    {
        #region Class Variables
        SessionStatusImpl session = new SessionStatusImpl();
        private CompliancePolicy policy = new CompliancePolicy();
        private const string V_SELECTEDCONFLICTS = "SELECTEDCONFLICTS";
        private const string V_CompliancePolicy = "ComplaincePolicy";
        private UIMode uiMode;
        #endregion

        #region Public Properties
        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
        }
        public CompliancePolicy CompliancePolicy
        {
            get
            {
                if (ViewState[V_CompliancePolicy] == null)
                    return null;
                return (CompliancePolicy)ViewState[V_CompliancePolicy];
            }
            set
            {
                ViewState[V_CompliancePolicy] = value;
            }
        }
        public Dictionary<long, string> SelectedConflicts
        {
            get
            {
                if (ViewState[V_SELECTEDCONFLICTS] != null)
                {
                    return (Dictionary<long, string>)ViewState[V_SELECTEDCONFLICTS];
                }
                return new Dictionary<long, string>();
            }
            set
            {
                ViewState[V_SELECTEDCONFLICTS] = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to set the mode of display
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }
        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to initialise page
        /// </summary>
        public void InitilizePage()
        {
            if (UIMode == UIMode.Edit || UIMode == UIMode.View)
            {
                if (CompliancePolicy != null && CompliancePolicy.Conflicts.Count > 0)
                {
                    foreach (SODConflictFunctions function in CompliancePolicy.Conflicts)
                    {
                        AddConflicts(function.Id, function.SodConflictCode + " - " + function.SodConflictDescription);
                    }
                }
            }
            BindConflictsGrid();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method to bind data to grdConflicts
        /// </summary>
        private void BindConflictsGrid()
        {
            grdConflicts.DataSource = this.SelectedConflicts;
            grdConflicts.DataBind();
            SODConflictFunctionSearch.Clear();
        }

        /// <summary>
        /// Method to add conflicts
        /// </summary>
        /// <param name="id"></param>
        /// <param name="policyName"></param>
        private void AddConflicts(long id, string policyName)
        {
            var conflicts = this.SelectedConflicts;
            if (!conflicts.ContainsKey(id))
            {
                conflicts.Add(id, policyName);
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Warning, "Conflict function already exists");
            }
            this.SelectedConflicts = conflicts;
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to Load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Method to handle click event in btnConflict 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConflict_Click(object sender, EventArgs e)
        {
            SODConflictFunctions function = SODConflictFunctionSearch.SelectedFunction;
            if (function != null)
            {
                AddConflicts(function.Id, function.SodConflictCode + " - " + function.SodConflictDescription);
            }
            BindConflictsGrid();
        }

        /// <summary>
        /// Method to handle item changed event in grdConflicts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdConflicts_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long key = (long)Convert.ChangeType(item.GetDataKeyValue("Key"), typeof(long));
                var conflicts = this.SelectedConflicts;
                conflicts.Remove(key);
                this.SelectedConflicts = conflicts;
                BindConflictsGrid();
            }
        }
        #endregion
    }
}