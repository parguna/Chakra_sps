using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Application;

using DI.Chakra.Infrastructure;
using DI.Chakra.Web.Job.Attestation.Wizard;
using DI.Chakra.Web.UsrControl.Common;
using DI.Chakra.Web.UsrControl.Scheduler;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Job.Attestation.Common
{
    public partial class JobInfo : UIBaseUC
    {
        #region Variable declaration

        private Dto.AttestationProcess _processConfig = null;
        private bool isEdit = false;
        private bool isView = false;
        private const string V_ID = "ENTITY_ID";
        #endregion

        #region Properties

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
        public string ProcessName
        {
            get { return txtProcessName.Text; }

        }
        public string Description
        {
            get { return txtDescription.Text; }
        }
        public Dto.AttestationProcess AttestationProcessConfig
        {
            set { _processConfig = value; }
            get { return _processConfig; }
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods

        public void InitializePage()
        {
            if (IsEdit || IsView)
            {
                txtProcessName.Text = AttestationProcessConfig.ProcessName;
                txtDescription.Text = AttestationProcessConfig.Description;
                ddlAttestationProcess.Items.Insert(0, new ListItem(AttestationProcessConfig.Attestation.AttestationName, Convert.ToString(AttestationProcessConfig.Attestation.Id)));
            }
            else
            {
                BindAttestationName();
            }
        }

        protected void BindAttestationName()
        {
            IOrderedEnumerable<Dto.Attestation> attestationNameList = DomainApiClient<Dto.Attestation>.GetAll().OrderBy(att=>att.AttestationName);
            try
            {
                ddlAttestationProcess.DataSource = attestationNameList;
                ddlAttestationProcess.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (attestationNameList != null && attestationNameList.ToList().Count > 0)
            {
                ddlAttestationProcess.Items.Insert(0, new ListItem("--Select--", "0"));
            }
        }

        public Dto.AttestationProcess getAttestationProcess()
        {
            Dto.AttestationProcess attestation = new Dto.AttestationProcess();
            attestation.ProcessName = ProcessName;
            attestation.Description = Description;
            attestation.AttestationId = Convert.ToInt64(ddlAttestationProcess.SelectedValue);

            return attestation;
        }

        public bool IsAttestationInProgress(string processName)
        {
            bool isInProgreess = false;
            if (!string.IsNullOrEmpty(processName))
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("processName", processName);
                parameter.Add("workflowStatus", "InProgress");

                isInProgreess = DomainApiClient<Dto.AttestationProcess>.GetByAction<bool>(DomainApiAction.Attestation.AttestationProcess.DoesExistsByWorkflowStatus, parameter);
            }
            return isInProgreess;
        }

        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }

        #endregion

    }
}

