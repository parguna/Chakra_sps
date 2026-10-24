using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.Comp.Mitigation.Wizard
{
    public partial class MitigationWized : UIBaseUC
    {
        #region Class Member Variables 
        private SessionStatusImpl session = new SessionStatusImpl();
        SODMitigation mitigation = new SODMitigation();
        private const string V_DISPLAYMODE = "DISPLAYMODE";
        #endregion

        #region  Public properties
        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.Add;
                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set { ViewState[V_DISPLAYMODE] = value; }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Set the control values from the Mitigation objects
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="isViewMode"></param>
        /// <param name="mitigaiton"></param>
        private void SetPage(SODMitigation mitigaiton)
        {
            MitigatCode.Mitigaiton = mitigation;
            MitigOutput.SODMitigation = mitigaiton;
            MitigRiskCode.Mitigation = mitigaiton;
            MitigRiskCode.RiskCode = mitigation.RiskCodes != null ? mitigation.RiskCodes : new List<SODRiskCode>();
        }
        #endregion

        #region Page and Control Methods
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                IntializePage();
            if (IsPostBack)
            {
                string FileInfo = MitigatCode.FilePathValue;
                Session["FileInfo"] = FileInfo;
            }
        }

        /// <summary>
        /// Method to save the Mitigation object in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MitigationWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            try
            {
                SODMitigation mitigation = new SODMitigation();
                if ((UIMode)ViewState[V_DISPLAYMODE] != UIMode.View)
                {
                    MitigatCode.GetMitigCodeDetails(ref mitigation);                 
                    if (DoesMitigationCodeAlreadyExists(mitigation.MitigationCode))
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Mitigation code already exists");
                        return;
                    }
                    MitigRiskCode.GetFilledRiskCodes(ref mitigation);
                    if (mitigation.RiskCodes == null || mitigation.RiskCodes.Count == 0)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Please select at least one Risk Code.");
                        return;
                    }
                    MitigOutput.GetMitigOutput(ref mitigation);
                    if ((mitigation.MonitorUserId == null && mitigation.MonitorGroupId == null) || (mitigation.ApproverUserId == null
                        && mitigation.ApproverGroupId == null))
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Approver / Reveiwer should not be empty");
                        return;
                    }
                    if ((UIMode)ViewState[V_DISPLAYMODE] == UIMode.Edit)
                        mitigation.Id = Convert.ToInt64(this.ContextObjectId);

                    DomainApiClient<SODMitigation>.Save(mitigation);
                    MasterPage.ShowAlert(AlertType.Success, "Saved Successfully");

                    Response.Redirect("~//Comp/Mitigation/List.aspx");
                }
                else
                {
                    Response.Redirect("~//Comp/Mitigation/List.aspx");
                }
            }
            catch (System.Exception ex)
            {
            }
        }

        protected void MitigationWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            Response.Redirect("~/Comp/Mitigation/List.aspx");
        }

        private bool DoesMitigationCodeAlreadyExists(string mitigationCode)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("mitiCode", mitigationCode);
            return  DomainApiClient<SODMitigation>.GetByAction<bool>
                (DomainApiAction.SODMitigation.DoesExists, parameter);
        }

        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to initialize the page in Various modes
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="isViewMode"></param>
        public void IntializePage()
        {
            if (!IsPostBack)
            {
                if ((UIMode)ViewState[V_DISPLAYMODE] == UIMode.Edit || (UIMode)ViewState[V_DISPLAYMODE] == UIMode.View)
                {
                    long MitigationId = Convert.ToInt64(this.ContextObjectId);
                    //TODO: HARI, build broken
                    // We need to select the mitigation.
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", MitigationId.ToString());
                    mitigation = DomainApiClient<SODMitigation>.GetByAction<SODMitigation>
                        (DomainApiAction.SODMitigation.GetByIdIncludeSodRiskCode, parameter);
                }
                if ((UIMode)ViewState[V_DISPLAYMODE] == UIMode.Edit)
                {
                    SetPage(mitigation);
                }
                else if ((UIMode)ViewState[V_DISPLAYMODE] == UIMode.View)
                {
                    SetPage(mitigation);
                    MitigatCode.DisplayMode();
                    MitigOutput.DisplayMode();
                    MitigRiskCode.DisplayMode();
                }
                MitigatCode.UIMode = (UIMode)ViewState[V_DISPLAYMODE];
                MitigRiskCode.UIMode = (UIMode)ViewState[V_DISPLAYMODE];
                MitigOutput.UIMode = (UIMode)ViewState[V_DISPLAYMODE];
                MitigatCode.IntializePage();
                MitigOutput.IntializePage();
                MitigRiskCode.IntializePage();
            }
        }
        #endregion
    }
}