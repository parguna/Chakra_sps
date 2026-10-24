using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.Comp.PolicyExecution.Wizard
{
    public partial class PolicyExecutionWizard : UIBaseUC
    {
        #region Class Member Variables            
        private SessionStatusImpl session = new SessionStatusImpl();
        private const string V_DISPLAYMODE = "DISPLAYMODE";      
        private SODRunPolicy policy = null;
        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to initialise page
        /// </summary>
        public void InitilizePage()
        {
            if (UIMode==UIMode.Edit)
            {
                SetPage();
            }
            else if (UIMode==UIMode.View)
            {
                SetPage();
                CompliancePolicyDetail.DisplayMode();
                CompliancePolicyEndPoint.DisplayMode();
                ComplianceExceptions.DisplayMode();
            }
            CompliancePolicyDetail.UIMode = UIMode;
            CompliancePolicyEndPoint.UIMode = UIMode;
            ComplianceExceptions.UIMode = UIMode;
            CompliancePolicyDetail.InitilizePage();
            CompliancePolicyEndPoint.InitilizePage();
            ComplianceExceptions.InitilizePage();         
        }
        #endregion
        
        #region Public Properties
        public SODRunPolicy SODRunPolicy
        {
            set { policy = value; }
            get { return policy; }
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
        /// Method to set page
        /// </summary>
        private void SetPage()
        {
            long runPolicyId = Convert.ToInt64(this.ContextObjectId);
            SODPolicyExecutionDefinition definition = GetSODPolicyExceutionDefinition(runPolicyId);
            CompliancePolicyDetail.PolicyDefinition = definition;
            CompliancePolicyEndPoint.PolicyDefinition = definition;
            ComplianceExceptions.PolicyDefinition = definition;       
        }

        /// <summary>
        /// Method to get policy execution definition
        /// </summary>
        /// <param name="runPolicyId"></param>
        /// <returns></returns>
        private SODPolicyExecutionDefinition GetSODPolicyExceutionDefinition(long runPolicyId)
        {          
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", runPolicyId.ToString());
          return DomainApiClient<SODPolicyExecutionDefinition>.GetByAction<Dto.SODPolicyExecutionDefinition>
                (DomainApiAction.SODPolicyExecutionDefinition.ExecutionDefinition.GetByIdIncludingCompliance, parameter);
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
        /// Method to handle finish button click for PolicyExecutionRadWizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PolicyExecutionRadWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            try
            {
                if (UIMode!=UIMode.View)
                {
                    SODPolicyExecutionDefinition definition = CompliancePolicyDetail.GetSODPolicyExceutionDefinition();
                    if(definition.CompliancePolicy == null)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Compliance Policy code cannot be empty");
                        return;
                    }
                    List<Dto.SODEndPointPolicySelection> endpointSelections = CompliancePolicyEndPoint.GetEndpointRunlevelSelections();
                    definition.RunAtObjectLevel = CompliancePolicyEndPoint.IsObjectAttributeSelected;
                    definition.EndpointSelection = endpointSelections;
                    definition.Exceptions = new List<Dto.SODException>();
                    foreach (var entry in ComplianceExceptions.SelectedExceptions)
                    {
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("id", entry.Key.ToString());
                        SODException exception=  DomainApiClient<SODException>.GetByAction<Dto.SODException>
                              (DomainApiAction.SODException.Exception.GetByIdIncludeRiskOwner, parameter);
                        if (exception != null)
                        {
                            definition.Exceptions.Add(exception);
                        }
                    }
                    if (UIMode==UIMode.Edit)
                    {
                        definition.Id = Convert.ToInt64(this.ContextObjectId);
                    }               
                    DomainApiClient<SODPolicyExecutionDefinition>.Save(definition);
                }
                Response.Redirect("~//Comp/PolicyExecution//List.aspx");
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }
        protected void PolicyExecutionRadWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {

            Response.Redirect("~/Comp/PolicyExecution/List.aspx");
        }
        #endregion
    }
}