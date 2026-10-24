using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Job.Compliance.Common
{
    public partial class ComplianceinfoUC : DI.Chakra.Web.UIBaseUC
    {
      
        private Dto.ComplianceProcess processConfig = null;
        private bool isEdit = false;
        private bool isView = false;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public Dto.ComplianceProcess ComplianceProcessConfig
        {
            set { processConfig = value; }
            get { return processConfig; }
        }

       
        public bool IsEdit
        {
            set { isEdit = value; }
            get { return isEdit; }
        }
        public bool IsView
        {
            set { isView = value; }
            get { return isView; }
        }
        public void InitilizePage()
        {
            if (IsEdit || IsView)
            {
                txtProcessName.Text = processConfig.ProcessName;
                txtDescription.Text = processConfig.Description;
                string code = processConfig.PolicyExecutionDefinition.CompliancePolicy.CompliancePolicyCode + "_" + processConfig.PolicyExecutionDefinition.UniqueRunCode;
                PolicyDefinitionAutoCompleteBox.SetValue(code, processConfig.PolicyExecutionDefinition.Id.ToString());
            }
        }
        public Dto.ComplianceProcess getComplianceProcess()
        {
            Dto.ComplianceProcess compliance = new Dto.ComplianceProcess();
            compliance.ProcessName = txtProcessName.Text;
            compliance.Description = txtDescription.Text;
            compliance.PolicyExecutionDefinitionId = PolicyDefinitionAutoCompleteBox.GetValue() != null ? Convert.ToInt32(PolicyDefinitionAutoCompleteBox.GetValue().Value) : -1;

            return compliance;
        }
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }
    }
}