using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.Comp.Conflict.Wizard
{
    public partial class ConflictWizardUC : UIBaseUC
    {
        #region Class Member Variables
        private SessionStatusImpl session = new SessionStatusImpl();      
        SODConflictFunctions _Function = null;
        private const string V_DISPLAYMODE = "DISPLAYMODE";
        #endregion

        #region Public Properties
        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.View;
                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set { ViewState[V_DISPLAYMODE] = value; }
        }
     
        public SODConflictFunctions ConflictFunction
        {
            set { _Function = value; }
            get { return _Function; }
        }

        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to initialise page
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="isViewMode"></param>
        public void InitializePage()
        {
            if (UIMode == UIMode.Edit)
            {
                SetPage();
            }
            else if (UIMode == UIMode.View)
            {
                SetPage();
                conflictDetails.DisplayMode();
                functionDetails.DisplayMode();
            }
            conflictDetails.UIMode = UIMode;
            functionDetails.UIMode = UIMode;

            conflictDetails.InitilizePage();
            functionDetails.InitilizePage();
        }
        #endregion

        #region Helper
        /// <summary>
        /// Method to saave the conflict function details
        /// </summary>
        /// <param name="conflictfunction"></param>
        private void SaveDetails(SODConflictFunctions conflictfunction)
        {
            conflictfunction.Functions = new List<SODFunction>();

            foreach (var entry in functionDetails.SelectedFunctions)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", entry.Key.ToString());
                SODFunction function = DomainApiClient<Dto.SODFunction>.GetByAction<SODFunction>
                    (DomainApiAction.SODFunction.Function.GetByIdIncludeSodFunctionItem, parameter);
                if (function != null)
                {
                    conflictfunction.Functions.Add(function);
                }
            }
            if (UIMode == UIMode.Edit)
            {
                conflictfunction.Id = Convert.ToInt64(this.ContextObjectId);
                SODConflictFunctions dbfunction = DomainApiClient<SODConflictFunctions>.GetById(this.ContextObjectId.Value);
                if (dbfunction.SodConflictCode != conflictfunction.SodConflictCode)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("sodConflictCode", conflictfunction.SodConflictCode);
                    bool isExists = DomainApiClient<Dto.SODConflictFunctions>.GetByAction<bool>
                        (DomainApiAction.SODFunction.SODConflictFunction.DoesExists, parameter);
                    if (isExists)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Conflict Function Code already exists");
                        return;
                    }
                }
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("sodConflictCode", conflictfunction.SodConflictCode);          
                bool isExists = DomainApiClient<Dto.SODConflictFunctions>.GetByAction<bool>
                    (DomainApiAction.SODFunction.SODConflictFunction.DoesExists, parameter);
              if (isExists)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Conflict Function Code already exists");
                    return;
                }
            }
            DomainApiClient<Dto.SODConflictFunctions>.Save(conflictfunction);
            Response.Redirect("~//Comp/Conflict//List.aspx");
        }

        /// <summary>
        /// Method to set display properties for the page
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="isViewMode"></param>
        private void SetPage()
        {
            long conflcitFunctionId = Convert.ToInt64(this.ContextObjectId);
            SODConflictFunctions function = GetSODConflictFunction(conflcitFunctionId);
            conflictDetails.SODConflictFunction = function;
            functionDetails.SODConflictFunction = function;
        }

        /// <summary>
        /// Method to get SODConflictFunction
        /// </summary>
        /// <param name="conflcitFunctionId"></param>
        /// <returns></returns>
        private SODConflictFunctions GetSODConflictFunction(long conflcitFunctionId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", conflcitFunctionId.ToString());
            return DomainApiClient<Dto.SODConflictFunctions>.GetByAction<SODConflictFunctions>
                (DomainApiAction.SODFunction.SODConflictFunction.GetByIdIncludingSODRiskCode, parameter);
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                InitializePage();
        }

        /// <summary>
        /// Method to handle finishbuttonclick event for ConflictRadWizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ConflictRadWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            try
            {
                if (UIMode != UIMode.View)
                {
                    SODConflictFunctions conflictfunction = conflictDetails.GetSODConflictFunction();
                    if (string.IsNullOrEmpty(conflictfunction.SodConflictCode))
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Conflict Function Code should not be empty");
                        return;
                    }
                    if (conflictfunction.RiskCodeId == 0)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Please select risk code.");
                        return;
                    }
                    if (conflictfunction.CompliancePolicyType == "Sensitive Access") // Senstive Type
                    {
                        if (functionDetails.SelectedFunctions.Count == 1)
                        {
                            SaveDetails(conflictfunction);
                        }
                        else if (functionDetails.SelectedFunctions.Count > 1)
                        {
                            MasterPage.ShowAlert(AlertType.Error, "Please select only 1 function");
                            return;
                        }
                        else if (functionDetails.SelectedFunctions.Count == 0)
                        {
                            MasterPage.ShowAlert(AlertType.Error, "Please select at least 1 function");
                            return;
                        }
                    }
                    else
                    {
                        if (functionDetails.SelectedFunctions.Count > 1)
                        {
                            SaveDetails(conflictfunction);
                        }
                        else
                        {
                            MasterPage.ShowAlert(AlertType.Error, "Please select at least 2 functions");
                            return;
                        }
                    }
                }
               // Response.Redirect("~//Comp/Conflict//List.aspx");
            }
            catch (System.Exception ex)
            {

            }
        }
        protected void ConflictRadWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {

            Response.Redirect("~/Comp/Conflict/List.aspx");
        }
        #endregion
    }
}