using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.Comp.Function.Wizard
{
    /// <summary>
    /// Wizard Class for Defining Functions in Compliance Module.
    /// Author - Harihara - 3/11/2015
    /// </summary>
    public partial class FunctionWizardUC : UIBaseUC
    {
        #region Class Member Variables
        private SessionStatusImpl session = new SessionStatusImpl();
        private const string V_DISPLAYMODE = "DISPLAYMODE";
        SODFunction _Function = null;
        #endregion

        #region Intialize Section
        /// <summary>
        /// Method to initialise page
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="isViewMode"></param>
        /// <param name="isCloneMode"></param>
        public void InitilizePage()
        {
            if (UIMode == UIMode.Edit || UIMode == UIMode.Clone)
            {
                SetPage();
            }
            else if (UIMode == UIMode.View)
            {
                SetPage();
                FunctionDetailsUC.DisplayMode();
                EndPointSystemUC.DisplayMode();
            }
            FunctionDetailsUC.UIMode = UIMode;
            EndPointSystemUC.UIMode = UIMode;
            FunctionDetailsUC.InitilizePage();
            EndPointSystemUC.InitilizePage();
        }
        #endregion

        #region Public Properties
        public SODFunction SodFunction
        {
            set { _Function = value; }
            get { return _Function; }
        }

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

        #endregion

        #region Helpers
        /// <summary>
        /// Method to set page display properties
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="isViewMode"></param>
        private void SetPage()
        {
            long FunctionId = Convert.ToInt64(this.ContextObjectId);
            SODFunction function = GetFunction(FunctionId);
            FunctionDetailsUC.SodFunction = function;
            EndPointSystemUC.SodFunction = function;
        }

        /// <summary>
        /// Mehod to get function
        /// </summary>
        /// <param name="conflcitFunctionId"></param>
        /// <returns></returns>
        private SODFunction GetFunction(long conflcitFunctionId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", conflcitFunctionId.ToString());
            return DomainApiClient<SODFunction>.GetByAction<SODFunction>
                (DomainApiAction.SODFunction.Function.GetByIdIncludeSodFunctionItem, parameter);
        }
        #endregion

        #region Page Events and RadWizard Event
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                InitilizePage();
        }

        /// <summary>
        /// Method to handle finish button click for FunctionRadWizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FunctionRadWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            try
            {
                if (UIMode != UIMode.View)
                {
                    SODFunction sodfunction = FunctionDetailsUC.GetSodFunction();
                    if (string.IsNullOrEmpty(sodfunction.SodFunctionCode))
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Function Code should not be empty");
                        return;
                    }

                    sodfunction.SodFunctionItems = new List<SODFunctionItem>();
                    sodfunction.SodFunctionItems.Clear();
                    sodfunction.SodFunctionItems = EndPointSystemUC.GetSodFunctionItems();

                    if (sodfunction.SodFunctionItems.Count >0)
                    {
                        if (UIMode == UIMode.Clone || UIMode != UIMode.Edit)
                        {
                            if (UIMode == UIMode.Clone)
                                sodfunction.SodFunctionCode = sodfunction.SodFunctionCode;
                            sodfunction.Id = 0;
                            Dictionary<string, string> parameter = new Dictionary<string, string>();
                            parameter.Add("sodFunctionCode", sodfunction.SodFunctionCode);
                            bool isExists = DomainApiClient<SODFunction>.GetByAction<bool>
                                (DomainApiAction.SODFunction.Function.DoesExists, parameter);
                            if (isExists)
                            {
                                MasterPage.ShowAlert(AlertType.Warning, "Function Code already exists");
                                return;
                            }
                        }
                        else if (UIMode == UIMode.Edit)
                        {
                            sodfunction.Id = Convert.ToInt64(this.ContextObjectId);
                        }
                        DomainApiClient<SODFunction>.Save(sodfunction);
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please add at least one function detail");
                        return;
                    }
                }
                Response.Redirect("~//Comp/Function//List.aspx");
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }
        protected void FunctionRadWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {

            Response.Redirect("~/Comp/Function/List.aspx");
        }
        #endregion
    }
}