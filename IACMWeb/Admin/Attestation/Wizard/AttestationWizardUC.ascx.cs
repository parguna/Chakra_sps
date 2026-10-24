using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;

namespace DI.Chakra.Web.Attestation.Wizard
{
    public partial class AttestationWizardUC : UIBaseUC//, IWizardControl
    {
        #region Members
        private const string V_ID = "ENTITY_ID";
        public static bool V_NEXTSTEP = true;
        private const string V_ISEDIT = "ISEDIT";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private static DI.Chakra.Dto.Attestation mAttestation;
        SessionStatusImpl sessionStatus = new SessionStatusImpl();
        public string strMessage = "";
        #endregion

        #region properties
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

        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.View;

                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }

        public DI.Chakra.Dto.Attestation attestation
        {
            set
            {
                mAttestation = value;
                SetControl();
            }
        }
        #endregion 

        #region Methods
        private void SetControl()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", this.EntityId.ToString());
            mAttestation = DomainApiClient<Dto.Attestation>.GetByAction<Dto.Attestation>(DomainApiAction.Attestation.GetByIdIncludingAttestationProcesses, parameter);
            AttestationInfo.attestation = mAttestation;
            AttestationScopeInfo.attestation = mAttestation;
            AttestationScopeInfo.attestationScope = mAttestation.AttestationScopes;
            WorkflowInfo.Requesttype = RequestType.Attestation;
            WorkflowInfo.Attestation = mAttestation;
            WorkflowInfo.WorkflowConfiguration = mAttestation.WorkflowConfiguration;
            
            if (UIMode == UIMode.View)
            {
                ContentWrapper.Disabled = true;
                ProvRoleWizard.Enabled = false;
            }
        }



        public void InitilizePage(bool isEditMode)
        {
            if (isEditMode)
            {
                SetControl();
            }
            if (UIMode == UIMode.View)
            {
                SetControl();
                ContentWrapper.Disabled = true;
            }
        }

        public void InitializeControl()
        {
            if ((UIMode == UIMode.View) | (UIMode == UIMode.Edit))
            {
                EntityId = Convert.ToInt64(this.ContextObjectId);
                AttestationInfo.attestation.Id = EntityId;
            }
            AttestationInfo.InitializeControl(UIMode);
            AttestationScopeInfo.InitializeControl(UIMode);
            WorkflowInfo.Requesttype = RequestType.Attestation;
            WorkflowInfo.InitializeControl();
            if ((UIMode == UIMode.View) | (UIMode == UIMode.Edit))
            {
                SetControl();
            }
        }

        private void SaveData(DI.Chakra.Dto.Attestation attstation)
        {
            try
            {
                if (ValidateSecondSteps(-1,-1))
                {
                    if (ValidateScreen())
                    {
                        if ((UIMode == UIMode.Add) || (UIMode == UIMode.Edit))
                        {
                            if (UIMode == UIMode.Add)
                            {
                                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Admin/Attestation - Add");
                            }
                            else if (UIMode == UIMode.Edit)
                            {
                                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Admin/Attestation - Edit");
                            }

                            if (this.IsAttestationNameExists(attstation.AttestationName, attstation.Id))
                            {
                                DuplicateName();
                                return;
                            }
                            attstation.AttestationScopes = AttestationScopeInfo.attestationScope;

                            if (WorkflowInfo.WorkflowConfiguration != null)
                            {
                                attstation.WorkflowConfigurationId = WorkflowInfo.WorkflowConfiguration.Id;

                                WorkflowInfo.GetExceptionConfiguration(attstation);
                            }
                            

                            

                            List<Dto.AttestationHierarchy> lstAttestationHierarchy = new List<Dto.AttestationHierarchy>();
                            AttestationInfo.attestation.AttestationEndpoint.ForEach(a => { lstAttestationHierarchy.AddRange((List<Dto.AttestationHierarchy>)Session[string.Format("AttestationHierarchy_EP_{0}", a.EndpointSystemId)]); });
                            lstAttestationHierarchy.ForEach(a => a.EndpointHierarchy = null);
                            attstation.AttestationHierarchy = lstAttestationHierarchy;
                            var oldAttestation = DomainApiClient<Dto.Attestation>.GetById(attstation.Id, true);
                            if (oldAttestation != null)
                            {
                                var data = $"{GetDataForUILog(oldAttestation,"Old Value")} {GetDataForUILog(attstation, "New Value")}";
                                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, data);
                            }
                            DomainApiClient<Dto.Attestation>.Save(attstation);
                            
                            Session["AttestationHierarchy"] = null;

                            var url = "~/Admin/Attestation/List.aspx";
                            url = url + $"?applicationType={this.ApplicationType}";
                            
                            Response.Redirect(url);
                        }
                        else if (UIMode == UIMode.View)
                        {
                            var url = "~/Admin/Attestation/List.aspx";
                            url = url + $"?applicationType={this.ApplicationType}";
                            Response.Redirect(url);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if(ex.Message != null && !ex.Message.Contains("Thread was being aborted"))
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Exception, "AttestationWizardUC - SaveData", ex);
            }
        }

        private string GetDataForUILog(Dto.Attestation attestation,string caption)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendLine($"{caption}:");
                sb.AppendLine($"AttestationName:{attestation.AttestationName}");
                sb.AppendLine($"AttestationDescription:{attestation.AttestationDescription}");
                sb.AppendLine($"SplitCampaignUsing:{attestation.SplitCampaignUsing}");
                sb.AppendLine($"RequiresAttestationRemediation:{attestation.RequiresAttestationRemediation}");
                sb.AppendLine($"IncludeUnmappedAccount:{attestation.IncludeUnmappedAccount}");
                sb.AppendLine($"EndPoint Scope:");
                foreach (var item in attestation.AttestationEndpoint)
                {
                    sb.AppendLine($"EndPoint Scope:{DomainApiClient<Dto.EndPoint>.GetById(item.EndpointSystemId)?.DisplayName}");
                }
                sb.AppendLine($"Attestation Scope:");
                foreach (var item in attestation.AttestationScopes)
                {
                    if(item.AttestationScopeName.Equals("User") || item.AttestationScopeName.Equals("RO"))
                    {
                        sb.AppendLine($"AttestationScopeName:{item.AttestationScopeName} Value:{DomainApiClient<Dto.User>.GetById(item.AttestationScopeValue.Value).DisplayName}");
                    }
                    else if(item.AttestationScopeName.Equals("IACMGroup"))
                    {
                        sb.AppendLine($"AttestationScopeName:{item.AttestationScopeName} Value:{DomainApiClient<Dto.Group>.GetById(item.AttestationScopeValue.Value)?.GroupName}");
                    }
                    else
                    {
                        sb.AppendLine($"AttestationScopeName:{item.AttestationScopeName} Value:{DomainApiClient<Dto.NOrganisation>.GetById(item.AttestationScopeValue.Value).Name}");
                    }
                }
                sb.AppendLine($"CreateTaskOnNoReviewer:{ attestation.CreateTaskOnNoReviewer}");
                sb.AppendLine($"SendEmailOnNoReviewer:{attestation.SendEmailOnNoReviewer}");
                if (attestation.IsGroupTask)
                {
                    sb.AppendLine($"TaskActionGroup:{DomainApiClient<Dto.Group>.GetById(attestation.TaskActionUserId.Value)?.GroupName}");
                }
                else
                {
                    sb.AppendLine($"TaskActionUser:{DomainApiClient<Dto.User>.GetById(attestation.TaskActionUserId.Value)?.DisplayName}");
                }
                sb.AppendLine($"Workflow:{DomainApiClient<Dto.WorkflowConfiguration>.GetById(attestation.WorkflowConfigurationId)?.WorkflowName}");
                sb.AppendLine($"{Environment.NewLine}:");
            }
            catch (Exception ex)
            {
            }
            return sb.ToString();
        }
        private bool IsAttestationNameExists(string attestationName, long attestationid)
        {
            try
            {
                bool attnameexist = false;
                if (UIMode == UIMode.Edit)
                {
                    if (attestationName.Equals(mAttestation.AttestationName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        attnameexist = false; ;
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("attestationName", attestationName);
                        parameter.Add("attestationid", this.ContextObjectId.Value.ToString());
                        if (DomainApiClient<Dto.Attestation>.GetByAction<bool>(DomainApiAction.Attestation.DoesExists, parameter))
                        {
                            attnameexist = false;
                        }
                        else
                        {
                            parameter["attestationid"] = "0";
                            if (DomainApiClient<Dto.Attestation>.GetByAction<bool>(DomainApiAction.Attestation.DoesExists, parameter))
                                attnameexist = true;                   
                        }
                    }
                }
                else if (UIMode == UIMode.Add)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("attestationName", attestationName);
                    parameter.Add("attestationid", "0");
                    if (DomainApiClient<Dto.Attestation>.GetByAction<bool>(DomainApiAction.Attestation.DoesExists, parameter))
                            attnameexist = true;
                    
                }
                return attnameexist;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void DuplicateName()
        {
            MasterPage.ShowAlert(AlertType.Warning, "Attestation Name already exists. Please select different name.");
        }

        public int ApplicationType
        {

            get
            {
                return Convert.ToInt32(ViewState["ApplicationType"]);
            }
            set
            {
                ViewState["ApplicationType"] = value;
            }
        }

        #endregion

        #region Page Events
        protected void Page_Load(object sender, EventArgs e)
        {
            WorkflowInfo.Requesttype = RequestType.Attestation;
            if (!IsPostBack)
            {
                Session["AttestationHierarchy"] = null;
                ApplicationType = Convert.ToInt32(Request.QueryString["ApplicationType"]);
            }
        }
        protected void ProvRoleWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            //DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Admin/Attestation - Cancel");

            var redirect_url = "~/Admin/Attestation/List.aspx";
            redirect_url = redirect_url + $"?applicationType={this.ApplicationType}";
            Response.Redirect(redirect_url);
        }

        protected void ProvRoleWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            Dto.Attestation attstation = AttestationInfo.attestation;
            if (attstation != null && (!String.IsNullOrEmpty(attstation.AttestationName) && (attstation.AttestationEndpoint != null && attstation.AttestationEndpoint.Count > 0) && (WorkflowInfo.WorkflowConfiguration != null && WorkflowInfo.WorkflowConfiguration.Id > 0) && (AttestationInfo.attestation.AttestationType == AttestationType.User && AttestationScopeInfo.SelectedScopeList.Count != 0)))
                SaveData(attstation);
            else
                MasterPage.ShowAlert(AlertType.Warning, "All Mandatory fields should be filled");
        }
        protected void ProvRoleWizard_NextButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            AttestationScopeInfo.attestation = AttestationInfo.attestation;

            if (ValidateSecondSteps(e.CurrentStepIndex,e.NextStepIndex))
                { }
            else
             e.CurrentStep.Active = true;
        }
        #endregion 

        #region Validations
        public bool ValidateSecondSteps(int currentIndex,int nextIndex)
        {

            

        int blnAttInfoErrFlg = 0;
            if (AttestationInfo.SelectedEndpoints.Count == 0)
            {
                blnAttInfoErrFlg += 1;
                V_NEXTSTEP = false;
                MasterPage.ShowAlert(AlertType.Warning, "Select at least 1 Endpoint system");
            }
            else if (AttestationInfo.attestation.IncludeDormantAccount && AttestationInfo.attestation.DormantAccountDays == 0)
            {
                blnAttInfoErrFlg += 1;
                V_NEXTSTEP = false;
                MasterPage.ShowAlert(AlertType.Error, "Enter no of Dormant Days");
            }
            else if (currentIndex == 0 && AttestationInfo.SelectedEndpointTypes.ContainsValue("SAP"))
            {
                int blnerrEBSFlg = 0;
                if ((AttestationInfo.SelectedEndpoints.ContainsValue("EBS - CRM") && !AttestationInfo.SelectedEndpoints.ContainsValue("EBS - ISU")))
                {
                    blnerrEBSFlg += 1;
                    V_NEXTSTEP = false;
                    MasterPage.ShowAlert(AlertType.Error, "Please select Both Endpoints EBS - CRM and EBS - ISU Collectively.");
                }
                else if ((AttestationInfo.SelectedEndpoints.ContainsValue("EBS - ISU") && !AttestationInfo.SelectedEndpoints.ContainsValue("EBS - CRM")))
                {
                    blnerrEBSFlg += 1;
                    V_NEXTSTEP = false;
                    MasterPage.ShowAlert(AlertType.Error, "Please select Both Endpoints EBS - CRM and EBS - ISU Collectively.");
                }

                int blnMSSLErrFlg = 0;
                if ((AttestationInfo.SelectedEndpoints.ContainsValue("MSSL - ISU") && !AttestationInfo.SelectedEndpoints.ContainsValue("MSSL - CRM")))
                {
                    blnMSSLErrFlg += 1;
                    V_NEXTSTEP = false;
                    MasterPage.ShowAlert(AlertType.Error, "Please select Both Endpoints MSSL - ISU and MSSL - CRM Collectively.");
                }
                else if ((!AttestationInfo.SelectedEndpoints.ContainsValue("MSSL - ISU") && AttestationInfo.SelectedEndpoints.ContainsValue("MSSL - CRM")))
                {
                    blnMSSLErrFlg += 1;
                    V_NEXTSTEP = false;
                    MasterPage.ShowAlert(AlertType.Error, "Please select Both Endpoints MSSL - ISU and  MSSL - CRM Collectively");
                }

                if (blnerrEBSFlg >= 1 && blnMSSLErrFlg >= 1)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select Both Endpoints (MSSL - ISU and MSSL - CRM) and (EBS - CRM and EBS - ISU) Collectively.");
                    blnAttInfoErrFlg += 1;
                }
                else if(blnerrEBSFlg >=1 || blnMSSLErrFlg >= 1)
                    blnAttInfoErrFlg += 1;
                else if (blnerrEBSFlg == 0 && blnMSSLErrFlg == 0)
                    V_NEXTSTEP = true;

            }
            
            if(blnAttInfoErrFlg == 0)
            {
                if (nextIndex > 1)
                    currentIndex = 1;
                if (currentIndex > 0 && AttestationInfo.attestation.AttestationType == AttestationType.User && AttestationScopeInfo.SelectedScopeList.Count == 0)
                {
                    V_NEXTSTEP = false;
                    MasterPage.ShowAlert(AlertType.Warning, "Select at least 1 Scope");
                }
                else
                    V_NEXTSTEP = true;
            }
            return V_NEXTSTEP;
        }

        public bool ValidateScreen()
        {
            if (WorkflowInfo.WorkflowConfiguration != null)
            {
                string errorMessage = "";
                if (!WorkflowInfo.ValidateExceptionConfiguration(out errorMessage))
                {
                    MasterPage.ShowAlert(AlertType.Warning, errorMessage);
                    return false;
                }
            }
            return true;
        }
        #endregion      
    }
}