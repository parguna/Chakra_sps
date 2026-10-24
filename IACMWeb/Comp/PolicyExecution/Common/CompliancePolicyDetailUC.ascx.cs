using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Web.UI;

namespace DI.Chakra.Web.Comp.PolicyExecution.Common
{
    public partial class CompliancePolicyDetailUC : DI.Chakra.Web.UIBaseUC
    {
        #region Class Variables
        private const string V_ID = "ENTITY_ID";
        private UIMode uiMode;
        private const string V_SODFunction = "SODPolicyExecutionDefinition";
        private const string V_RunCode = "RunCode";
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (RunCode == null)
                {
                    txtUniqueCode.Text = "URC" + new Random().Next().ToString();
                    RunCode = txtUniqueCode.Text;
                }
                else
                    txtUniqueCode.Text = RunCode;
            }                
        }
        #endregion

        #region Public Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }
        public string RunCode
        {
            get
            {
                if (ViewState[V_RunCode] == null)
                    return null;
                return (string)ViewState[V_RunCode];
            }
            set
            {
                ViewState[V_RunCode] = value;
            }
        }
        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
        }

        public SODPolicyExecutionDefinition PolicyDefinition
        {
            get
            {
                if (ViewState[V_SODFunction] == null)
                    return null;
                return (SODPolicyExecutionDefinition)ViewState[V_SODFunction];
            }
            set
            {
                ViewState[V_SODFunction] = value;
            }
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

        #region Initialise Section
        /// <summary>
        /// Method to initilaise page
        /// </summary>
        public void InitilizePage()
        {
            if (UIMode==UIMode.Edit || UIMode==UIMode.View)
            {
                txtUniqueCode.Text = PolicyDefinition.UniqueRunCode;
                CompliancePolicySearch.SelectedPolicy = PolicyDefinition.CompliancePolicy;
                if (PolicyDefinition.OutputType == Infrastructure.Enums.SODOutputType.Workflow)
                    rblWorkflow.Checked = true;
                if (PolicyDefinition.OutputType == Infrastructure.Enums.SODOutputType.Report)
                    rblReport.Checked = true;
            }
            else
            rblReport.Checked = true;
        }
        #endregion
        
        #region Helpers
        /// <summary>
        /// Method to User input as SODPolicyExceutionDefinition
        /// </summary>
        /// <returns></returns>
        public SODPolicyExecutionDefinition GetSODPolicyExceutionDefinition()
        {
            SODPolicyExecutionDefinition definition = new SODPolicyExecutionDefinition();
            definition.UniqueRunCode = txtUniqueCode.Text;
            definition.CompliancePolicy = CompliancePolicySearch.SelectedPolicy;
            if (definition.CompliancePolicy !=null)
                definition.CompliancePolicyId = CompliancePolicySearch.SelectedPolicy.Id;
            if (rblWorkflow.Checked)
                definition.OutputType = Infrastructure.Enums.SODOutputType.Workflow;
            if (rblReport.Checked)
                definition.OutputType = Infrastructure.Enums.SODOutputType.Report;
            return definition;
        }
        #endregion
    }
}