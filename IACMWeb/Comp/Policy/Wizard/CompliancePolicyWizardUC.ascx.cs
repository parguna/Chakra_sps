using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using DI.Chakra.Web;
using System;
using System.Collections.Generic;

namespace IACMWeb.Comp.Policy.Wizard
{
    public partial class CompliancePolicyWizardUC : UIBaseUC
    {
        #region Class Variables
        private SessionStatusImpl session = new SessionStatusImpl();
        private const string V_DISPLAYMODE = "DISPLAYMODE";
        private const string V_ENTITYID = "V_ENTITYID";
        private CompliancePolicy policy = null;
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
        public long EntityId
        {
            get
            {
                if (ViewState[V_ENTITYID] == null)
                    return 0;
                return (long)ViewState[V_ENTITYID];
            }
            set
            {
                ViewState[V_ENTITYID] = value;
            }
        }
        public CompliancePolicy CompliancePolicy
        {
            set { policy = value; }
            get { return policy; }
        }
        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to initialise Page
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="isViewMode"></param>
        public void InitilizePage()//bool isEditMode, bool isViewMode
        {
            if (UIMode==UIMode.Edit)
            {
                SetPage();              
            }
            else if (UIMode==UIMode.View)
            {
                SetPage();
                ComplianceDetails.DisplayMode();
                ConflictDetails.DisplayMode();
            }
            ComplianceDetails.UIMode = UIMode;
            ConflictDetails.UIMode = UIMode;
            ComplianceDetails.InitilizePage();
            ConflictDetails.InitilizePage();
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
                InitilizePage();
        }

        /// <summary>
        /// Method to handle finish button click event in ComplianceRadWizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ComplianceRadWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            try
            {
                if (UIMode != UIMode.View)
                {
                    CompliancePolicy compliancePolicy = ComplianceDetails.GetCompliancePolicy();
                    if(string.IsNullOrEmpty(compliancePolicy.CompliancePolicyCode))
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Compliance Policy Code should not be empty");
                        return;
                    }
                    if (compliancePolicy.OwnerId == null )
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Compliance Owner should not be empty");
                        return;
                    }
                    compliancePolicy.Conflicts = new List<SODConflictFunctions>();
                    if (ConflictDetails.SelectedConflicts.Count > 0)
                    {
                        foreach (var entry in ConflictDetails.SelectedConflicts)
                        {
                            Dictionary<string, string> parameters = new Dictionary<string, string>();
                            parameters.Add("id", entry.Key.ToString());
                            SODConflictFunctions conflict = DomainApiClient<SODConflictFunctions>.GetByAction<SODConflictFunctions>
                                (DomainApiAction.SODFunction.SODConflictFunction.GetByIdIncludingSODRiskCode, parameters);

                            if (conflict != null)
                            {
                                compliancePolicy.Conflicts.Add(conflict);
                            }
                        }
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Please add at least one conflict");
                        return;
                    }
                    if (UIMode == UIMode.Edit)
                    {
                        
                        compliancePolicy.Id = Convert.ToInt64(this.ContextObjectId);
                        CompliancePolicy policy = DomainApiClient<CompliancePolicy>.GetById(this.ContextObjectId.Value);
                        if(policy.CompliancePolicyCode != compliancePolicy.CompliancePolicyCode)
                        {
                            Dictionary<string, string> parameters = new Dictionary<string, string>();
                            parameters.Add("compliancepolicycode", compliancePolicy.CompliancePolicyCode);
                            bool isExists = DomainApiClient<CompliancePolicy>.GetByAction<bool>
                           (DomainApiAction.Complaince.CompliancePolicy.Policy.DoesExists, parameters);
                            if (!isExists)
                            {
                                MasterPage.ShowAlert(AlertType.Error, "Compliance Policy Code already exists");
                                return;
                            }
                        }

                        else
                        {
                            DomainApiClient<CompliancePolicy>.Save(compliancePolicy);
                            Response.Redirect("~//Comp/Policy//List.aspx");
                        }

                    }
                    else
                    {
                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters.Add("compliancepolicycode", compliancePolicy.CompliancePolicyCode);        
                        bool isExists = DomainApiClient<CompliancePolicy>.GetByAction<bool>
                            (DomainApiAction.Complaince.CompliancePolicy.Policy.DoesExists, parameters);
                        if (isExists)
                        {
                            MasterPage.ShowAlert(AlertType.Error, "Compliance Policy Code already exists");
                            return;
                        }
                        else
                        {
                            List<CompliancePolicy> CompliancePolicies = DomainApiClient<CompliancePolicy>.GetAll();
                            if (UIMode == UIMode.Add && CompliancePolicies.Count > 2)
                            {
                                MasterPage.ShowAlert(AlertType.Error, "Only two compliance Policies are allowed.");
                                return;
                            }
                            else
                            {
                                DomainApiClient<CompliancePolicy>.Save(compliancePolicy);
                            }

                        }
                    }
                    
                }
                else
                {
                    Response.Redirect("~//Comp/Policy//List.aspx");
                }
            }
            catch (System.Exception ex)
            {
            }
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
            long conflcitFunctionId = Convert.ToInt64(this.ContextObjectId);
            CompliancePolicy compliancePolicy = GetCompliancePolicy(conflcitFunctionId);
            ComplianceDetails.CompliancePolicy = compliancePolicy;
            ComplianceDetails.EntityId = conflcitFunctionId;
            ConflictDetails.CompliancePolicy = compliancePolicy;
        }

        /// <summary>
        /// Method to get compliance policy
        /// </summary>
        /// <param name="compliancePolicyId"></param>
        /// <returns></returns>
        private CompliancePolicy GetCompliancePolicy(long compliancePolicyId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("id", compliancePolicyId.ToString());
            return DomainApiClient<CompliancePolicy>.GetByAction<CompliancePolicy>
                (DomainApiAction.Complaince.CompliancePolicy.Policy.GetByIdWithNavigation, para);
        }
        protected void CompliancePolicyRadWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {

            Response.Redirect("~/Comp/Policy/List.aspx");
        }
        #endregion      
    }
}