using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;

namespace DI.Chakra.Web.Comp.Conflict.Common
{
    /// <summary>
    /// User Control responsible for capturing Conflcit Details in Conflict Management Wizard.
    /// Author : Harihara - 11/10/2015.
    /// </summary>
    public partial class ConflictDetailsUC : DI.Chakra.Web.UIBaseUC
    {
        #region Class Member Variables
        SessionStatusImpl session = new SessionStatusImpl();
        private const string V_ID = "ENTITY_ID";
        private const string V_SODConflictFunction = "SODConflictFunction";
        #endregion

        #region Public Properties     
        private UIMode uiMode;

        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
        }

        public SODConflictFunctions SODConflictFunction
        {
            get
            {
                if (ViewState[V_SODConflictFunction] == null)
                    return null;
                return (SODConflictFunctions)ViewState[V_SODConflictFunction];
            }
            set
            {
                ViewState[V_SODConflictFunction] = value;
            }
        }
        #endregion

        #region Intialize Section
        /// <summary>
        /// Method to initialise page
        /// </summary>
        public void InitilizePage()
        {
            BindRiskCodes();
            BindComplianceType();
            rblActive.Checked = true;
            if (UIMode==UIMode.Edit||UIMode==UIMode.View)
            {
                txtConflictCode.Text = SODConflictFunction.SodConflictCode;
                txtConflictDescription.Text = SODConflictFunction.SodConflictDescription;
                ddlComplianceType.SelectedValue = SODConflictFunction.CompliancePolicyType;
                if (SODConflictFunction.IsActive.Value)
                {
                    rblActive.Checked = true;
                }
                else
                {
                    rblInActive.Checked = true;
                }
                AutRskCode.SetValue(SODConflictFunction.SodRiskCode.RiskCode + "-" + SODConflictFunction.SodRiskCode.RiskDescription, 
                    SODConflictFunction.RiskCodeId.ToString());
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

        /// <summary>
        /// Mehtod to get user inputs as SODConflictFunction
        /// </summary>
        /// <returns></returns>
        public SODConflictFunctions GetSODConflictFunction()
        {
            
            SODConflictFunctions function = new SODConflictFunctions();
            function.IsActive = rblActive.Checked;
            if(AutRskCode.GetValue() != null)
                  function.RiskCodeId = Convert.ToInt64(AutRskCode.GetValue().Value);
            function.CompliancePolicyType = ddlComplianceType.SelectedValue;
            function.SodConflictCode = txtConflictCode.Text;
            function.SodConflictDescription = txtConflictDescription.Text;
            return function;
        }
        #endregion

        #region Helper
        /// <summary>
        /// Method to bind data to ddlRiskCode
        /// </summary>
        private void BindRiskCodes()
        {            
            AutRskCode.ServiceMethodName = "GetAllRiskCodes";
            AutRskCode.ServicePath = "../../Services/AutoCompleteDataService.svc";
        }

        /// <summary>
        /// Method to bind compliance type dropdown
        /// </summary>
        private void BindComplianceType()
        {
            ddlComplianceType.DataSource = DomainApiClient<ComplianceType>.GetAll();
            ddlComplianceType.DataTextField = "PolicyTypeName";
            ddlComplianceType.DataValueField = "PolicyTypeName";
            ddlComplianceType.DataBind();
        }
        #endregion
    }
}