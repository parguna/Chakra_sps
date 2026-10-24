using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Conflict.Common
{
    public partial class ConflictPairsUC : UIBaseUC
    {
        #region Class Member Variables
        private SessionStatusImpl session = new SessionStatusImpl();
        private const string V_SELECTEDFUNCTIONS = "SELECTEDFUNCTIONS";
        private bool isEdit = false;
        private bool isView = false;
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        /// <summary>
        /// Method to handle ItemCommand event for grdConflcitFunctions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdConflcitFunctions_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long key = (long)Convert.ChangeType(item.GetDataKeyValue("Key"), typeof(long));
                var fucntions = this.SelectedFunctions;
                fucntions.Remove(key);
                this.SelectedFunctions = fucntions;
                BindConflictFunctionGrid();
            }
        }

        /// <summary>
        /// Method to handle click event for btnConflictFunction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConflictFunction_Click(object sender, EventArgs e)
        {
            SODConflictFunctions function = SODConflictFunctionSearch.SelectedFunction;
            if (function != null)
            {
                AddFunctions(function.Id, function.SodConflictCode + " - " + function.SodConflictDescription);
            }
            BindConflictFunctionGrid();
        }

        /// <summary>
        /// Method to handle any click event in the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
            }
        }
        #endregion

        #region Public Properties
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

        #region Helper
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
            }
            this.SelectedFunctions = functions;
        }

        /// <summary>
        /// Method to bind data to grdConflcitFunctions
        /// </summary>
        private void BindConflictFunctionGrid()
        {
            grdConflcitFunctions.DataSource = this.SelectedFunctions;
            grdConflcitFunctions.DataBind();
            SODConflictFunctionSearch.Clear();
        }

        /// <summary>
        /// Method to save conflict functions
        /// </summary>
        private void Save()
        {
            foreach (GridDataItem item in grdConflcitFunctions.Items)
            {
                long id = (long)item.GetDataKeyValue("Key");
                GenerateConflicts(id);
            }
            Response.Redirect("~//Comp/Conflict//List.aspx");
        }

        /// <summary>
        /// Method to generate conflict functions
        /// </summary>
        /// <param name="Id"></param>
        private void GenerateConflicts(long Id)
        {
            SODConflictFunctions conflictfunction = DomainApiClient<SODConflictFunctions>.GetById(Id,true);
            conflictfunction.LastGeneratedDate = DateTime.Now;
            DomainApiClient<SODConflictFunctions>.Save(conflictfunction);

            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("sodConflictCode", conflictfunction.SodConflictCode);
            DomainApiClient<SODConflict>.GetByAction<object>
                (DomainApiAction.SODFunction.SODConflict.DeleteBySodConflictCode, parameter);

            for (int i = 0; i < conflictfunction.Functions.Count; i++)
            {
                for (int j = i + 1; j < conflictfunction.Functions.Count; j++)
                {
                    bool Matched = true;
                    Dictionary<String, string> function1 = new Dictionary<String, string>();
                    function1.Add("id", conflictfunction.Functions[i].Id.ToString());
                    SODFunction f1 = DomainApiClient<SODFunction>.GetByAction<SODFunction>(DomainApiAction.SODFunction.Function.GetByIdIncludeSodFunctionItem, function1);
                    Dictionary<String, string> function2 = new Dictionary<String, string>();
                    function2.Add("id", conflictfunction.Functions[j].Id.ToString());
                    SODFunction f2 = DomainApiClient<SODFunction>.GetByAction<SODFunction>(DomainApiAction.SODFunction.Function.GetByIdIncludeSodFunctionItem, function2);

                    foreach (SODFunctionItem chkItem1 in f1.SodFunctionItems)
                    {
                        foreach (SODFunctionItem chkItem2 in f2.SodFunctionItems)
                        {
                            if (chkItem1.SodRoleDesc != chkItem2.SodRoleDesc || chkItem1.Endpointsystemid != chkItem2.Endpointsystemid)
                            {
                                Matched = false;
                            }
                        }
                    }

                    if (!Matched)
                    {
                        SODConflict sodconflict = new SODConflict();
                        sodconflict.SODConflictCode = conflictfunction.SodConflictCode;
                        sodconflict.SODConflictDescription = conflictfunction.SodConflictDescription;
                        sodconflict.FunctionId1 = f1.Id;
                        sodconflict.FunctionId2 = f2.Id;
                        sodconflict.RiskCodeId = conflictfunction.RiskCodeId;
                        DomainApiClient<SODConflict>.Save(sodconflict);
                    }
                }
            }
        }
        #endregion
    }
}