using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class ActionTaskUC : UIBaseUC
    {
        #region Members
        SessionStatusImpl _sessionStatus = new SessionStatusImpl();
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_ISFROMACCESSREQUEST = "V_ISFROMACCESSREQUEST";
        #endregion

        #region Properties
        public bool IsAttestationCompaign
        {
            get
            {
                if (ViewState["IsAttestationCompaign"] != null)
                {
                    return Convert.ToBoolean(ViewState["IsAttestationCompaign"]);
                }
                return false;
            }
            set
            {
                ViewState["IsAttestationCompaign"] = value;
                if(GenericRequestDetailsControl != null)
                {
                    GenericRequestDetailsControl.IsAttestationCompaign = value;
                }
            }
        }
        public IsAdminMode IsAdminMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return IsAdminMode.No;
                return (IsAdminMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }

        EmployeeEventTrigger _eventTrigger;
        public EmployeeEventTrigger eventTrigger
        {
            get
            {
                if (_eventTrigger == null)
                {
                    _eventTrigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetById(Convert.ToInt64(Task.EmployeeEventTriggerId));
                }
                return _eventTrigger;
            }
            set { }
        }
        public long EndpointSystemId
        {
            get
            {
                if (ViewState["EndpointSystemId"] == null)
                    return 0;
                return Convert.ToInt64(ViewState["EndpointSystemId"]);
            }
            set
            {
                ViewState["EndpointSystemId"] = value;
            }
        }
        public bool isFromAccessRequest
        {
            get
            {
                if (ViewState[V_ISFROMACCESSREQUEST] == null)
                    return false;
                return (bool)ViewState[V_ISFROMACCESSREQUEST];
            }
            set
            {
                ViewState[V_ISFROMACCESSREQUEST] = value;
            }
        }

        public Dto.Task Task
        {
            get
            {
                if (ViewState["Task"] != null)
                {
                    return (Dto.Task)ViewState["Task"];
                }
                return new Dto.Task();
            }
            set
            {
                ViewState["Task"] = value;
            }
        }

        public Dto.WorkflowConfiguration workFlowConfig
        {
            get
            {
                if (ViewState["WorkflowConfiguration"] != null)
                {
                    return (Dto.WorkflowConfiguration)ViewState["WorkflowConfiguration"];
                }
                return new Dto.WorkflowConfiguration();
            }
            set
            {
                ViewState["WorkflowConfiguration"] = value;
            }
        }

        public List<PresentationApprovalChain> TaskApprovers
        {
            get
            {
                if (ViewState["TaskApprovers"] != null)
                {
                    return (List<PresentationApprovalChain>)ViewState["TaskApprovers"];
                }
                else
                {
                    return new List<PresentationApprovalChain>();
                }
            }
            set
            {
                ViewState["TaskApprovers"] = value;
            }
        }

        #endregion

        #region Methods

        void BindSAPUserType()
        {
            try
            {
                SAPUserTypeDropDown.Items.Clear();
                SAPUserTypeDropDown.Items.Insert(0, new ListItem("-- Select SAP UserType --", "-1"));

                foreach (SAPUserType item in Enum.GetValues(typeof(SAPUserType)))
                {
                    if (item == SAPUserType.Reference)
                        continue;

                    var description = UtilityMethods.GetEnumAttribute<Description>(item);
                    ListItem listItem = new ListItem(description, ((byte)item).ToString());
                    SAPUserTypeDropDown.Items.Add(listItem);
                }
            }
            catch
            {
                throw;
            }
        }
        private void LoadApplicationDetail()
        {
            Dto.Task task = Task;
            IsAttestationCompaign = Task.AttestationCampaignId.HasValue && Task.AttestationCampaignId.Value > 0;
            if (task.AccessRequest == null || task.AccessRequest.AdHocAccessRequest == null) return;
            Dto.AdHocAccessRequest adhocRequestDetails = task.AccessRequest.AdHocAccessRequest;
            this.ExceptionRemarksValueLabel.Text = task.Remarks;
            this.ExceptionRemarkstr.Visible = string.IsNullOrWhiteSpace(task.Remarks) ? false : true;
            if (task.AccessRequest != null && task.AccessRequest.AdHocAccessRequest != null && (task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                    && !string.IsNullOrWhiteSpace(task.Remarks) && task.Remarks.Contains("<b>System Remarks</b>"))
            {
                var delimeter = new string[] { "<b>System Remarks</b>" };
                this.ExceptionRemarksValueLabel.Text = task.Remarks.Split(delimeter, StringSplitOptions.None)[0];
                this.ExceptionRemarkstr.Visible = this.ExceptionRemarkstr.Visible = string.IsNullOrWhiteSpace(this.ExceptionRemarksValueLabel.Text) ? false : true;
            }
            hdnActionType.Value = ((int)task.AccessRequest.AdHocAccessRequest.AdhocActionType).ToString();

            //ApprovalStatusDetailsUC.AdHocRequestId = this.AdhocAccessRequestId;
            btnSubmit.Visible = false;
            //INHUserInfoUC.Visible = false;
            MSSLAdditionalInfoValueLabel.Text = task.AccessRequest.Remarks;
            AdditionalInformationtr.Visible = string.IsNullOrWhiteSpace(MSSLAdditionalInfoValueLabel.Text) ? false : true;
            ConflictsRemarkLabel.Text = task.AccessRequest.AdditionalInfo;
            ConflictsRemarktr.Visible = ConflictsRemarkImage.Visible = ConflictsRemarkLabel.Visible = string.IsNullOrWhiteSpace(ConflictsRemarkLabel.Text) ? false : true;
            if (task.SubTask != null)
            {
                RoleNameValueLabel.Text = task.SubTask.RoleName;
                TCodeValueLabel.Text = task.SubTask.TCode;
                RemarksValueLabel.Text = task.SubTask.Remarks;
                RoleNameValuetr.Visible = RoleNameValueLabel.Visible = string.IsNullOrWhiteSpace(RoleNameValueLabel.Text) ? false : true;
                TCodeValueLabeltr.Visible = TCodeValueLabel.Visible = string.IsNullOrWhiteSpace(TCodeValueLabel.Text) ? false : true;
                RemarksValuetr.Visible = RemarksValueLabel.Visible = string.IsNullOrWhiteSpace(RemarksValueLabel.Text) ? false : true;
            }
            if (task.IsActionOnlyRequiredTask && Task.AccessRequest.AdHocAccessRequest.EndpointSystemType == EndpointType.InhouseSystems)
            {
                InhouseTaskInfoUC.Task = Task;
                InhouseTaskInfoUC.InitializeForm();
                pnlInhouse.Visible = true;
                InhAccountDetailsHeader.Visible = true;
            }

            if (!task.IsCompleted)
            {

                if (task.IsActionOnlyRequiredTask && ((adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary))))
                {

                    if (adhocRequestDetails.ApplicationType == ApplicationType.FLINS_BI)
                    {
                        SharedEditSAP.Visible = true;
                        BindSAPUserType();
                        MainPanel.Visible = true;
                        MainPanelHeaderSpan.InnerText = "Account Details";
                        var adhocAcessRequest = adhocRequestDetails;
                        var adHocRequest = (AdHocSAPFLINSBIRequest)adhocAcessRequest.GetEntitlementDetail();
                        if (adHocRequest != null)
                        {
                            if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocRequest.AdHocFLINSBISharedAccount.SAPUserType)) != null)
                            {
                                SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocRequest.AdHocFLINSBISharedAccount.SAPUserType)).Selected = true;
                            }
                            txtAccountNameSAP.Text = adHocRequest.AdHocFLINSBISharedAccount.AccountName;
                            txtDescriptionSAP.Text = adHocRequest.AdHocFLINSBISharedAccount.Description;
                        }
                    }
                    else if (adhocRequestDetails.ApplicationType == ApplicationType.HRMS_FLINS)
                    {

                        SharedEditSAP.Visible = true;
                        MainPanel.Visible = true;
                        MainPanelHeaderSpan.InnerText = "Account Details";
                        BindSAPUserType();
                        var adhocAcessRequest = adhocRequestDetails;
                        var adHocRequest = (AdHocSAPHRFLINSRequest)adhocAcessRequest.GetEntitlementDetail();
                        if (adHocRequest != null)
                        {
                            if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocRequest.AdHocSAPHRFLINSSharedAccount.SAPUserType)) != null)
                            {
                                SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocRequest.AdHocSAPHRFLINSSharedAccount.SAPUserType)).Selected = true;
                            }
                            txtAccountNameSAP.Text = adHocRequest.AdHocSAPHRFLINSSharedAccount.AccountName;
                            txtDescriptionSAP.Text = adHocRequest.AdHocSAPHRFLINSSharedAccount.Description;
                        }
                    }
                    else if (adhocRequestDetails.ApplicationType == ApplicationType.SAP_GRC)
                    {
                        SharedEditSAP.Visible = true;
                        MainPanel.Visible = true;
                        MainPanelHeaderSpan.InnerText = "Account Details";
                        BindSAPUserType();
                        var adhocAcessRequest = adhocRequestDetails;
                        var adHocRequest = (AdHocSAPGRCRequest)adhocAcessRequest.GetEntitlementDetail();
                        if (adHocRequest != null)
                        {
                            if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocRequest.AdHocSAPGRCSharedAccount.SAPUserType)) != null)
                            {
                                SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocRequest.AdHocSAPGRCSharedAccount.SAPUserType)).Selected = true;
                            }
                            txtAccountNameSAP.Text = adHocRequest.AdHocSAPGRCSharedAccount.AccountName;
                            txtDescriptionSAP.Text = adHocRequest.AdHocSAPGRCSharedAccount.Description;
                        }
                    }
                    else if (adhocRequestDetails.ApplicationType == ApplicationType.SAP_PI)
                    {
                        SharedEditSAP.Visible = true;
                        MainPanel.Visible = true;
                        MainPanelHeaderSpan.InnerText = "Account Details";
                        BindSAPUserType();
                        var adhocAcessRequest = adhocRequestDetails;
                        var adHocRequest = (AdHocSAPPIRequest)adhocAcessRequest.GetEntitlementDetail();
                        if (adHocRequest != null)
                        {
                            if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocRequest.AdHocSAPPISharedAccount.SAPUserType)) != null)
                            {
                                SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocRequest.AdHocSAPPISharedAccount.SAPUserType)).Selected = true;
                            }
                            txtAccountNameSAP.Text = adHocRequest.AdHocSAPPISharedAccount.AccountName;
                            txtDescriptionSAP.Text = adHocRequest.AdHocSAPPISharedAccount.Description;
                        }
                    }
                }
                if (task.IsActionOnlyRequiredTask && adhocRequestDetails.ApplicationType == ApplicationType.MSSL_ISU || adhocRequestDetails.ApplicationType == ApplicationType.MSSL_CRM || adhocRequestDetails.ApplicationType == ApplicationType.EBS_ISU || adhocRequestDetails.ApplicationType == ApplicationType.EBS_CRM)
                {
                    if (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary))
                    {
                        MainPanel.Visible = task.IsActionOnlyRequiredTask;
                        MainPanelHeaderSpan.InnerText = "Account Details";
                        SharedEditSAP.Visible = true;
                        BindSAPUserType();
                        if (task.IsActionOnlyRequiredTask && adhocRequestDetails.ApplicationType == ApplicationType.MSSL_ISU || adhocRequestDetails.ApplicationType == ApplicationType.MSSL_CRM)
                        {
                            var adhocAcessRequest = adhocRequestDetails;
                            var data = adhocAcessRequest.GetEntitlementDetail();
                            if (data is AdHocMSSLRequest)
                            {
                                var adHocMSSLRequest = (AdHocMSSLRequest)data;
                                if (adHocMSSLRequest != null)
                                {
                                    if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocMSSLRequest.AdHocSAPMSSLSharedAccount.SAPUserType)) != null)
                                    {
                                        SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocMSSLRequest.AdHocSAPMSSLSharedAccount.SAPUserType)).Selected = true;
                                    }
                                    txtAccountNameSAP.Text = adHocMSSLRequest.AdHocSAPMSSLSharedAccount.AccountName;
                                    txtDescriptionSAP.Text = adHocMSSLRequest.AdHocSAPMSSLSharedAccount.Description;
                                }
                            }
                            else
                            {
                                var adHocMSSLRequest = (AdHocMSSLRequest)data;
                                if (adHocMSSLRequest != null)
                                {
                                    if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocMSSLRequest.AdHocSAPMSSLSharedAccount.SAPUserType)) != null)
                                    {
                                        SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocMSSLRequest.AdHocSAPMSSLSharedAccount.SAPUserType)).Selected = true;
                                    }
                                    txtAccountNameSAP.Text = adHocMSSLRequest.AdHocSAPMSSLSharedAccount.AccountName;
                                    txtDescriptionSAP.Text = adHocMSSLRequest.AdHocSAPMSSLSharedAccount.Description;
                                }
                            }
                        }
                        else
                        {
                            var adhocAcessRequest = adhocRequestDetails;
                            var data = adhocAcessRequest.GetEntitlementDetail();
                            if (data is AdHocEBSRequest)
                            {
                                var adHocEBSRequest = (AdHocEBSRequest)data;
                                if (adHocEBSRequest != null)
                                {
                                    if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocEBSRequest.AdHocSAPEBSSharedAccount.SAPUserType)) != null)
                                    {
                                        SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocEBSRequest.AdHocSAPEBSSharedAccount.SAPUserType)).Selected = true;
                                    }
                                    txtAccountNameSAP.Text = adHocEBSRequest.AdHocSAPEBSSharedAccount.AccountName;
                                    txtDescriptionSAP.Text = adHocEBSRequest.AdHocSAPEBSSharedAccount.Description;
                                }
                            }
                            else
                            {
                                var adHocEBSRequest = (AdHocEBSRequest)data;
                                if (adHocEBSRequest != null)
                                {
                                    if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocEBSRequest.AdHocSAPEBSSharedAccount.SAPUserType)) != null)
                                    {
                                        SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocEBSRequest.AdHocSAPEBSSharedAccount.SAPUserType)).Selected = true;
                                    }
                                    txtAccountNameSAP.Text = adHocEBSRequest.AdHocSAPEBSSharedAccount.AccountName;
                                    txtDescriptionSAP.Text = adHocEBSRequest.AdHocSAPEBSSharedAccount.Description;
                                }
                            }
                        }
                    }
                    else
                    {
                        var adhocAcessRequest = adhocRequestDetails;
                        if (adhocRequestDetails.ApplicationType == ApplicationType.MSSL_ISU || adhocRequestDetails.ApplicationType == ApplicationType.MSSL_CRM)
                        {
                            bool isMssl = false;
                            var data = adhocAcessRequest.GetEntitlementDetail();
                            if (data != null && data is AdHocMSSLRequest)
                                isMssl = true;
                            if (data != null && adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && adhocRequestDetails.AccountType == AccountType.User && string.IsNullOrWhiteSpace((isMssl ? ((AdHocMSSLRequest)data).PositionPrefix : ((AdHocMSSLRequest)data).PositionPrefix)))
                            {
                                MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                MainPanel.Visible = task.IsActionOnlyRequiredTask;
                            }
                            else if (data != null && adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd && adhocRequestDetails.AccountType == AccountType.User && (isMssl ? ((AdHocMSSLRequest)data).IsReallocate : ((AdHocMSSLRequest)data).IsReallocate) && string.IsNullOrWhiteSpace((isMssl ? ((AdHocMSSLRequest)data).PositionPrefix : ((AdHocMSSLRequest)data).PositionPrefix)))
                            {
                                MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                MainPanel.Visible = task.IsActionOnlyRequiredTask;
                            }
                        }
                        else
                        {
                            bool isEbs = false;
                            var data = adhocAcessRequest.GetEntitlementDetail();
                            if (data != null && data is AdHocEBSRequest)
                                isEbs = true;
                            if (data != null && adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && adhocRequestDetails.AccountType == AccountType.User && string.IsNullOrWhiteSpace((isEbs ? ((AdHocEBSRequest)data).PositionPrefix : ((AdHocEBSRequest)data).PositionPrefix)))
                            {
                                MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                MainPanel.Visible = task.IsActionOnlyRequiredTask;
                            }
                            if (data != null && adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd && adhocRequestDetails.AccountType == AccountType.User && (isEbs ? ((AdHocEBSRequest)data).IsReallocate : ((AdHocEBSRequest)data).IsReallocate) && string.IsNullOrWhiteSpace((isEbs ? ((AdHocEBSRequest)data).PositionPrefix : ((AdHocEBSRequest)data).PositionPrefix)))
                            {
                                MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                MainPanel.Visible = task.IsActionOnlyRequiredTask;
                            }
                            //else if (data != null && adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount
                            //          && adhocRequestDetails.AccountType == AccountType.User
                            //          && string.IsNullOrEmpty((isEbs ? ((AdHocEBSRequest)data).PositionPrefix : ((AdHocEBSRequest)data).PositionPrefix))
                            //          && adhocRequestDetails.ApplicationType == ApplicationType.EBS_BI)
                            //{
                            //    bool canRequestBI = false;

                            //    Dictionary<string, string> para = new Dictionary<string, string>{
                            //        { "userId", adhocRequestDetails.RequesteeUserId.ToString() },
                            //        { "endpointSystemId", adhocRequestDetails.EndpointSystemId.ToString() } };
                            //    List<AccountReconcilation> sapUsers = DomainApiClient<Dto.AccountReconcilation>.GetByAction<List<Dto.AccountReconcilation>>
                            //        (DomainApiAction.Provisioning.AccountReconcilation.GetByUserIdAndEndpointSystemId,
                            //         para);
                            //    if (sapUsers.Count > 0)
                            //    {
                            //        canRequestBI = true;
                            //    }
                            //    if (!canRequestBI)
                            //    {
                            //        MainPanelHeaderSpan.InnerText = "Prefix Submission";
                            //        PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                            //        MainPanel.Visible = task.IsActionOnlyRequiredTask;
                            //    }
                            //}

                        }

                    }
                }
                if (task.IsActionOnlyRequiredTask && adhocRequestDetails.ApplicationType == ApplicationType.MSSL || adhocRequestDetails.ApplicationType == ApplicationType.EBS)
                {
                    if (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary))
                    {
                        MainPanel.Visible = task.IsActionOnlyRequiredTask;
                        MainPanelHeaderSpan.InnerText = "Account Details";
                        SharedEditSAP.Visible = true;
                        BindSAPUserType();
                        {
                            var adhocAcessRequest = adhocRequestDetails;
                            var data = adhocAcessRequest.GetEntitlementDetail();
                            if (data is AdHocSAPHANARequest)
                            {
                                var adHocSAPHANARequest = (AdHocSAPHANARequest)data;
                                if (adHocSAPHANARequest != null)
                                {
                                    if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocSAPHANARequest.AdHocSAPHANASharedAccount.SAPUserType)) != null)
                                    {
                                        SAPUserTypeDropDown.Items.FindByText(Convert.ToString(adHocSAPHANARequest.AdHocSAPHANASharedAccount.SAPUserType)).Selected = true;
                                    }
                                    txtAccountNameSAP.Text = adHocSAPHANARequest.AdHocSAPHANASharedAccount.AccountName;
                                    txtDescriptionSAP.Text = adHocSAPHANARequest.AdHocSAPHANASharedAccount.Description;
                                }
                            }
                        }
                    }
                    else
                    {
                        var adhocAcessRequest = adhocRequestDetails;
                        {
                            var data = adhocAcessRequest.GetEntitlementDetail();
                            var sAPHANARequest = (AdHocSAPHANARequest)data;
                            if (data != null && adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && adhocRequestDetails.AccountType == AccountType.User 
                                && string.IsNullOrWhiteSpace(sAPHANARequest.PositionPrefix))
                            {
                                MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                MainPanel.Visible = task.IsActionOnlyRequiredTask;
                            }
                            if (data != null && adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd && adhocRequestDetails.AccountType == AccountType.User &&
                                sAPHANARequest.IsReallocate
                                && string.IsNullOrWhiteSpace(sAPHANARequest.PositionPrefix))
                            {
                                MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                MainPanel.Visible = task.IsActionOnlyRequiredTask;
                            }

                        }

                    }
                }
                if (task.IsActionOnlyRequiredTask && adhocRequestDetails.ApplicationType == ApplicationType.MicrosoftTeams)
                {
                    var adhocAcessRequest = adhocRequestDetails;
                    EndpointSystemId = adhocAcessRequest.EndpointSystemId.Value;
                    var currentAccessRequest = (AdhocSkypeForBusinessRequest)adhocAcessRequest.GetEntitlementDetail();
                    if (adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd)
                    {

                        //Special Number case
                        if (currentAccessRequest.EntitlementType == EntitlementType.Default)
                        {
                            lblLineUrIPrefix.Text = currentAccessRequest.LineUri;
                            this.SkypeForBusinessPanel.Visible = true;
                            divSpecialNumberText.Visible = true;
                            lblSpecialNumber.Text = "Request for Special Number";
                            ReservedNumberAutoCompleteBox.Visible = true;
                            var param = new Dictionary<string, string>();
                            param.Add("endpointSystemId", this.EndpointSystemId.ToString());
                            param.Add("fieldName", "LineUriPrefix");
                            var field = DomainApiClient<Dto.NProvisioningField>.GetByAction<Dto.NProvisioningField>(DomainApiAction.Ruleset.NProvisioningField.GetByEndpointSystemIdAndFieldName, param);
                            if (field != null)
                            {
                                lblLineUrIPrefix.Text = field.Value;
                            }
                        }
                        else if (currentAccessRequest.EntitlementType == EntitlementType.DCC)
                        {
                            ShowWarningIfUserHasResponseGroup(currentAccessRequest, adhocAcessRequest.EndpointSystemId);
                            //ShowDCCLineUriPrefix();
                            //txtDCCExtension.Text = currentAccessRequest.Extension;
                            //txtDCCExtension.Visible = true;
                        }
                    }
                    else if (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        if (currentAccessRequest.EntitlementType == EntitlementType.DCC)
                        {
                            //ShowDCCLineUriPrefix();
                            txtDCCExtension.Text = currentAccessRequest.Extension;
                            //txtDCCExtension.Enabled = true;
                            //txtDCCExtension.Visible = true;
                        }
                    }
                }
                if (task.IsActionOnlyRequiredTask && task.AccessRequest.ApplicationType == ApplicationType.VPN)
                {
                    var adhocAcessRequest = adhocRequestDetails;
                    var currentAccessRequest = (AdHocVPNRequest)adhocAcessRequest.GetEntitlementDetail();
                    long groupId = 0;
                    if (task.IsGroupTask && task.ActionGroupId.HasValue && task.ActionGroupId > 0)
                    {
                        groupId = task.ActionGroupId.Value;
                    }
                    if (groupId == MasterConfigHelper.ITSecurityGroupId)
                    {
                        if (task.IsActionOnlyRequiredTask)
                        {
                            if (task.IsActionOnlyRequiredTask && adhocRequestDetails.AdhocActionType != AdHocActionType.DeleteAccount)
                            {
                                OneKeyPanel.Visible = true;
                            }
                            txtOneKey.Text = currentAccessRequest.OneKey;
                        }
                    }
                }
                if (task.AccessRequest.ApplicationType == ApplicationType.VPN)
                {
                    var adhocAcessRequest = adhocRequestDetails;
                    var currentAccessRequest = (AdHocVPNRequest)adhocAcessRequest.GetEntitlementDetail();
                    if (adhocAcessRequest.AdhocActionType == AdHocActionType.AmendmentAdd && currentAccessRequest.UpdateOneKeyToken == false)
                    {
                        txtOneKey.Enabled = false;
                    }
                    long groupId = 0;
                    if (task.IsGroupTask && task.ActionGroupId.HasValue && task.ActionGroupId > 0)
                    {
                        groupId = task.ActionGroupId.Value;
                    }
                    if (groupId == MasterConfigHelper.NetworkManagement || groupId == MasterConfigHelper.ITSecurityGroupId)
                    {
                        var ADdetail = DomainApiClient<Dto.ADSUserInfo>.GetByAction<List<ADSUserInfo>>(DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.GetByDIUserId,
                               new Dictionary<string, string> { { "DIUserId", adhocRequestDetails.RequesteeUserId.ToString() } }).FirstOrDefault();
                        if (ADdetail != null)
                            GenericRequestDetailsControl.AdName = ADdetail.sAMAccountName;
                        else
                        {
                            if (adhocAcessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                            {
                                GenericRequestDetailsControl.AdName = string.Empty;
                            }
                            else
                            {
                                this.GenericRequestDetailsControl.AdName = "(AD account is not created yet, please contact security operations.)";
                            }
                        }
                    }
                }
                #region Commented
                //if (adhocRequestDetails.ApplicationType == ApplicationType.VPN)
                //{
                //    var adhocAcessRequest = adhocRequestDetails;
                //    var currentAccessRequest = (AdHocVPNRequest)adhocAcessRequest.GetEntitlementDetail();
                //    long groupId = 0;
                //    if (task.IsGroupTask && task.ActionGroupId.HasValue && task.ActionGroupId > 0)
                //    {
                //        groupId = task.ActionGroupId.Value;
                //    }
                //    if (groupId == MasterConfigHelper.ITSecurityGroupId)
                //    {
                //        if (task.IsActionOnlyRequiredTask)
                //        {
                //            if (task.IsActionOnlyRequiredTask && adhocRequestDetails.AdhocActionType != AdHocActionType.DeleteAccount)
                //            {
                //                OneKeyPanel.Visible = true;
                //            }
                //            txtOneKey.Text = currentAccessRequest.OneKey;
                //        }
                //    }

                //    //long groupId = task.ActionGroupId ?? default(long);
                //    //if (groupId == MasterConfigHelper.NetworkManagement || groupId == MasterConfigHelper.ITSecurityGroupId)
                //    //{
                //    //    var ADdetail = DomainApiClient<Dto.ADSUserInfo>.GetByAction<List<ADSUserInfo>>(DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.GetByDIUserId,
                //    //           new Dictionary<string, string> { { "DIUserId", adhocRequestDetails.RequesteeUserId.ToString() } }).FirstOrDefault();
                //    //    if (ADdetail != null)
                //    //        GenericRequestDetailsControl.AdName = ADdetail.sAMAccountName;
                //    //    else
                //    //    {
                //    //        if (adhocRequestDetails.AdhocActionType == AdHocActionType.DeleteAccount)
                //    //        {
                //    //            GenericRequestDetailsControl.AdName = string.Empty;
                //    //        }
                //    //        else
                //    //        {
                //    //            GenericRequestDetailsControl.AdName = "(AD account is not created yet, please contact security operations.)";
                //    //        }
                //    //    }
                //    //}
                //}
                #endregion Commented
                if (adhocRequestDetails.ApplicationType == ApplicationType.ADSingaporePowerLocal //|| adhocRequestDetails.ApplicationType== ApplicationType.ADSDCLocal
                    || adhocRequestDetails.ApplicationType == ApplicationType.SAPLocal)
                {
                    GenericRequestDetailsControl.BindADGroupInfo(task);
                }
                #region GEMS SharedAccountEdit
                if (task.IsActionOnlyRequiredTask && (adhocRequestDetails.ApplicationType == ApplicationType.GEMS) && (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary)))
                {
                    SharedAccountEditPanel.Visible = true;
                    SharedAccountGEMSEditPanel.Visible = true;
                    var adhocAcessRequest = adhocRequestDetails;
                    var adHocRequest = (AdHocGEMSRequest)adhocAcessRequest.GetEntitlementDetail();
                    if (adHocRequest != null)
                    {
                        txtAccountName.Text = adHocRequest.AdHocGEMSSharedAccount.AccountName;
                        txtEmail.Text = adHocRequest.AdHocGEMSSharedAccount.EmailAddress;
                        txtDescriptionShared.Text = adHocRequest.AdHocGEMSSharedAccount.Description;
                        txtDisplayNameShared.Text = adHocRequest.AdHocGEMSSharedAccount.DisplayName;
                    }
                }
                #endregion
                #region O365 SharedAccountEdit
                if (task.IsActionOnlyRequiredTask && (adhocRequestDetails.ApplicationType == ApplicationType.Office365))
                {
                    var adHocRequest = (AdHocO365Request)adhocRequestDetails.GetEntitlementDetail();
                    if (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary))
                    {
                        SharedAccountEditPanel.Visible = true;
                        O365SharedEditPanel.Visible = true;

                        if (adHocRequest != null)
                        {
                            txtboxAliasName.Text = adHocRequest.AdHocO365SharedAccount.AccountName;
                            txtboxDisplayName.Text = adHocRequest.AdHocO365SharedAccount.DisplayName;

                        }
                    }

                    if ((adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount || adhocRequestDetails.AdhocActionType == AdHocActionType.DeleteAccount || adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentRemove) && adhocRequestDetails.AccountType == AccountType.Shared)
                    {
                        if (adHocRequest.MailBoxConversionType == MailBoxConversionType.UserToShared || (adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentRemove && adHocRequest.MailBoxConversionType == MailBoxConversionType.SharedToUser))
                        {
                            int o365AdminGroup = MasterConfigHelper.O365Admin;
                            var adminGroup = DomainApiClient<Dto.Group>.GetById(o365AdminGroup);
                            string ProcurementTeam = adminGroup?.GroupName;
                            if (!string.IsNullOrWhiteSpace(ProcurementTeam))
                            {
                                var group = DomainApiClient<Dto.Group>.GetByAction<Dto.Group>(DomainApiAction.Group.GetByGroupName, new Dictionary<string, string> { { "groupName", ProcurementTeam } });
                                if (group != null)
                                {
                                    bool IsWintelAdmin = DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.CheckUserGroup,
                                                                new Dictionary<string, string> { { "userId",
                                                                (task.ActionUserId.HasValue) ? task.ActionUserId.ToString() : ( (task.AssignedTo.HasValue)? task.AssignedTo.ToString() : new SessionStatusImpl().User.Id.ToString() )  },
                                                                                                { "GroupID", group.Id.ToString() }
                                                                });

                                    if (IsWintelAdmin)
                                    {
                                        sharedAccountO365NewAccountPanel.Visible = true;
                                        SharedAccountEditPanel.Visible = true;
                                        var user = DomainApiClient<Dto.User>.GetById(adhocRequestDetails.RequesterUserId);
                                        if (user != null && !string.IsNullOrWhiteSpace(user.CostCenter))
                                            lblRequesterCostCenter.Text = $"{user.CostCenterText}[{user.CostCenter}]";
                                        //txtEntityId.Text = "";
                                    }
                                }
                            }
                        }
                    }

                }
                #endregion
                #region SAP SRM
                if (task.IsActionOnlyRequiredTask && (adhocRequestDetails.ApplicationType == ApplicationType.SRM) && (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary)))
                {
                    SharedAccountEditPanel.Visible = true;
                    SharedAccountSRMEditPanel.Visible = true;
                    var adhocAcessRequest = adhocRequestDetails;
                    var adHocRequest = (AdHocSAPSRMRequest)adhocAcessRequest.GetEntitlementDetail();
                    if (adHocRequest != null)
                    {
                        txtSRMAccountName.Text = adHocRequest.AdHocSAPSRMSharedAccount.AccountName;

                    }
                }
                #endregion

                #region INH SharedAccountEdit
                // Commenting out the below code snippet since we are using the any of the values txtAcntNameIHN, txtDisplayNameINH, txtDiscriptionINH
                //if (task.IsActionOnlyRequiredTask && (adhocRequestDetails.ApplicationType == ApplicationType.InhouseSystems) && (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary)))
                //{
                //    SharedAccountEditPanel.Visible = true;
                //    SharedAccountInhouseEditPanel.Visible = true;
                //    var adhocAcessRequest = adhocRequestDetails;
                //    var adHocRequest = (AdHocINHRequest)adhocAcessRequest.GetEntitlementDetail();
                //    if (adHocRequest != null)
                //    {
                //        txtAcntNameIHN.Text = adHocRequest.AdHocINHSharedAccount.AccountName;
                //        txtDisplayNameINH.Text = adHocRequest.AdHocINHSharedAccount.DisplayName;
                //        txtDiscriptionINH.Text = adHocRequest.AdHocINHSharedAccount.Description;
                //    }
                //}
                #endregion
                #region AD SharedAccountEdit
                if (task.IsActionOnlyRequiredTask && (//adhocRequestDetails.ApplicationType== ApplicationType.SAPLocal ||
                    adhocRequestDetails.ApplicationType == ApplicationType.ADSingaporePowerLocal) && (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary)))
                {
                    SharedAccountEditPanel.Visible = true;
                    SharedAccountADEditPanel.Visible = true;
                    trOU.Visible = true;
                    Session["EndpointSystemID"] = adhocRequestDetails.EndpointSystemId;
                    var adhocAcessRequest = adhocRequestDetails;
                    var adHocADRequest = (AdHocADRequest)adhocAcessRequest.GetEntitlementDetail();
                    if (adHocADRequest != null)
                    {
                        txtAcntName.Text = adHocADRequest.AdHocADSharedAccount.AccountName;
                        txtDisplayName.Text = adHocADRequest.AdHocADSharedAccount.DisplayName;
                        txtDiscription.Text = adHocADRequest.AdHocADSharedAccount.Description;

                        if (string.IsNullOrWhiteSpace(adHocADRequest.AdHocADSharedAccount.OU))
                        {
                            if (adhocAcessRequest.AccountType == AccountType.User)
                            {
                                var defaultOU = DomainApiClient<ADSOuInfo>.GetByAction<ADSXLProvisioningMatrix>(DomainApiAction.Endpoint.ActiveDirectory.ADSOuInfo.GetDefaultOU, new Dictionary<string, string> { { "DiUserId", adhocAcessRequest.RequesteeUserId.ToString() } });
                                if (defaultOU != null)
                                    adHocADRequest.AdHocADSharedAccount.OU = defaultOU.OU;
                            }
                            else if (adhocAcessRequest.AccountType == AccountType.Secondary)
                            {
                                adHocADRequest.AdHocADSharedAccount.OU = MasterConfigHelper.ADSecondaryAccountOU;
                                //OUAutoCompleteBox.Enabled = false;
                            }
                            else if (adhocAcessRequest.AccountType == AccountType.Shared)
                            {
                                adHocADRequest.AdHocADSharedAccount.OU = MasterConfigHelper.ADSharedAccountOU;
                                //OUAutoCompleteBox.Enabled = false;
                            }
                            else if (adhocAcessRequest.AccountType == AccountType.Service)
                            {
                                adHocADRequest.AdHocADSharedAccount.OU = MasterConfigHelper.ADServiceAccountOU;
                                //OUAutoCompleteBox.Enabled = false;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(adHocADRequest.AdHocADSharedAccount.OU))
                        {
                            OUAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(adHocADRequest.AdHocADSharedAccount.OU));
                        }
                    }
                }

                #endregion
                #region SAPLocal
                if (task.IsActionOnlyRequiredTask && (adhocRequestDetails.ApplicationType == ApplicationType.SAPLocal) && (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount))
                {
                    SharedAccountEditPanel.Visible = true;
                    SharedAccountADEditPanel.Visible = true;
                    trOU.Visible = true;
                    Session["EndpointSystemID"] = adhocRequestDetails.EndpointSystemId;
                    var adhocAcessRequest = adhocRequestDetails;
                    var adHocADRequest = (AdHocSAPLocalRequest)adhocAcessRequest.GetEntitlementDetail();
                    if (adHocADRequest != null)
                    {
                        txtAcntName.Text = adHocADRequest.AccountName;
                        txtDisplayName.Text = adHocADRequest.DisplayName;
                        txtDiscription.Text = adHocADRequest.Description;
                        if (string.IsNullOrWhiteSpace(adHocADRequest.OU))
                        {
                            if (adhocAcessRequest.AccountType == AccountType.User)
                            {
                                //var defaultOU = DomainApiClient<ADSOuInfo>.GetByAction<ADSXLProvisioningMatrix>(DomainApiAction.Endpoint.ActiveDirectory.ADSOuInfo.GetDefaultOU, new Dictionary<string, string> { { "DiUserId", adhocAcessRequest.RequesteeUserId.ToString() } });
                                //if (defaultOU != null)
                                //    adHocADRequest.OU = defaultOU.OU;
                                //if (defaultOU != null)
                                adHocADRequest.OU = MasterConfigHelper.ADSAPLocalUserAccountsOU;
                            }
                            else if (adhocAcessRequest.AccountType == AccountType.Secondary)
                            {
                                adHocADRequest.OU = MasterConfigHelper.ADSAPLocalSecondaryAccountsOU;
                                // OUAutoCompleteBox.Enabled = false;
                            }
                            else if (adhocAcessRequest.AccountType == AccountType.Shared)
                            {
                                adHocADRequest.OU = MasterConfigHelper.ADSAPLocalSharedAccountsOU;
                                // OUAutoCompleteBox.Enabled = false;
                            }
                            else if (adhocAcessRequest.AccountType == AccountType.Service)
                            {
                                adHocADRequest.OU = MasterConfigHelper.ADSAPLocalServiceAccountsOU;
                                // OUAutoCompleteBox.Enabled = false;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(adHocADRequest.OU))
                        {
                            OUAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(adHocADRequest.OU));
                        }
                    }
                }
                #endregion SAPLocal




                #region SAPEP

                GroupEntilementtr.Visible = false;
                RoleEntilementtr.Visible = false;
                SAPEPEntitlementPanel.Visible = false;
                btnGrpAdd.Visible = false;
                btnroleAdd.Visible = false;


                if (task.IsActionOnlyRequiredTask && (adhocRequestDetails.ApplicationType == ApplicationType.SAP_EP || adhocRequestDetails.ApplicationType == ApplicationType.CE))
                {
                    MainPanel.Visible = true;
                    int WintelAdminGroupID = MasterConfigHelper.WBSAdmin;
                    bool IsWBSAdmin = DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.CheckUserGroup,
                                                    new Dictionary<string, string> { { "userId",(task.ActionUserId.HasValue) ? task.ActionUserId.ToString() : ( (task.AssignedTo.HasValue)? task.AssignedTo.ToString() : new SessionStatusImpl().User.Id.ToString() ) },
                                                                                                { "GroupID", WintelAdminGroupID.ToString() }
                                                    });

                    if (task.WorkflowStateId == 1 && ((adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount) && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary)))
                    {
                        MainPanelHeaderSpan.InnerText = "Account Details";
                        SharedEditSAP.Visible = true;
                        this.SAPUserTypeRow.Visible = false;
                        var adhocAcessRequest = adhocRequestDetails;
                        var sapepDetails = (AdhocSAPEPRequest)adhocAcessRequest.GetEntitlementDetail();
                        if (sapepDetails != null)
                        {
                            txtAccountNameSAP.Text = sapepDetails.AdHocSAPEPSharedAccount.AccountName;
                            txtDescriptionSAP.Text = sapepDetails.AdHocSAPEPSharedAccount.Description;
                        }
                    }

                    if (IsWBSAdmin)
                    {
                        MainPanelHeaderSpan.InnerText = "Entitlement Submission";
                        SAPEPEntitlementPanel.Visible = task.IsActionOnlyRequiredTask;
                        GroupEntilementtr.Visible = task.IsActionOnlyRequiredTask;
                        RoleEntilementtr.Visible = task.IsActionOnlyRequiredTask;
                        var adhocAcessRequest = adhocRequestDetails;
                        var sapepDetails = (AdhocSAPEPRequest)adhocAcessRequest.GetEntitlementDetail();

                        btnGrpAdd.Visible = true;

                        btnroleAdd.Visible = true;
                        if (adhocAcessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                        {
                            SAPEPEntitlementPanel.Visible = false;
                        }
                        if (adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentRemove)
                        {

                            hdnEndpointUserId.Value = sapepDetails.EndpointUserId.ToString();
                            EndpointSystemId = adhocRequestDetails.EndpointSystemId.Value;

                            GroupsAutoCompleteBox.Visible = false;
                            RolesAutoCompleteBox.Visible = false;
                            ExistingGroupAutoCompleteBox.Visible = true;
                            ExistingRoleAutoCompleteBox.Visible = true;
                            btnGrpAdd.Visible = false;
                            btnroleAdd.Visible = false;
                            trGroupGrid.Visible = false;
                            trRoleGrid.Visible = false;

                            Dictionary<string, string> param = new Dictionary<string, string> { { "EndpointUserID", sapepDetails.EndpointUserId.ToString() } };
                            Dictionary<string, string> dataSourceGroup = DomainApiClient<Dto.SAPEPGroupInfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAPEP.SAPEPGroupInfo.GetGroupSAPEPForRemove, param);

                            ExistingGroupAutoCompleteBox.DataSource = dataSourceGroup;
                            ExistingGroupAutoCompleteBox.DataTextField = "Value";
                            ExistingGroupAutoCompleteBox.DataValueField = "Key";
                            ExistingGroupAutoCompleteBox.DataBind();

                            Dictionary<string, string> dataSourceRole = DomainApiClient<Dto.SAPEPRoleInfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAPEP.SAPEPRoleInfo.GetRolesSAPEPForRemove, param);

                            ExistingRoleAutoCompleteBox.DataSource = dataSourceRole;
                            ExistingRoleAutoCompleteBox.DataTextField = "Value";
                            ExistingRoleAutoCompleteBox.DataValueField = "Key";
                            ExistingRoleAutoCompleteBox.DataBind();
                        }
                        else if (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount || adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd)
                        {
                            hdnEndpointUserId.Value = sapepDetails.EndpointUserId.ToString();
                            EndpointSystemId = adhocRequestDetails.EndpointSystemId.Value;
                            GroupsAutoCompleteBox.Visible = true;
                            RolesAutoCompleteBox.Visible = true;
                            ExistingGroupAutoCompleteBox.Visible = false;
                            ExistingRoleAutoCompleteBox.Visible = false;
                            btnGrpAdd.Visible = true;
                            btnroleAdd.Visible = true;
                            trGroupGrid.Visible = true;
                            trRoleGrid.Visible = true;

                            if (!string.IsNullOrWhiteSpace(sapepDetails.Roles))
                            {
                                Dictionary<string, string> param = new Dictionary<string, string> { { "EndpointSystemId", adhocRequestDetails.EndpointSystemId.HasValue ? adhocRequestDetails.EndpointSystemId.ToString() : "0" } };

                                Dictionary<string, string> dataSourceRole = DomainApiClient<Dto.SAPEPRoleInfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAPEP.SAPEPRoleInfo.GetRoleSAPEP, param);
                                string[] roleID = sapepDetails.Roles.Split(',');
                                //string entiltment = "";
                                foreach (string rolekey in roleID)
                                {
                                    if (dataSourceRole.ContainsKey(rolekey))
                                        boundEPRoles(rolekey, dataSourceRole[rolekey]);
                                }
                                //RoleEntitlementLabel.Text = entiltment;
                            }
                            else { trRoleGrid.Visible = false; }

                            if (!string.IsNullOrWhiteSpace(sapepDetails.Groups))
                            {
                                Dictionary<string, string> param = new Dictionary<string, string> { { "EndpointSystemId", adhocRequestDetails.EndpointSystemId.HasValue ? adhocRequestDetails.EndpointSystemId.ToString() : "0" } };

                                Dictionary<string, string> dataSourceGroup = DomainApiClient<Dto.SAPEPGroupInfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAPEP.SAPEPGroupInfo.GetGroupSAPEP, param);
                                string[] groupIds = sapepDetails.Groups.Split(',');
                                //string entiltment = "";
                                foreach (string groupkey in groupIds)
                                {
                                    if (dataSourceGroup.ContainsKey(groupkey))
                                        boundEPGroups(groupkey, dataSourceGroup[groupkey]);
                                    //string role = dataSourceRole.Where(item => item.Key == rolekey).Select(item => item.Value).FirstOrDefault();
                                    //entiltment = entiltment == "" ? role : entiltment + "," + role;
                                }
                            }
                            else { trGroupGrid.Visible = false; }
                        }
                    }
                }
                #endregion

                //bool IsWintelGroup = GenericRequestDetailsControl.IsWintelGroup(task.ActionUserId.Value);
                if (task.IsActionOnlyRequiredTask)
                {
                    btnApprove.Visible = !task.IsActionOnlyRequiredTask;
                    btnReject.Visible = !task.IsActionOnlyRequiredTask;
                    if (!task.IsCompleted)
                    {
                        btnSubmit.Visible = task.IsActionOnlyRequiredTask;
                    }
                    if ((!task.IsCompleted && task.IsActionOnlyRequiredTask) || SharedAccountEditPanel.Visible //|| INHUserInfoUC.Visible 
                        || SharedEditSAP.Visible || PostionPrefixPanel.Visible || OneKeyPanel.Visible || SharedAccountLNSPanel.Visible || SAPEPEntitlementPanel.Visible || SkypeForBusinessPanel.Visible)
                    {
                        btnApprove.Visible = false;
                        btnReject.Visible = true;
                        btnSubmit.Visible = true;
                    }
                    else
                    {
                        btnApprove.Visible = true;
                        btnReject.Visible = true;
                        btnSubmit.Visible = false;
                    }
                }
                //if (IsWintelGroup)
                //{
                //    btnApprove.Text = "Submit";
                //}
                //else
                //{
                //    btnApprove.Text = "Approve";
                //}
            }


            if (Task.IsActionOnlyRequiredTask && (SharedAccountEditPanel.Visible
                   //|| INHUserInfoUC.Visible 
                   || SharedEditSAP.Visible
                   || PostionPrefixPanel.Visible
                   || OneKeyPanel.Visible
                   || SharedAccountLNSPanel.Visible
                   || SkypeForBusinessPanel.Visible
                   || SAPEPEntitlementPanel.Visible))
            {
                btnSubmit.Visible = true;
            }
        }

        private void ShowWarningIfUserHasResponseGroup(AdhocSkypeForBusinessRequest currentAccessRequest, long? epId)
        {
            var param = new Dictionary<string, string>();
            param.Add("userId", currentAccessRequest.EndpointUserId.ToString());
            param.Add("endpointSystemId", epId.ToString());
            var isUserHasResponseGroup = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetByAction<bool>("HasUserHoldAnyResponseGroup", param);
            if (isUserHasResponseGroup)
            {
                divDCCResponseGroup.Visible = true;
                dccRemarksForResponseGroupUser.Text = MasterConfigHelper.SFBWarningMessageForDCCIfResponseGroupExists;
            }

        }

        private void ShowDCCLineUriPrefix()
        {
            var param = new Dictionary<string, string>();
            param.Add("endpointSystemId", this.EndpointSystemId.ToString());
            param.Add("fieldName", "DefaultDCC");
            var field = DomainApiClient<Dto.NProvisioningField>.GetByAction<Dto.NProvisioningField>(DomainApiAction.Ruleset.NProvisioningField.GetByEndpointSystemIdAndFieldName, param);
            if (field != null)
            {
                lblLineUrIPrefix.Text = field.Value;
            }
            this.SkypeForBusinessPanel.Visible = true;
            lblSpecialNumber.Text = "Request for DCC";
            txtDCCExtension.Visible = true;
            ReservedNumberAutoCompleteBox.Visible = false;
        }

        void UpdateTaskWithUserInfo()
        {
            try
            {
                Task.ActionUserId = _sessionStatus.User.Id;
                Task.AssignedTo = _sessionStatus.User.Id;
                Task.AssignedBy = _sessionStatus.User.Id;
                DomainApiClient<Dto.Task>.Save(Task);
            }
            catch (Exception ex)
            {

            }
        }

        private void UpdateAccessRequest(AccessType accessType, bool fromEmployeeEvent = false)
        {
            if (Task.IsGroupTask)
            {
                Task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.GetProvisioningTask, new Dictionary<string, string> { { "id", Task.Id.ToString() } });
                UpdateTaskWithUserInfo();
            }
            //Dictionary<string, string> parameter = new Dictionary<string, string>();
            //parameter.Add("accessType", accessType.ToString());
            //parameter.Add("UserName", _sessionStatus.User.UserName);
            //parameter.Add("TaskId", Task.Id.ToString());
            //parameter.Add("Comments", txtComments.Text);
            //parameter.Add("AccountCreatedNotificationTO", MasterConfigHelper.AccountCreatedNotificationTO.ToString());
            //parameter.Add("conflictRemarks", this.GenericRequestDetailsControl.ConflictRemark != null && this.GenericRequestDetailsControl.ConflictRemark.Count > 0 ? string.Join("<br/>", this.GenericRequestDetailsControl.ConflictRemark) : string.Empty);

            PresentationWorkFlowCriteria wfCriteriaParam = new PresentationWorkFlowCriteria();
            wfCriteriaParam.AccessType = accessType;
            wfCriteriaParam.TaskId = Task.Id;
            wfCriteriaParam.UserName = _sessionStatus.User.UserName;
            wfCriteriaParam.Comments = txtComments.Text;
            wfCriteriaParam.AccountCreatedNotificationTO = MasterConfigHelper.AccountCreatedNotificationTO.ToString();
            if (this.GenericRequestDetailsControl.IsEBSMSSLConflictRemark)
            {
                wfCriteriaParam.ConflictRemarks = this.GenericRequestDetailsControl.MSSLEBSConflictRemark != null && this.GenericRequestDetailsControl.MSSLEBSConflictRemark.Count > 0 ? UtilityMethods.Serialize<List< PresentationConflictRemarks>>(this.GenericRequestDetailsControl.MSSLEBSConflictRemark) : null;
            }
            else
            {
                wfCriteriaParam.ConflictRemarks = this.GenericRequestDetailsControl.ConflictRemark != null && this.GenericRequestDetailsControl.ConflictRemark.Count > 0 ? string.Join("<br/>", this.GenericRequestDetailsControl.ConflictRemark) : string.Empty;
            }
            

            var criteria = DomainApiClient<PresentationWorkflowCriteriaBase>.GetByAction<PresentationWorkflowCriteriaBase>
                                            (DomainApiAction.Workflow.Presentation.PresentationWorkflowCriteriaBase.GetWorkFlowCriteria, null, wfCriteriaParam);



            bool status = DomainApiClient<Object>.GetByAction<bool>
                                                    ("WorkflowController", DomainApiAction.WorkflowProcessor.SubmitWorkflowCriteria, null, criteria);
            if (accessType == AccessType.Revoke)
            {
                if (this.CommonTaskInfoUC.complianceReport != null)
                {
                    List<UserSODComplianceReport> report = this.CommonTaskInfoUC.complianceReport;
                    foreach (UserSODComplianceReport rep in report)
                    {
                        DomainApiClient<UserSODComplianceReport>.DeleteById(rep.Id);
                    }
                }
            }
            if (status)
            {
                //if (fromEmployeeEvent)
                //{
                //    CreateAccessRequests();
                //}
                RedirectToParentPage();
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Action not completed. Please contact administrator for more information.");
            }
        }

        //private void CreateAccessRequests()
        //{
        //    var adEndpoint = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, new Dictionary<string, string> { { "applicationType", Convert.ToString((int)ApplicationType.ADSingaporePowerLocal) } });

        //    #region AD
        //    Dto.AccessRequest adRequest = new Dto.AccessRequest();
        //    adRequest.UserId = eventTrigger.Id;
        //    adRequest.EndpointSystemId = adEndpoint.Id;
        //    adRequest.RequestStatus = AccessRequestStatus.Approved;
        //    adRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
        //    adRequest.RequesterId = _sessionStatus.User.Id;
        //    adRequest.ActivationDate = DateTime.Now;
        //    adRequest.Description = "This AD request is created for one card access";
        //    adRequest.TransactionId = Guid.NewGuid();
        //    adRequest.ExecutionTime = DateTime.Now;
        //    adRequest.Remarks = string.Empty;
        //    adRequest.AccountType = AccountType.User;
        //    adRequest.RequestSource = AccessRequestSource.AdhocRequest;

        //    AdHocAccessRequest adhocrequst = new AdHocAccessRequest();
        //    adhocrequst.RequesteeUserId = eventTrigger.UserId;
        //    adhocrequst.RequesterUserId = _sessionStatus.User.Id;
        //    adhocrequst.EndpointSystemType = EndpointType.ActiveDirectory;
        //    adhocrequst.EndpointSystemId = adEndpoint.Id;
        //    adhocrequst.AdhocActionType = AdHocActionType.NewAccount;
        //    adhocrequst.AccountType = AccountType.User;
        //    adhocrequst.ValidFrom = DateTime.Now;
        //    adhocrequst.Comments = "Non Employee Creation Request";
        //    var adhocRequestId = DomainApiClient<Dto.AdHocAccessRequest>.SaveAndReturnId(adhocrequst);
        //    adRequest.AdHocAccessRequestId = adhocRequestId;
        //    var adRequestId = DomainApiClient<Dto.AccessRequest>.SaveAndReturnId(adRequest);
        //    #endregion

        //    #region OneCard
        //    var oneCardEndpoint = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, new Dictionary<string, string> { { "applicationType", Convert.ToString((int)ApplicationType.OneCard) } });
        //    Dto.AccessRequest onecardRequest = new Dto.AccessRequest();
        //    onecardRequest.UserId = eventTrigger.Id;
        //    onecardRequest.EndpointSystemId = oneCardEndpoint.Id;
        //    onecardRequest.RequestStatus = AccessRequestStatus.Requested;
        //    onecardRequest.ProgressStatus = AccessRequestProgress.Submitted;
        //    onecardRequest.RequesterId = _sessionStatus.User.Id;
        //    onecardRequest.ActivationDate = DateTime.Now;
        //    onecardRequest.Description = "This request is created for one card access";
        //    onecardRequest.TransactionId = Guid.NewGuid();
        //    onecardRequest.ExecutionTime = DateTime.Now;
        //    onecardRequest.Remarks = string.Empty;
        //    onecardRequest.AccountType = AccountType.User;
        //    onecardRequest.RequestSource = AccessRequestSource.AdhocRequest;
        //    onecardRequest.ParentRequestId = adRequestId;
        //    onecardRequest.IsParentRequest = true;

        //    AdHocAccessRequest adhocOneCard = new AdHocAccessRequest();
        //    adhocOneCard.RequesteeUserId = eventTrigger.UserId;
        //    adhocOneCard.RequesterUserId = _sessionStatus.User.Id;
        //    adhocOneCard.EndpointSystemType = EndpointType.OneCard;
        //    adhocOneCard.EndpointSystemId = oneCardEndpoint.Id;
        //    adhocOneCard.AdhocActionType = AdHocActionType.NewAccount;
        //    adhocOneCard.AccountType = AccountType.User;
        //    adhocOneCard.ValidFrom = DateTime.Now;
        //    adhocOneCard.Comments = "Non Employee Creation Request";
        //    var adhocOneCardId = DomainApiClient<Dto.AdHocAccessRequest>.SaveAndReturnId(adhocOneCard);
        //    adRequest.AdHocAccessRequestId = adhocRequestId;
        //    var onecardRequestId = DomainApiClient<Dto.AccessRequest>.SaveAndReturnId(onecardRequest);
        //    #endregion
        //}

        private void Refresh()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            if (this.ContextObjectId != null)
                parameter.Add("id", this.ContextObjectId.Value.ToString());
            else
                parameter.Add("id", Request.QueryString["Id"].ToString());

            Dto.Task task = null;
            if (Task == null)
            {
                long queryStrinValue = 0;
                if (Request.QueryString["Id"] != null)
                {
                    queryStrinValue = Convert.ToInt64(Request.QueryString["Id"].ToString());
                    if (queryStrinValue > 0 && this.ContextObjectId != queryStrinValue)
                    {
                        parameter.Clear();
                        parameter.Add("id", queryStrinValue.ToString());
                    }
                }

                task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.GetProvisioningTask, parameter);
                if (task != null)
                {
                    Task = task;
                }
            }

            if (Task != null)
            {
                if (Task.AccessRequest != null && (Task.AccessRequest.RequestStatus == AccessRequestStatus.Withdrawn || Task.AccessRequest.RequestStatus == AccessRequestStatus.Rejected))
                {
                    btnSubmit.Visible = false;
                    btnApprove.Visible = false;
                    btnReject.Visible = false;
                }
                InfoUC.AdhocAccessRequest = Task.AccessRequest.AdHocAccessRequest;
                //if (!this.MasterPage.DisableImageButton && Task != null && Task.AccessRequest != null)
                //{
                //    //btnSendNotification.Visible = btnApprove.Visible = btnReject.Visible = IsAdminMode == IsAdminMode.Yes ? false : (Task.AccessRequest.ProgressStatus == AccessRequestProgress.WaitingforApproval
                //    //) ? true : false;
                //    //btnSendNotification.Enabled = UCSendNotification.IsSendNotificationRequired();
                //    //btnSendNotification.Visible = false;
                //}
            }
        }
        private void RedirectToParentPage()
        {
            _sessionStatus = new SessionStatusImpl();
            if (_sessionStatus.RedirectFrom == "MyTask")
                Response.Redirect("~/Task/List.aspx");
            else if (_sessionStatus.RedirectFrom == "Home")
                Response.Redirect("~/Home/Home.aspx");
            else if (_sessionStatus.RedirectFrom == "GroupTask")
                Response.Redirect("GroupTasks.aspx");
            else if (_sessionStatus.RedirectFrom == "MyNotifications")
                Response.Redirect("~/Notification/Detail.aspx?Id=" + Request.QueryString["NotifyId"].ToString());
            else if (_sessionStatus.RedirectFrom == "JAC")
                Response.Redirect("~/JobAllocationCenter/List.aspx");
            else if (_sessionStatus.RedirectFrom == "JACReport")
                Response.Redirect("~/Report/User/JobAllocationCenterReport.aspx");
            else if (_sessionStatus.RedirectFrom == "Tasks")
                Response.Redirect("~/Prov/Task/List.aspx");
            else
                Response.Redirect("~/Task/List.aspx");
        }


        private void SendNotificationDisplay(bool IsDisplay)
        {
            UCSendNotification.ClearControls();
            RadWinSendNotification.VisibleOnPageLoad = IsDisplay;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            ConfirmMessage1.ConfirmYesClick += ConfirmMessage1_ConfirmYesClick;
            if (!IsPostBack)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                if (this.ContextObjectId != null)
                    parameter.Add("id", this.ContextObjectId.Value.ToString());
                else
                    parameter.Add("id", Request.QueryString["Id"].ToString());

                Dto.Task task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.GetProvisioningTask, parameter);

                var loginId = new SessionStatusImpl().UserId;

                //if (!(task.ActionUserId == loginId || task.AssignedTo == loginId))
                //{
                //    RedirectToParentPage();
                //}

                this.Task = task;


                MasterPage mstr = this.Parent.Page.Master as MasterPage;
                (mstr.FindControl("lblPageTitle") as Label).Text = "Task Details";
                if (task.IsGroupTask)
                {
                    (mstr.FindControl("lblPageTitle") as Label).Text = "Group Task Details";
                }

                parameter.Clear();
                parameter.Add("id", this.Task.WorkflowConfigurationId.ToString());
                var workFlowCOnfigurationDetails = DomainApiClient<Dto.WorkflowConfiguration>.GetByAction<Dto.WorkflowConfiguration>
                    (DomainApiAction.Workflow.WorkflowConfiguration.GetWorkflowConfigurationDetailById, parameter);
                workFlowConfig = workFlowCOnfigurationDetails;


                long sessionUser = new SessionStatusImpl().UserId;
                Dictionary<string, string> ssnUser = new Dictionary<string, string>();
                ssnUser.Add("actionUserId", sessionUser.ToString());
                List<long> userGroups = DomainApiClient<Dto.User>.GetByAction<List<long>>(DomainApiAction.User.GetGroupIds, ssnUser);

                if (this.Task.Action != StatusType.Pending)
                    this.MasterPage.DisableImageButton = true;
                else
                {
                    if (task.AssignedTo == sessionUser)
                        this.MasterPage.DisableImageButton = false;
                    else if ((userGroups.Contains((int)(this.Task.ActionGroupId.HasValue ? this.Task.ActionGroupId.Value : 0)) && this.Task.IsGroupTask && task.AssignedTo == null))
                        this.MasterPage.DisableImageButton = false;
                    else if ((task.ActionUserId == sessionUser && !task.IsGroupTask) || ((task.IsGroupTask && task.AssignedTo == sessionUser)))
                        this.MasterPage.DisableImageButton = false;
                    if (!CanApprove(this.Task) && IsAdminMode == IsAdminMode.No)
                    {
                        Response.Redirect("~/Login.aspx");
                    }
                }

                if (task.EmployeeEventTriggerId > 0) //Non Employee Creation
                {
                    InfoUC.EmployeeEventTask = task;
                    NoteInfoUC.userId = eventTrigger.UserId;
                    NoteInfoUC.endpointId = 1;
                    NoteInfoUC.Initialize();
                    txtComments.Text = task.Comments;
                    DelegatedBytr.Visible = task.IsDelegated;
                    DelegatedFromtr.Visible = task.IsDelegated;
                    DelegationCommentstr.Visible = task.IsDelegated;
                    SetDelegationInfo();
                    SetApproverForm();
                    SetOtherInfo(eventTrigger);
                    User newUser = null;
                    var user = DomainApiClient<Dto.User>.GetById(eventTrigger.UserId);
                    if (eventTrigger.EventType == EmployeeEvents.NonEmployeeExtension)
                    {
                        newUser = UtilityMethods.Deserialize<User>(eventTrigger.UserDetails);
                        user.TerminationDate = newUser.TerminationDate;
                    }
                    GenericRequestDetailsControl.SetNonEmployeeInfo(newUser != null ? newUser : user, eventTrigger);
                    this.MasterPage.ActionClicked += ActionClicked;
                    return;
                }

                if (task.AccessRequest == null)
                    return;
                InfoUC.AdhocAccessRequest = Task.AccessRequest.AdHocAccessRequest;
                CommonTaskInfoUC.AdhocAccessRequest = Task.AccessRequest.AdHocAccessRequest;
                CommonTaskInfoUC.SetAdhocRequest(Task.AccessRequest.AdHocAccessRequest);
                GenericRequestDetailsControl.AdHocRequestId = Task.AccessRequest.AdHocAccessRequest.Id;
                NoteInfoUC.userId = Task.AccessRequest.UserId;
                NoteInfoUC.endpointId = Task.AccessRequest.EndpointSystemId.Value;
                NoteInfoUC.Initialize();
                LoadApplicationDetail();
                txtComments.Text = Task.Comments;
                DelegatedBytr.Visible = task.IsDelegated;
                DelegatedFromtr.Visible = task.IsDelegated;
                DelegationCommentstr.Visible = task.IsDelegated;
                UCSendNotification.BindNotifiableUsers(this.Task.AccessRequestId);


                SetDelegationInfo();
                SetOtherInfo();

                //if (task.AccessRequest != null && task.AccessRequest.AdHocAccessRequest != null)
                //{
                //    var request = task.AccessRequest.AdHocAccessRequest;
                //    if (request.ApplicationType == ApplicationType.ADSingaporePowerLocal
                //       //|| request.ApplicationType == ApplicationType.ADSDCLocal
                //       || request.ApplicationType == ApplicationType.SAPLocal)
                //    {
                //        GenericRequestDetailsControl.BindADGroupInfo(task);
                //    }
                //}


                SetApproverForm();

            }
            this.MasterPage.ActionClicked += ActionClicked;

        }

        void SetOtherInfo(Dto.EmployeeEventTrigger eventTrigger = null)
        {
            if (!this.MasterPage.DisableImageButton)
            {

                //btnSendNotification.Enabled = UCSendNotification.IsSendNotificationRequired();
                //btnSendNotification.Visible = false;
                if (btnApprove.Visible)
                {
                    if (Task.IsGroupTask && (Task.AssignedTo != null && Task.AssignedTo != _sessionStatus.User.Id))
                    {
                        //btnSendNotification.Visible = false;
                        this.MasterPage.FindControl("AccessPanel").Visible = false;
                    }
                    else
                    {
                        this.txtComments.Enabled = true;
                    }

                    if (Task.IsActionOnlyRequiredTask)
                    {
                        btnApprove.Enabled = false;
                    }
                }
                else
                {
                    //this.txtComments.Enabled = false;
                    this.MasterPage.FindControl("AccessPanel").Visible = false;
                }
            }
            else
            {
                this.txtComments.Enabled = this.btnApprove.Visible = btnReject.Visible = btnSubmit.Visible = false;
                //btnSendNotification.Visible = false;
            }

            if (IsAdminMode == IsAdminMode.Yes)
            {
                this.txtComments.Enabled = this.btnApprove.Visible = btnReject.Visible = btnSubmit.Visible = false;
            }

            if (eventTrigger != null)
                btnSubmit.Visible = Task.IsActionOnlyRequiredTask;
        }

        void SetDelegationInfo()
        {
            if (Task.IsDelegated)
            {
                User _user = null;
                if (Task.DelegatedFrom != null && Task.DelegatedFrom.Value > 0)
                {
                    _user = DomainApiClient<User>.GetById(Task.DelegatedFrom.Value);

                    if (_user != null)
                        lblDelegatedFrom.Text = _user.DisplayName;
                    else
                    {
                        lblDelegatedFrom.Text = "-";
                    }
                }
                else
                {
                    lblDelegatedFrom.Text = "-";
                }
                _user = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetByUserName, new Dictionary<string, string> { { "name", Task.DelegatedBy } });
                    if (_user != null)
                        lblDelegatedBy.Text = _user.DisplayName;
                    else
                        lblDelegatedBy.Text = Task.DelegatedBy;

                    lblDelegationComments.Text = Task.DelegationComments?.Replace("\n", "<br>");
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            var AccessPanel = this.MasterPage.FindControl("AccessPanel");
            if (AccessPanel != null && AccessPanel.FindControl("rptPermissions") != null)
            {
                var Repeater = (Repeater)AccessPanel.FindControl("rptPermissions");
                foreach (RepeaterItem item in Repeater.Items)
                {
                    Panel Panel = item.FindControl("PermissionPanel") as Panel;
                    if (Panel != null)
                    {
                        if (Task.AccessRequest != null && (Task.AccessRequest.ProgressStatus != AccessRequestProgress.WaitingforApproval) &&
                                ((Panel.ToolTip == Convert.ToString(ACLAction.Reject) || Panel.ToolTip == Convert.ToString(ACLAction.Approve))))
                        {
                            Panel.Enabled = false;
                        }
                        else if (Task.IsGroupTask && (Task.AssignedTo != null && Task.AssignedTo != _sessionStatus.User.Id))
                        {
                            Panel.Enabled = false;
                        }
                    }
                }
            }
            base.OnPreRender(e);
        }

        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Approve)
            {
                btnApprove_Click(null, null);
            }
            else if (e.ActionType == ACLAction.Reject)
            {
                btnReject_Click(null, null);
            }
        }

        Dto.Task GetTask()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            string Id = string.Empty;
            if (Request.QueryString["Id"] == null)
            {
                if (Request.QueryString["contextId"] != null)
                    Id = Request.QueryString["contextId"].ToString();
            }
            else
            {
                Id = Request.QueryString["Id"].ToString();
            }
            parameter.Add("id", Id);
            Dto.Task task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.GetProvisioningTask, parameter);
            return task;
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            //Provisioning_InhouseUserInfoUC UC = new Provisioning_InhouseUserInfoUC();

            if (Task.EmployeeEventTriggerId > 0)
            {
                UpdateTask(AccessType.Approve);
                UpdateAccessRequest(AccessType.Approve, true);
                return;
            }
            var task = GetTask();

            if (task.AccessRequest == null || task.AccessRequest.AdHocAccessRequest == null) return;

            if (task.AccessRequest.RequestStatus == AccessRequestStatus.Withdrawn)
            {
                ShowAlertMessage(AlertType.Error, "Request has been withdrawn.You cannot submit the task");
                return;
            }
            if (task.AccessRequest.RequestStatus == AccessRequestStatus.Rejected)
            {
                ShowAlertMessage(AlertType.Error, "Request has been Rejected.You cannot submit the task");
                return;
            }
            Dto.AdHocAccessRequest adhocRequestDetails = task.AccessRequest.AdHocAccessRequest;
            ApplicationType applicationType = adhocRequestDetails.ApplicationType;

            if (applicationType == ApplicationType.InhouseSystems)
            {
                if (task.IsActionOnlyRequiredTask &&
                    (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount &&
                    (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary)))
                {
                    string message = InhouseTaskInfoUC.Validate();
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        ShowAlertMessage(AlertType.Error, message);
                        return;
                    }
                    else
                    {
                        string validationMsg = string.Empty;
                        long endpointId = adhocRequestDetails.EndpointSystemId.HasValue ? adhocRequestDetails.EndpointSystemId.Value : 0;
                        if (AdhocAccessRequestValidation.ValidateAccountName(InhouseTaskInfoUC.Username, endpointId, out validationMsg, adhocRequestDetails.RequesteeUserId, adhocRequestDetails.AccountType))
                        {
                            var adHocADRequest = (AdHocINHRequest)adhocRequestDetails.GetEntitlementDetail();
                            adHocADRequest.AdHocINHSharedAccount.AccountName = InhouseTaskInfoUC.Username;
                            adHocADRequest.AdHocINHSharedAccount.DisplayName = InhouseTaskInfoUC.DisplayName;
                            adHocADRequest.AdHocINHSharedAccount.Password = RSAHelper.EncryptData(InhouseTaskInfoUC.Password);
                            adHocADRequest.Entitlement = InhouseTaskInfoUC.GetEntitlementString();
                            adhocRequestDetails.EntitlementDetail = adhocRequestDetails.GetSerializedString(adHocADRequest);
                            DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocRequestDetails);
                            this.UpdateAccessRequest(AccessType.Approve);
                        }
                        else
                        {
                            ShowAlertMessage(AlertType.Error, validationMsg);
                            return;
                        }
                    }
                }
                else
                {
                    string validationMsg = string.Empty;
                    long endpointId = adhocRequestDetails.EndpointSystemId.HasValue ? adhocRequestDetails.EndpointSystemId.Value : 0;
                    this.InhouseTaskInfoUC.ActionType = adhocRequestDetails.AdhocActionType;

                    if (adhocRequestDetails.AdhocActionType != AdHocActionType.DeleteAccount && InhouseTaskInfoUC.EntitlementRequest.Count == 0 && task.AccessRequest.RequestSource != AccessRequestSource.UndoEvent && task.AccessRequest.RequestSource != AccessRequestSource.NonEmployeeExtension)
                    {
                        ShowAlertMessage(AlertType.Error, "Please select entitlement.");
                        return;
                    }

                    string message = InhouseTaskInfoUC.Validate();
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        ShowAlertMessage(AlertType.Error, message);
                        return;
                    }

                    bool userCreated = true;
                    InhouseTaskInfoUC.Task = task;
                    InhouseTaskInfoUC.RequestSource = task.AccessRequest.RequestSource;

                    AdHocINHRequest INHRequest = null;
                    if (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && task.AccessRequest.RequestSource != AccessRequestSource.UndoEvent)
                        userCreated = InhouseTaskInfoUC.CreateINHUser();
                    else
                    {
                        INHRequest = (AdHocINHRequest)adhocRequestDetails.GetEntitlementDetail();
                        if (INHRequest != null)
                        {
                            InhouseTaskInfoUC.EndPointUserId = INHRequest.EndpointUserId;
                            InhouseTaskInfoUC.IsOffBoardRequest = INHRequest.IsOffBoardRequest;
                        }
                    }

                    if (userCreated)
                    {
                        if (INHRequest == null)
                        {
                            INHRequest = InhouseTaskInfoUC.GetEntitlementDetail();
                        }

                        INHRequest.Entitlement = InhouseTaskInfoUC.GetEntitlementString();

                        adhocRequestDetails.EntitlementDetail = adhocRequestDetails.GetSerializedString(INHRequest);
                        DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocRequestDetails);
                        this.UpdateAccessRequest(AccessType.Approve);
                    }
                }
            }
            if (task.IsActionOnlyRequiredTask && (applicationType == ApplicationType.GEMS) && (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service)))
            {
                string Message = "Please enter";
                string ControlMessage = "";
                if (string.IsNullOrWhiteSpace(txtAccountName.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AccountName" : ControlMessage + ", AccountName";
                }
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Email Id" : ControlMessage + " , Email Id";
                }
                if (string.IsNullOrWhiteSpace(txtDisplayNameShared.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Display Name" : ControlMessage + " , Display Name";
                }
                if (ControlMessage != "")
                {
                    Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
                    ShowAlertMessage(AlertType.Error, Message);
                    return;
                }
                else
                {
                    if (ValidateAccountName(txtAccountName.Text, adhocRequestDetails.EndpointSystemType, adhocRequestDetails.EndpointSystemId))
                    {

                        var adhocAcessRequest = task.AccessRequest.AdHocAccessRequest;
                        var adHocADRequest = (AdHocGEMSRequest)adhocAcessRequest.GetEntitlementDetail();
                        adHocADRequest.AdHocGEMSSharedAccount.AccountName = txtAccountName.Text;
                        adHocADRequest.AdHocGEMSSharedAccount.EmailAddress = txtEmail.Text;
                        adHocADRequest.AdHocGEMSSharedAccount.DisplayName = txtDisplayNameShared.Text;
                        adHocADRequest.AdHocGEMSSharedAccount.Description = txtDescriptionShared.Text;
                        adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(adHocADRequest);
                        DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                        this.UpdateAccessRequest(AccessType.Approve);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            else if (task.IsActionOnlyRequiredTask && (adhocRequestDetails.ApplicationType == ApplicationType.Office365) && (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary)))
            {
                string Message = "Please enter";
                string ControlMessage = "";
                if (string.IsNullOrWhiteSpace(txtboxAliasName.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AliasName" : ControlMessage + ", AliasName";
                }
                if (string.IsNullOrWhiteSpace(txtboxDisplayName.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Display Name" : ControlMessage + " , Display Name";
                }
                //if (adhocRequestDetails.AccountType == AccountType.Shared && sharedAccountO365NewAccountPanel.Visible && string.IsNullOrWhiteSpace(txtEntityId.Text))
                //{
                //    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Entity Id" : ControlMessage + " , Entity Id";
                //}
                if (ControlMessage != "")
                {
                    Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
                    ShowAlertMessage(AlertType.Error, Message);
                    return;
                }
                else
                {
                    if (ValidateAccountName(txtboxAliasName.Text, adhocRequestDetails.EndpointSystemType, adhocRequestDetails.EndpointSystemId))
                    {

                        var adhocAcessRequest = task.AccessRequest.AdHocAccessRequest;
                        var adhocOffice365Request = (AdHocO365Request)adhocAcessRequest.GetEntitlementDetail();
                        adhocOffice365Request.AdHocO365SharedAccount.DisplayName = txtboxDisplayName.Text;
                        //adhocOffice365Request.AdHocO365SharedAccount.AliasName = txtboxAliasName.Text;

                        var accessRequest = task.AccessRequest;
                        accessRequest.ConflictsRemarks = lblRequesterCostCenter.Text;
                        //accessRequest.AdditionalInfo = txtEntityId.Text;

                        adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(adhocOffice365Request);
                        //DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                        DomainApiClient<Dto.AccessRequest>.Save(accessRequest);
                        this.UpdateAccessRequest(AccessType.Approve);

                    }
                }
            }
            else if (task.IsActionOnlyRequiredTask && (adhocRequestDetails.ApplicationType == ApplicationType.Office365) && (adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequestDetails.AdhocActionType == AdHocActionType.DeleteAccount)
                )
            {
                var adhocAcessRequest = task.AccessRequest.AdHocAccessRequest;
                var adhocOffice365Request = (AdHocO365Request)adhocAcessRequest.GetEntitlementDetail();
                var accessRequest = task.AccessRequest;
                accessRequest.ConflictsRemarks = lblRequesterCostCenter.Text;
                adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(adhocOffice365Request);
                DomainApiClient<Dto.AccessRequest>.Save(accessRequest);
                this.UpdateAccessRequest(AccessType.Approve);
            }
            else if (task.IsActionOnlyRequiredTask && (adhocRequestDetails.ApplicationType == ApplicationType.SRM) && (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount && (adhocRequestDetails.AccountType == AccountType.Shared || adhocRequestDetails.AccountType == AccountType.Service || adhocRequestDetails.AccountType == AccountType.Secondary)))
            {

                string Message = "Please enter";
                string ControlMessage = "";
                if (string.IsNullOrWhiteSpace(txtSRMAccountName.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AccountName" : ControlMessage + ", AccountName";
                }

                if (ControlMessage != "")
                {
                    Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
                    ShowAlertMessage(AlertType.Error, Message);
                    return;
                }
                else
                {
                    if (SRPAccountValidation(task.AccessRequest.AdHocAccessRequest, txtSRMAccountName.Text))
                    {

                        var adhocAcessRequest = task.AccessRequest.AdHocAccessRequest;
                        var adhocSRMRequest = (AdHocSAPSRMRequest)adhocAcessRequest.GetEntitlementDetail();
                        adhocSRMRequest.AdHocSAPSRMSharedAccount.AccountName = txtboxDisplayName.Text;
                        adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(adhocSRMRequest);
                        DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                        this.UpdateAccessRequest(AccessType.Approve);
                    }

                }
            }


            if (applicationType == ApplicationType.FLINS_BI)
            {
                string Message = "Please enter";
                string ControlMessage = "";
                if (SAPUserTypeDropDown.SelectedIndex <= 0)
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " SAP UserType" : ControlMessage + ", SAP UserType";
                }
                if (string.IsNullOrWhiteSpace(txtAccountNameSAP.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AccountName" : ControlMessage + ", AccountName";
                }
                if (ControlMessage != "")
                {
                    Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
                    ShowAlertMessage(AlertType.Warning, Message);
                    return;
                }
                else
                {
                    if (ValidateAccountName(txtAccountNameSAP.Text, adhocRequestDetails.EndpointSystemType, adhocRequestDetails.EndpointSystemId))
                    {

                        var adHocRequest = (AdHocSAPFLINSBIRequest)adhocRequestDetails.GetEntitlementDetail();
                        adHocRequest.AdHocFLINSBISharedAccount.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                        adHocRequest.AdHocFLINSBISharedAccount.AccountName = txtAccountNameSAP.Text;
                        adHocRequest.AdHocFLINSBISharedAccount.Description = txtDescriptionSAP.Text;
                        adhocRequestDetails.EntitlementDetail = adhocRequestDetails.GetSerializedString(adHocRequest);
                        DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocRequestDetails);
                        this.UpdateAccessRequest(AccessType.Approve);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (adhocRequestDetails.ApplicationType == ApplicationType.HRMS_FLINS)
            {
                string Message = "Please enter";
                string ControlMessage = "";
                if (SAPUserTypeDropDown.SelectedIndex <= 0)
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " SAP UserType" : ControlMessage + ", SAP UserType";
                }
                if (string.IsNullOrWhiteSpace(txtAccountNameSAP.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AccountName" : ControlMessage + ", AccountName";
                }
                if (ControlMessage != "")
                {
                    Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;

                    ShowAlertMessage(AlertType.Error, Message);
                    return;
                }
                else
                {
                    if (ValidateAccountName(txtAccountNameSAP.Text, adhocRequestDetails.EndpointSystemType, adhocRequestDetails.EndpointSystemId))
                    {

                        var adhocAcessRequest = adhocRequestDetails;
                        var adHocRequest = (AdHocSAPHRFLINSRequest)adhocAcessRequest.GetEntitlementDetail();
                        adHocRequest.AdHocSAPHRFLINSSharedAccount.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                        adHocRequest.AdHocSAPHRFLINSSharedAccount.AccountName = txtAccountNameSAP.Text;
                        adHocRequest.AdHocSAPHRFLINSSharedAccount.Description = txtDescriptionSAP.Text;
                        adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(adHocRequest);
                        DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                        this.UpdateAccessRequest(AccessType.Approve);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (adhocRequestDetails.ApplicationType == ApplicationType.SAP_GRC)
            {
                string Message = "Please enter";
                string ControlMessage = "";
                if (SAPUserTypeDropDown.SelectedIndex <= 0)
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " SAP UserType" : ControlMessage + ", SAP UserType";
                }
                if (string.IsNullOrWhiteSpace(txtAccountNameSAP.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AccountName" : ControlMessage + ", AccountName";
                }
                if (ControlMessage != "")
                {
                    Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;

                    ShowAlertMessage(AlertType.Error, Message);
                    return;
                }
                else
                {
                    if (ValidateAccountName(txtAccountNameSAP.Text, adhocRequestDetails.EndpointSystemType, adhocRequestDetails.EndpointSystemId))
                    {

                        var adhocAcessRequest = adhocRequestDetails;
                        var adHocRequest = (AdHocSAPGRCRequest)adhocAcessRequest.GetEntitlementDetail();
                        adHocRequest.AdHocSAPGRCSharedAccount.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                        adHocRequest.AdHocSAPGRCSharedAccount.AccountName = txtAccountNameSAP.Text;
                        adHocRequest.AdHocSAPGRCSharedAccount.Description = txtDescriptionSAP.Text;
                        adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(adHocRequest);
                        DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                        this.UpdateAccessRequest(AccessType.Approve);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (adhocRequestDetails.ApplicationType == ApplicationType.SAP_PI)
            {
                string Message = "Please enter";
                string ControlMessage = "";
                if (SAPUserTypeDropDown.SelectedIndex <= 0)
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " SAP UserType" : ControlMessage + ", SAP UserType";
                }
                if (string.IsNullOrWhiteSpace(txtAccountNameSAP.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AccountName" : ControlMessage + ", AccountName";
                }
                if (ControlMessage != "")
                {
                    Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
                    ShowAlertMessage(AlertType.Error, Message);
                    return;
                }
                else
                {
                    if (ValidateAccountName(txtAccountNameSAP.Text, adhocRequestDetails.EndpointSystemType, adhocRequestDetails.EndpointSystemId))
                    {

                        var adhocAcessRequest = adhocRequestDetails;
                        var adHocRequest = (AdHocSAPPIRequest)adhocAcessRequest.GetEntitlementDetail();
                        adHocRequest.AdHocSAPPISharedAccount.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                        adHocRequest.AdHocSAPPISharedAccount.AccountName = txtAccountNameSAP.Text;
                        adHocRequest.AdHocSAPPISharedAccount.Description = txtDescriptionSAP.Text;
                        adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(adHocRequest);
                        DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                        this.UpdateAccessRequest(AccessType.Approve);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (adhocRequestDetails.ApplicationType == ApplicationType.MSSL_ISU || adhocRequestDetails.ApplicationType == ApplicationType.MSSL_CRM || //adhocRequestDetails.ApplicationType == ApplicationType.MSSL_BI || 
                adhocRequestDetails.ApplicationType == ApplicationType.EBS_ISU || adhocRequestDetails.ApplicationType == ApplicationType.EBS_CRM // ||  adhocRequestDetails.ApplicationType == ApplicationType.EBS_BI
                )
            {
                string Message = "Please enter";
                string ControlMessage = "";
                if (SAPUserTypeDropDown.SelectedIndex <= 0 && SharedEditSAP.Visible)
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " SAP UserType" : ControlMessage + ", SAP UserType";
                }
                if (string.IsNullOrWhiteSpace(txtAccountNameSAP.Text) && SharedEditSAP.Visible)
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AccountName" : ControlMessage + ", AccountName";
                }
                if (PostionPrefixPanel.Visible)
                {
                    if (string.IsNullOrWhiteSpace(txtPrefixValue.Text))
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " prefix" : ControlMessage + ", prefix";
                    else if (txtPrefixValue.Text.Length != 4)
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " a valid prefix" : ControlMessage + ", a valid prefix";
                    else if (!Regex.IsMatch(txtPrefixValue.Text, "^[A-Za-z]+$"))
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " a valid prefix" : ControlMessage + ", a valid prefix";
                }
                if (ControlMessage != "")
                {
                    Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;

                    ShowAlertMessage(AlertType.Error, Message);
                    return;
                }
                else
                {
                    bool IsValideAccountName = true;
                    if (SharedEditSAP.Visible)
                    {
                        IsValideAccountName = ValidateAccountName(txtAccountNameSAP.Text, adhocRequestDetails.EndpointSystemType, adhocRequestDetails.EndpointSystemId);
                    }
                    if (IsValideAccountName)
                    {
                        var adhocAcessRequest = adhocRequestDetails;
                        if (adhocRequestDetails.ApplicationType == ApplicationType.MSSL_ISU ||
                            adhocRequestDetails.ApplicationType == ApplicationType.MSSL_CRM // || adhocRequestDetails.ApplicationType == ApplicationType.MSSL_BI
                            )
                        {
                            var mssldetail = (AdHocMSSLRequest)adhocAcessRequest.GetEntitlementDetail();
                            if (mssldetail != null)
                            {
                                if (PostionPrefixPanel.Visible)
                                {
                                    mssldetail.PositionPrefix = txtPrefixValue.Text;
                                }
                                else
                                {
                                    mssldetail.AdHocSAPMSSLSharedAccount.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                                    mssldetail.AdHocSAPMSSLSharedAccount.AccountName = txtAccountNameSAP.Text;
                                    mssldetail.AdHocSAPMSSLSharedAccount.Description = txtDescriptionSAP.Text;
                                }
                                adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(mssldetail);
                            }
                        }
                        else
                        {
                            var ebsdetail = (AdHocEBSRequest)adhocAcessRequest.GetEntitlementDetail();
                            if (ebsdetail != null)
                            {
                                if (PostionPrefixPanel.Visible)
                                {
                                    ebsdetail.PositionPrefix = txtPrefixValue.Text;
                                }
                                else
                                {
                                    ebsdetail.AdHocSAPEBSSharedAccount.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                                    ebsdetail.AdHocSAPEBSSharedAccount.AccountName = txtAccountNameSAP.Text;
                                    ebsdetail.AdHocSAPEBSSharedAccount.Description = txtDescriptionSAP.Text;
                                }
                                adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(ebsdetail);
                            }
                        }
                        DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                        this.UpdateAccessRequest(AccessType.Approve);

                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (adhocRequestDetails.ApplicationType == ApplicationType.MSSL || adhocRequestDetails.ApplicationType == ApplicationType.EBS)
            {
                string Message = "Please enter";
                string ControlMessage = "";
                if (SAPUserTypeDropDown.SelectedIndex <= 0 && SharedEditSAP.Visible)
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " SAP UserType" : ControlMessage + ", SAP UserType";
                }
                if (string.IsNullOrWhiteSpace(txtAccountNameSAP.Text) && SharedEditSAP.Visible)
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AccountName" : ControlMessage + ", AccountName";
                }
                if (PostionPrefixPanel.Visible)
                {
                    if (string.IsNullOrWhiteSpace(txtPrefixValue.Text))
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " prefix" : ControlMessage + ", prefix";
                    else if (txtPrefixValue.Text.Length != 4)
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " a valid prefix" : ControlMessage + ", a valid prefix";
                    else if (!Regex.IsMatch(txtPrefixValue.Text, "^[A-Za-z]+$"))
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " a valid prefix" : ControlMessage + ", a valid prefix";
                }
                if (ControlMessage != "")
                {
                    Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;

                    ShowAlertMessage(AlertType.Error, Message);
                    return;
                }
                else
                {
                    bool IsValideAccountName = true;
                    if (SharedEditSAP.Visible)
                    {
                        IsValideAccountName = ValidateAccountName(txtAccountNameSAP.Text, adhocRequestDetails.EndpointSystemType, adhocRequestDetails.EndpointSystemId);
                    }
                    if (IsValideAccountName)
                    {
                        var adhocAcessRequest = adhocRequestDetails;
                        var ebsdetail = (AdHocSAPHANARequest)adhocAcessRequest.GetEntitlementDetail();
                        if (ebsdetail != null)
                        {
                            if (PostionPrefixPanel.Visible)
                            {
                                ebsdetail.PositionPrefix = txtPrefixValue.Text;
                            }
                            else
                            {
                                ebsdetail.AdHocSAPHANASharedAccount.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                                ebsdetail.AdHocSAPHANASharedAccount.AccountName = txtAccountNameSAP.Text;
                                ebsdetail.AdHocSAPHANASharedAccount.Description = txtDescriptionSAP.Text;
                            }
                            adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(ebsdetail);
                        }
                        DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                        this.UpdateAccessRequest(AccessType.Approve);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            if (adhocRequestDetails.ApplicationType == ApplicationType.VPN)
            {
                var vpnDetail = (AdHocVPNRequest)adhocRequestDetails.GetEntitlementDetail();
                var oneKeyToken = txtOneKey.Text.Trim();
                if (((adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount)
                    || (adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd && vpnDetail.UpdateOneKeyToken)) &&
                    string.IsNullOrWhiteSpace(txtOneKey.Text))
                {
                    ShowAlertMessage(AlertType.Error, "Please Enter the Token Number");
                    return;
                }
                User user = DomainApiClient<Dto.User>.GetById(adhocRequestDetails.RequesteeUserId);

                if (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount || adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd)
                {

                    if (!string.IsNullOrWhiteSpace(oneKeyToken))
                    {
                        var errormsg = $"Token Number:{oneKeyToken} already exists. Please enter valid Token Number.";
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        if (vpnDetail.UpdateOneKeyToken)
                        {
                            if (!string.IsNullOrWhiteSpace(user.OneKeyTokenId) && user.OneKeyTokenId.Equals(oneKeyToken, StringComparison.InvariantCultureIgnoreCase))
                            {
                                ShowAlertMessage(AlertType.Error, $"Token Number:{oneKeyToken} should not be same as Current Token Number");
                                return;
                            }
                            parameter.Add("userid", adhocRequestDetails.RequesteeUserId.ToString());
                        }
                        else
                        {
                            parameter.Add("userid", adhocRequestDetails.RequesteeUserId.ToString());
                        }
                        parameter.Add("accountName", oneKeyToken);
                        parameter.Add("EndpointSystemId", adhocRequestDetails.EndpointSystemId.ToString());
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>
                        (DomainApiAction.Provisioning.AccountReconcilation.DoesAccountExistInAccountRecon, parameter);
                        if (accountRecon != null && accountRecon.IsDeleted == false)
                        {
                            ShowAlertMessage(Infrastructure.AlertType.Error, errormsg);
                            return;
                        }
                        vpnDetail.OneKey = oneKeyToken;
                    }
                }
                else if (adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentRemove)
                {

                }
                adhocRequestDetails.EntitlementDetail = adhocRequestDetails.GetSerializedString(vpnDetail);
                DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocRequestDetails);
                this.UpdateAccessRequest(AccessType.Approve);

            }
            else if (adhocRequestDetails.ApplicationType == ApplicationType.MicrosoftTeams)
            {

                //TODO:sivanandan
                if (adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    var sfbdetail = (AdhocSkypeForBusinessRequest)adhocRequestDetails.GetEntitlementDetail();
                    if (sfbdetail.EntitlementType == EntitlementType.Default)
                    {
                        if (ReservedNumberAutoCompleteBox.Entries.Count == 0)
                        {
                            ShowAlertMessage(AlertType.Error, "Please select Extension");
                            return;
                        }
                        var extension = ReservedNumberAutoCompleteBox.Entries[0].Value;
                        var validExtension = ValidateExtension(extension);
                        if (validExtension == -1)
                        {
                            return;
                        }
                        sfbdetail.Extension = validExtension.ToString();
                        sfbdetail.LineUri = $"{lblLineUrIPrefix.Text.Replace(" ", "")}{validExtension}";
                    }
                    else if (sfbdetail.EntitlementType == EntitlementType.DCC)
                    {
                        //var extension = txtDCCExtension.Text;
                        //var validExtension = ValidateExtension(extension);
                        //if (validExtension == -1)
                        //{
                        //    return;
                        //}
                        ////check for exisiting 
                        //var param = new Dictionary<string, string>();
                        //param.Add("endpointId", adhocRequestDetails.EndpointSystemId.ToString());
                        //param.Add("number", validExtension.ToString());
                        //param.Add("accessRequestId", task.AccessRequest.Id.ToString());
                        //param.Add("actionType", AdHocActionType.AmendmentAdd.ToString());
                        //var isExists = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.SkypeForBusiness.IsDCCNumberExists, param);
                        //if (isExists)
                        //{
                        //    ShowAlertMessage(AlertType.Warning, "The given DCC extension already exists.");
                        //    return;
                        //}
                        //sfbdetail.Extension = validExtension.ToString();
                        //sfbdetail.LineUri = lblLineUrIPrefix.Text + sfbdetail.Extension;
                    }
                    adhocRequestDetails.EntitlementDetail = adhocRequestDetails.GetSerializedString(sfbdetail);
                    DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocRequestDetails);
                    this.UpdateAccessRequest(AccessType.Approve);
                }
                else if (adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocRequestDetails);
                    this.UpdateAccessRequest(AccessType.Approve);
                }
                else if (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount)
                {
                    var sfbdetail = (AdhocSkypeForBusinessRequest)adhocRequestDetails.GetEntitlementDetail();
                    if (sfbdetail.EntitlementType == EntitlementType.DCC)
                    {
                        var extension = txtDCCExtension.Text;
                        var validExtension = ValidateExtension(extension);
                        if (validExtension == -1)
                        {
                            ShowAlertMessage(AlertType.Warning, "Please enter valid extension for DCC");
                            return;
                        }
                        //check for exisiting 
                        var param = new Dictionary<string, string>();
                        param.Add("endpointId", adhocRequestDetails.EndpointSystemId.ToString());
                        param.Add("number", validExtension.ToString());
                        param.Add("accessRequestId", task.AccessRequest.Id.ToString());
                        param.Add("actionType", AdHocActionType.AmendmentAdd.ToString());
                        var isExists = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.SkypeForBusiness.IsDCCNumberExists, param);
                        if (isExists)
                        {
                            ShowAlertMessage(AlertType.Warning, "The given DCC extension already exists.");
                            return;
                        }
                        sfbdetail.Extension = validExtension.ToString();
                        sfbdetail.LineUri = lblLineUrIPrefix.Text + sfbdetail.Extension;
                        adhocRequestDetails.EntitlementDetail = adhocRequestDetails.GetSerializedString(sfbdetail);
                    }

                    DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocRequestDetails);
                    this.UpdateAccessRequest(AccessType.Approve);
                }
                else if (adhocRequestDetails.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocRequestDetails);
                    this.UpdateAccessRequest(AccessType.Approve);
                }
            }
            else if (adhocRequestDetails.ApplicationType == ApplicationType.SAP_EP ||
                adhocRequestDetails.ApplicationType == ApplicationType.CE)
            {

                var adhocAcessRequest = adhocRequestDetails;
                var sapepdetail = (AdhocSAPEPRequest)adhocAcessRequest.GetEntitlementDetail();
                if (adhocRequestDetails.AdhocActionType == AdHocActionType.NewAccount &&
                    (adhocRequestDetails.AccountType == AccountType.Shared ||
                    adhocRequestDetails.AccountType == AccountType.Service ||
                    adhocRequestDetails.AccountType == AccountType.Secondary)
                    )
                {
                    string Message = "Please enter";
                    string ControlMessage = "";

                    if (string.IsNullOrWhiteSpace(txtAccountNameSAP.Text) && SharedEditSAP.Visible)
                    {
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AccountName" : ControlMessage + ", AccountName";
                    }
                    if (ControlMessage != "")
                    {
                        Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
                        ShowAlertMessage(AlertType.Error, Message);
                        return;
                    }
                    else
                    {
                        if (ValidateAccountName(txtAccountNameSAP.Text, adhocRequestDetails.EndpointSystemType, adhocRequestDetails.EndpointSystemId))
                        {
                            if (SharedEditSAP.Visible)
                            {
                                sapepdetail.AdHocSAPEPSharedAccount.AccountName = txtAccountNameSAP.Text;
                                sapepdetail.AdHocSAPEPSharedAccount.Description = txtDescriptionSAP.Text;
                            }
                            adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(sapepdetail);
                            DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                            this.UpdateAccessRequest(AccessType.Approve);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {

                    if (adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        if (ExistingGroupAutoCompleteBox.CheckedItems.Count <= 0 && ExistingRoleAutoCompleteBox.CheckedItems.Count <= 0)
                        {
                            ShowAlertMessage(AlertType.Error, "Please select at least one Group or Role");
                            return;
                        }
                        else
                        {
                            if (ExistingGroupAutoCompleteBox.CheckedItems.Count > 0)
                            {
                                sapepdetail.Groups = EntiltementString(ExistingGroupAutoCompleteBox, "Groups");
                            }
                            if (ExistingRoleAutoCompleteBox.CheckedItems.Count > 0)
                            {
                                sapepdetail.Roles = EntiltementString(ExistingRoleAutoCompleteBox, "Roles");
                            }
                            adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(sapepdetail);
                            DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);

                        }
                    }
                    else if (adhocRequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd)
                    {
                        if (PortalGroups.Count <= 0 && PortalRoles.Count <= 0)
                        {
                            ShowAlertMessage(AlertType.Error, "Please select at least one Group or Role");
                            return;
                        }
                        else
                        {

                            if (PortalGroups.Count > 0)
                            {
                                sapepdetail.Groups = GetPortalEntitlements("Groups");
                            }
                            if (PortalRoles.Count > 0)
                            {
                                sapepdetail.Roles = GetPortalEntitlements("Roles");
                            }
                            adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(sapepdetail);
                            DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);

                        }
                    }

                }

                this.UpdateAccessRequest(AccessType.Approve);
            }


            else if ((applicationType == ApplicationType.ADSingaporePowerLocal || applicationType == ApplicationType.SAPLocal))
            {
                bool isSharedService = task.AccessRequest.AdHocAccessRequest.AccountType == AccountType.Secondary || task.AccessRequest.AdHocAccessRequest.AccountType == AccountType.Service
                    || task.AccessRequest.AdHocAccessRequest.AccountType == AccountType.Shared;

                if (task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount && (isSharedService || applicationType == ApplicationType.SAPLocal))
                {
                    string Message = "Please enter";
                    string ControlMessage = "";
                    if (string.IsNullOrWhiteSpace(txtAcntName.Text))
                    {
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " AccountName" : ControlMessage + ", AccountName";
                    }
                    if (string.IsNullOrWhiteSpace(txtDisplayName.Text))
                    {
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " DisplayName" : ControlMessage + ", DisplayName";
                    }
                    if (OUAutoCompleteBox.Entries.Count != 1)
                    {
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " OU" : ControlMessage + ", OU";
                    }
                    if (ControlMessage != "")
                    {
                        Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
                        ShowAlertMessage(AlertType.Error, Message);
                        return;
                    }
                    else
                    {

                        if (ValidateAccountName(txtAcntName.Text, adhocRequestDetails.EndpointSystemType, adhocRequestDetails.EndpointSystemId))
                        {
                            string validation = "";
                            GenericRequestDetailsControl.UpdateADEntitlement(task, out validation);
                            if (validation == "")
                            {
                                Dto.AdHocAccessRequest adhocRequest = DomainApiClient<Dto.AdHocAccessRequest>.GetById(adhocRequestDetails.Id);
                                if (applicationType != ApplicationType.SAPLocal)
                                {
                                    var adHocADRequest = (AdHocADRequest)adhocRequest.GetEntitlementDetail();
                                    adHocADRequest.AdHocADSharedAccount.AccountName = txtAcntName.Text;
                                    adHocADRequest.AdHocADSharedAccount.DisplayName = txtDisplayName.Text;
                                    adHocADRequest.AdHocADSharedAccount.Description = txtDiscription.Text;
                                    if (OUAutoCompleteBox.Entries.Count > 0)
                                    {
                                        adHocADRequest.AdHocADSharedAccount.OU = OUAutoCompleteBox.Entries[0].Text;
                                    }
                                    adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocADRequest);
                                }
                                else
                                {
                                    var adHocADRequest = (AdHocSAPLocalRequest)adhocRequest.GetEntitlementDetail();
                                    adHocADRequest.AccountName = txtAcntName.Text;
                                    adHocADRequest.DisplayName = txtDisplayName.Text;
                                    adHocADRequest.Description = txtDiscription.Text;
                                    if (OUAutoCompleteBox.Entries.Count > 0)
                                    {
                                        adHocADRequest.OU = OUAutoCompleteBox.Entries[0].Text;
                                    }
                                    adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocADRequest);

                                }

                                DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocRequest);
                                this.UpdateAccessRequest(AccessType.Approve);
                                Session["EndpointSystemID"] = null;
                            }
                            else
                            {
                                ShowAlertMessage(AlertType.Error, validation);
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    string validation = "";
                    GenericRequestDetailsControl.UpdateADEntitlement(task, out validation);
                    if (!string.IsNullOrWhiteSpace(validation))
                    {
                        ShowAlertMessage(AlertType.Error, validation);
                        return;
                    }
                    this.UpdateAccessRequest(AccessType.Approve);
                }
            }
        }

        private void UpdateTask(AccessType accessType)
        {
            if (eventTrigger != null)
            {
                if (accessType == AccessType.Approve)
                {
                    var user = DomainApiClient<Dto.User>.GetById(eventTrigger.UserId);
                    eventTrigger.RequestStatus = AccessRequestStatus.Approved;
                    if (eventTrigger.EventType == EmployeeEvents.NonEmployeeExtension)
                    {
                        var newUser = UtilityMethods.Deserialize<User>(eventTrigger.UserDetails);
                        if (newUser != null)
                        { 
                            user.TerminationDate = newUser.TerminationDate;
                            
                        }
                    } 
                    user.Reserved4 = Constants.EmployeeStatus.OnBoarded;
                    DomainApiClient<Dto.User>.Save(user); 
                }
                else if (accessType == AccessType.Revoke)
                {
                    eventTrigger.RequestStatus = AccessRequestStatus.Rejected;
                }
                eventTrigger.LastModifiedBy = _sessionStatus.User.UserName;
                eventTrigger.LastModifiedDate = DateTime.Now;
                eventTrigger.Comments = txtComments.Text;
                DomainApiClient<Dto.EmployeeEventTrigger>.Save(eventTrigger);
            }

            Task.LastModifiedBy = _sessionStatus.User.UserName;
            Task.LastModifiedDate = DateTime.Now;
            AddToInterimStore(Task);
        }

        private void AddToInterimStore(Dto.Task task)
        {
            InterimStore interimStore = new InterimStore()
            {
                IsActive = true,
                IsSaved = true,
                TaskId = task.Id,
                XmlData = UtilityMethods.Serialize(Task),
            };
            DomainApiClient<Dto.InterimStore>.Save(interimStore);
        }

        string EntiltementString(object Control, string TypeofEntiltement)
        {
            string entiltment = "";
            RadComboBox control = (RadComboBox)Control;
            foreach (RadComboBoxItem checkedItems in control.CheckedItems)
            {
                if (TypeofEntiltement == "Roles")
                {
                    entiltment = entiltment == "" ? checkedItems.Value : entiltment + "," + checkedItems.Value;
                }
                else
                {
                    entiltment = entiltment == "" ? checkedItems.Value : entiltment + "," + checkedItems.Value;
                }
            }
            return entiltment;
        }
        public bool SRPAccountValidation(AdHocAccessRequest adhocRequest, string accountName)
        {

            //var ADdetail = DomainApiClient<Dto.ADSUserInfo>.GetByAction<List<ADSUserInfo>>(DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.GetByDIUserId,
            //                 new Dictionary<string, string> { { "DIUserId", adhocRequest.RequesteeUserId.ToString() } });

            //var adUsers = adFacade.GetAdsUsers(adhocRequest.RequesteeUserId);
            AdHocSAPSRMRequest srmRequest = (AdHocSAPSRMRequest)adhocRequest.GetEntitlementDetail();
            bool adGroupExist = false;
            foreach (var item in srmRequest.AdHocSAPSRMRequestDetails)
            {
                adGroupExist = DomainApiClient<Dto.SAPSRMMainRoleMatrix>.GetByAction<bool>(DomainApiAction.Matrix.SRM.IsAdGroupsExist,
                     new Dictionary<string, string> { { "RoleName", item.Entitlement } });
                if (adGroupExist)
                    break;

            }
            if (adGroupExist)
            {
                if (accountName != string.Empty)
                {

                    var aduserByName = DomainApiClient<Dto.ADSUserInfo>.GetByAction<ADSUserInfo>(DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.GetSharedADSUserInfoByName,
                       new Dictionary<string, string> { { "username", accountName }, { "accountType", adhocRequest.AccountType.ToString() } });
                    if (aduserByName == null)
                    {
                        ShowAlertMessage(AlertType.Error, accountName + " account does not exists in AD.");
                        return false;
                    }
                }
            }
            return true;
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (Task.EmployeeEventTriggerId > 0)
            {
                UpdateTask(AccessType.Approve);
                UpdateAccessRequest(AccessType.Approve);
                return;
            }

            var task = GetTask();
            if (task.AccessRequest == null)
            {
                ShowAlertMessage(AlertType.Error, "AccessRequest Object is null in Task. Need a code fix.");
                return;
            }
            if (task.AccessRequest.RequestStatus == AccessRequestStatus.Withdrawn)
            {
                ShowAlertMessage(AlertType.Error, "Request has been withdrawn.You cannot approve the task");
                return;
            }
            if (task.AccessRequest.RequestStatus == AccessRequestStatus.Rejected)
            {
                ShowAlertMessage(AlertType.Error, "Request has been Rejected.You cannot approve the task");
                return;
            }
            if (Task != null && Task.AccessRequest != null && Task.AccessRequest.AdHocAccessRequest != null)
            {
                //if (CanShowGRCMessage())
                //{
                //    if (Convert.ToString(Task.WorkflowStateId) == MasterConfigHelper.MSSLEBSROWorkflowStateId)
                //    {
                //        //Show confirm message
                //        ShowConfirm(MasterConfigHelper.EBS_MSSL_WarningMessage);
                //        return;
                //    }
                //}
                if(Task != null &&
                (Task.AccessRequest != null && (Task.AccessRequest.ApplicationType == ApplicationType.MSSL || Task.AccessRequest.ApplicationType == ApplicationType.EBS || Task.AccessRequest.ApplicationType == ApplicationType.EBS_ISU || Task.AccessRequest.ApplicationType == ApplicationType.EBS_CRM || Task.AccessRequest.ApplicationType == ApplicationType.MSSL_CRM || Task.AccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                ) && (Task.AccessRequest != null && Task.AccessRequest.AdHocAccessRequest != null && (Task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount || Task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
                && (TCodeValueLabeltr.Visible && !string.IsNullOrWhiteSpace(TCodeValueLabel.Text) && string.IsNullOrWhiteSpace(txtComments.Text))
                )
                {
                    ShowAlertMessage(AlertType.Error, "Approver comments is required for Critical Tcode Access", true);
                    return;
                }

                if (CanShowFlinsConflictMessage() || CanShowEBSMSSLConflictMessage())
                {
                    ShowAlertMessage(AlertType.Error, "Approver comments is required for Conflicting Access",true);                  
                    return;
                }

                ApproveTask(task);
            }
        }
        bool CanShowGRCMessage()
        {
            var applications = new ApplicationType[] { ApplicationType.MSSL_ISU, ApplicationType.MSSL_CRM, ApplicationType.EBS_CRM, ApplicationType.EBS_ISU };
            var adhocActions = new AdHocActionType[] { AdHocActionType.NewAccount, AdHocActionType.AmendmentAdd };
            var accountTypes = new AccountType[] { AccountType.User, AccountType.Secondary };
            return applications.Contains(Task.AccessRequest.AdHocAccessRequest.ApplicationType) && adhocActions.Contains(Task.AccessRequest.AdHocAccessRequest.AdhocActionType)
                && accountTypes.Contains(Task.AccessRequest.AccountType);
        }

        bool CanShowFlinsConflictMessage()
        {
            var adhocRequestDetails = Task.AccessRequest.AdHocAccessRequest;
            ApplicationType applicationType = adhocRequestDetails.ApplicationType;
            return applicationType == ApplicationType.HRMS_FLINS && this.GenericRequestDetailsControl.ConflictRemark != null && this.GenericRequestDetailsControl.ConflictRemark.Count > 0 && string.IsNullOrWhiteSpace(txtComments.Text);
        }

        bool CanShowEBSMSSLConflictMessage()
        {
            return (
                (Task != null &&
                (Task.AccessRequest != null && (
                Task.AccessRequest.ApplicationType == ApplicationType.EBS ||
                Task.AccessRequest.ApplicationType == ApplicationType.MSSL ||
                Task.AccessRequest.ApplicationType == ApplicationType.EBS_ISU || Task.AccessRequest.ApplicationType == ApplicationType.EBS_CRM || Task.AccessRequest.ApplicationType == ApplicationType.MSSL_CRM || Task.AccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                ) && this.GenericRequestDetailsControl != null && this.GenericRequestDetailsControl.MSSLEBSConflictRemark != null && this.GenericRequestDetailsControl.MSSLEBSConflictRemark.Count > 0)
                && string.IsNullOrWhiteSpace(txtComments.Text));
        }

        public void ShowConfirm(string Message)
        {
            ConfirmMessage1.Message = Message;
            ConfirmMessageUpdatePanel.Update();
            ConfirmMessage1.Show();
        }

        private void ConfirmMessage1_ConfirmYesClick(object sender, EventArgs e)
        {
            var task = GetTask();
            ApproveTask(task);
        }

        void ApproveTask(Dto.Task task)
        {
            string validationmsg = string.Empty;
            GenericRequestDetailsControl.UpdateADEntitlement(task, out validationmsg);
            if (!string.IsNullOrWhiteSpace(validationmsg))
            {
                ShowAlertMessage(AlertType.Error, validationmsg);
                return;
            }
            if (task.Action == StatusType.Pending)
            {
                this.UpdateAccessRequest(AccessType.Approve);
            }
            else
            {
                ShowAlertMessage(AlertType.Error, "This task has already been approved.");
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            var task = GetTask();
            if (task.EmployeeEventTriggerId.HasValue == false)
            {
                if (task.AccessRequest == null)
                {
                    ShowAlertMessage(AlertType.Error, "AccessRequest Object is null in Task. Need a code fix.");
                    return;
                }
                if (task.AccessRequest.RequestStatus == AccessRequestStatus.Withdrawn)
                {
                    ShowAlertMessage(AlertType.Error, "Request has been withdrawn.You cannot reject the task");
                    return;
                }
                if (task.AccessRequest.RequestStatus == AccessRequestStatus.Rejected)
                {
                    ShowAlertMessage(AlertType.Error, "Request has been Rejected.You cannot reject the task");
                    return;
                }
            }
            if (!string.IsNullOrWhiteSpace(txtComments.Text))
            {
                UpdateTask(AccessType.Revoke);
                this.UpdateAccessRequest(AccessType.Revoke);
            }
            else
            {
                ShowAlertMessage(AlertType.Error, "Please enter comments for rejection.".Replace("\n", "<br>"));
            }
        }
        private bool CanApprove(Dto.Task task)
        {
            bool can = false;
            var session = new SessionStatusImpl();
            if ((task.ActionUserId.HasValue && task.ActionUserId.Value == session.UserId)
                || (task.AssignedTo.HasValue && task.AssignedTo.Value == session.UserId))
            {
                can = true;
            }
            else if (task.IsGroupTask && task.ActionGroupId.HasValue && task.ActionGroupId.Value > 0)
            {
                var param = new Dictionary<string, string>();
                param.Add("id", task.ActionGroupId.Value.ToString());
                var group = DomainApiClient<Dto.Group>.GetByAction<Dto.Group>(DomainApiAction.Group.GetByIdIncludingUser, param);
                if (group != null && group.Users.Any(i => i.Id == session.UserId))
                {
                    can = true;
                }
            }
            return can;
        }
        //protected void BackButton_Click(object sender, EventArgs e)
        //{
        //    RedirectToParentPage();
        //}

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectToParentPage();
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        protected void lnkViewNotification_Click(object sender, EventArgs e)
        {
            ViewNotification.RequestId = this.Task.AccessRequestId;
            ViewNotification.BindNotifications();
            RadWinViewNotification.VisibleOnPageLoad = true;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            RadWinViewNotification.VisibleOnPageLoad = false;
        }

        protected void btnSendNotification_Click(object sender, EventArgs e)
        {
            UCSendNotification.BindNotifiableUsers(this.Task.AccessRequestId);
            SendNotificationDisplay(true);
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string validationMessage = UCSendNotification.ValidateNotification();
            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                RadWinSendNotification.VisibleOnPageLoad = true;
                MasterPage.ShowAlert(AlertType.Warning, validationMessage);
                return;
            }
            Dto.Notification notification = UCSendNotification.GetNotificationDetails();
            string appendSubText = "Request Id - " + (long)Task.AccessRequestId + " ";
            if (!notification.Subject.Contains(appendSubText))
                notification.Subject = appendSubText + notification.Subject;
            notification.RequestId = (long)Task.AccessRequestId;
            DomainApiClient<Dto.Notification>.Save(notification);
            SendNotificationDisplay(false);
            MasterPage.ShowAlert(AlertType.Success, "Notification sent successfully");
        }

        protected void btnCancelSend_Click(object sender, EventArgs e)
        {
            UCSendNotification.RequestId = this.Task.AccessRequestId;
            SendNotificationDisplay(false);
        }




        #endregion

        protected void DeleteGroupButton_Click(object sender, EventArgs e)
        {
            string groupUniqueName = ((LinkButton)sender).CommandArgument;
            var groups = this.PortalGroups;
            groups.Remove(groupUniqueName);
            this.PortalGroups = groups;

            GroupGrid.DataSource = PortalGroups;
            GroupGrid.Rebind();
        }
        protected void DeleteRoleButton_Click(object sender, EventArgs e)
        {
            string roleId = ((LinkButton)sender).CommandArgument.ToString();
            var roles = this.PortalRoles;
            roles.Remove(roleId);
            this.PortalRoles = roles;

            RoleGrid.DataSource = PortalRoles;
            RoleGrid.Rebind();
        }
        Dictionary<string, string> PortalRoles
        {
            get
            {
                if (ViewState["PortalRoles"] != null)
                {
                    return (Dictionary<string, string>)ViewState["PortalRoles"];
                }
                return new Dictionary<string, string>();
            }
            set
            {
                ViewState["PortalRoles"] = value;
            }
        }
        string GetPortalEntitlements(string TypeofEntiltement)
        {
            string entiltment = "";

            if (TypeofEntiltement == "Roles")
            {
                foreach (KeyValuePair<string, string> role in PortalRoles)
                {
                    entiltment = entiltment == "" ? role.Key : entiltment + "," + role.Key;
                }
            }
            if (TypeofEntiltement == "Groups")
            {
                foreach (KeyValuePair<string, string> group in PortalGroups)
                {
                    entiltment = entiltment == "" ? group.Key : entiltment + "," + group.Key;
                }
            }
            return entiltment;
        }
        Dictionary<string, string> PortalGroups
        {
            get
            {
                if (ViewState["PortalGroups"] != null)
                {
                    return (Dictionary<string, string>)ViewState["PortalGroups"];
                }
                return new Dictionary<string, string>();
            }
            set
            {
                ViewState["PortalGroups"] = value;
            }
        }

        void boundEPGroups(string groupUniqueName, string group)
        {
            var groups = this.PortalGroups;
            if (!groups.ContainsKey(groupUniqueName))
            {
                groups.Add(groupUniqueName, group);
            }

            this.PortalGroups = groups;

            GroupGrid.DataSource = PortalGroups;
            GroupGrid.Rebind();
            trGroupGrid.Visible = true;
        }

        void boundEPRoles(string roleId, string roleName)
        {
            var roles = this.PortalRoles;

            if (!roles.ContainsKey(roleId))
            {
                roles.Add(roleId, roleName);
            }

            this.PortalRoles = roles;

            RoleGrid.DataSource = PortalRoles;
            RoleGrid.Rebind();
            trRoleGrid.Visible = true;
        }

        protected void btnGrpAdd_Click(object sender, EventArgs e)
        {
            if (this.GroupsAutoCompleteBox.Entries.Count != 0)
            {
                boundEPGroups(this.GroupsAutoCompleteBox.Entries[0].Value, this.GroupsAutoCompleteBox.Entries[0].Text);
                this.GroupsAutoCompleteBox.Entries.Clear();
            }
            else
            {
                ShowAlertMessage(AlertType.Warning, "Please select at least one group");
            }
        }

        protected void btnroleAdd_Click(object sender, EventArgs e)
        {
            if (this.RolesAutoCompleteBox.Entries.Count != 0)
            {
                boundEPRoles(this.RolesAutoCompleteBox.Entries[0].Value, this.RolesAutoCompleteBox.Entries[0].Text);

                this.RolesAutoCompleteBox.Entries.Clear();
            }
            else
            {
                ShowAlertMessage(AlertType.Warning, "Please select at least one role");
            }
        }

        private void SetApproverForm()
        {

            var approvers = this.TaskApprovers;

            if (approvers.Count == 0)
            {
                approvers = this.GetApprovers();
                this.TaskApprovers = approvers;
            }
            grdApprover.MasterTableView.DetailTables[0].HierarchyDefaultExpanded = true;
            grdApprover.DataSource = approvers;
            grdApprover.DataBind();

            //if (approvers.Any(a => a.IsActionOnlyRequired))
            //{
            //    divExecutionerGrid.Visible = true;
            //    divExecutionerHeader.Visible = true;
            //    grdExecutioner.DataSource = approvers.Where(app => app.IsActionOnlyRequired).Select(app => app).ToList();
            //    grdExecutioner.DataBind();
            //}
            //else
            //{
            //    divExecutionerGrid.Visible = false;
            //    divExecutionerHeader.Visible = false;
            //}
        }

        protected void grdApprover_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            byte state = (byte)dataItem.GetDataKeyValue("ApprovalState");
            try
            {
                var approver = this.TaskApprovers.FirstOrDefault(item => item.ApprovalState == state && item.ApproverName == dataItem.DataItem.ToString());
                e.DetailTableView.DataSource = approver.ApproverDetails;
            }
            catch (Exception)
            {
                var approver = this.TaskApprovers.FirstOrDefault(item => item.ApprovalState == state);

                e.DetailTableView.DataSource = approver.ApproverDetails;
            }
            
        }

        List<Dto.Presentation.PresentationApprovalChain> GetApprovers()
        {
            if (Task.AccessRequestId.HasValue)
            {
                if (!Task.AccessRequest.BulkRequestId.HasValue)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("accessRequestId", Task.AccessRequestId.ToString());
                    return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByRequestId, parameter);

                }
                else
                {
                    if (Task.AccessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.InhouseSystems)
                    {
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("accessRequestId", Task.AccessRequestId.ToString());
                        return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByRequestId, parameter);
                    }
                    else
                    {
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("bulkRequestId", Task.AccessRequest.BulkRequestId.ToString());
                        return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByBulkRequestId, parameter);
                    }
                }
            }
            else if (Task.BulkRequestId != null && Task.BulkRequestId > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("bulkRequestId", Task.BulkRequestId.ToString());
                return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByBulkRequestId, parameter);
            }
            else if (Task.EmployeeEventTriggerId != null && Task.EmployeeEventTriggerId > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("triggerId", Task.EmployeeEventTriggerId.ToString());
                return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByEmployeeRequestId, parameter);
            }

            return new List<PresentationApprovalChain>();
        }

        protected void ResponseGroupAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (ReservedNumberAutoCompleteBox.Entries.Count > 0)
            {
                string group = string.Empty;
                var NewItem = ReservedNumberAutoCompleteBox.Entries[ReservedNumberAutoCompleteBox.Entries.Count - 1];
                ReservedNumberAutoCompleteBox.Entries.Clear();
                ReservedNumberAutoCompleteBox.Entries.Add(NewItem);
            }
            else
            {
                ShowAlertMessage(AlertType.Warning, "Multi selection not allowed");
            }
        }
        protected void ResponseGroupAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

        }
        private int ValidateExtension(string extensionText)
        {
            var validExtension = -1;
            var extension = extensionText;
            if (string.IsNullOrWhiteSpace(extension))
            {
                ShowAlertMessage(AlertType.Warning, "Please enter extension Number");
                return validExtension;
            }
            int outValidxtension;
            if (int.TryParse(extension, out outValidxtension) && extension.Length == 4)
            {
                return validExtension = outValidxtension;
            }
            else
            {
                ShowAlertMessage(AlertType.Warning, "Please enter valid 4 digit extension Number");
                return validExtension;
            }
        }

        protected void grdApprover_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                foreach (GridDataItem dataitem in grdApprover.MasterTableView.Items)
                {
                    if (dataitem != null)
                    {
                        var status = (StatusType)dataitem.GetDataKeyValue("Action");
                        if (status == StatusType.Approved || status == StatusType.AutoCompleted || status == StatusType.Rejected || status == StatusType.Pending || status == StatusType.Withdrawn)
                        {
                            dataitem.Expanded = true;
                        }
                    }
                }
            }
        }

        protected void grdApprover_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == RadGrid.ExpandCollapseCommandName)
            {
                foreach (GridItem item in e.Item.OwnerTableView.Items)
                {
                    if (item.Expanded && item != e.Item)
                    {
                        item.Expanded = false;
                    }
                }
            }
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            //if (!string.IsNullOrWhiteSpace(this.Request.QueryString["ddltaskstatus"]) && !string.IsNullOrWhiteSpace(this.Request.QueryString["source"]) && !string.IsNullOrWhiteSpace(this.Request.QueryString["showalltask"]))
            {
                string source = this.Request.QueryString["source"] ?? "";
                string taskStatus = this.Request.QueryString["ddltaskstatus"] ?? "";
                string showAllTasks = this.Request.QueryString["showalltask"] ?? "";
                string fromDate = this.Request.QueryString["fromDate"] ?? "";
                string toDate = this.Request.QueryString["toDate"] ?? "";
                string requestFor = this.Request.QueryString["requestFor"] ?? "";
                string requestForName = this.Request.QueryString["requestForName"] ?? "";
                string epId = this.Request.QueryString["endpointsystemId"] ?? "";
                if (source.ToLower() == "grouptask")
                {
                    Response.Redirect($"GroupTasks.aspx?ddltaskstatus={taskStatus}&showalltask={showAllTasks}&toDate={toDate}&fromDate={fromDate}&requestFor={requestFor}&requestForName={requestForName}&endpointsystemId={epId}");
                }
                else if (source.ToLower() == "mytask")
                {
                    Response.Redirect($"List.aspx?ddltaskstatus={taskStatus}&showalltask={showAllTasks}&toDate={toDate}&fromDate={fromDate}&requestFor={requestFor}&requestForName={requestForName}&endpointsystemId={epId}");
                }
                else if (source.ToLower() == "home")
                {
                    Response.Redirect("~/Home/Home.aspx");
                }else if(source.ToUpper()== "bulkassigntask".ToUpper())
                {
                    string groupId = this.Request.QueryString["groupId"] ?? "";
                    Response.Redirect($"BulkAssignGroupTasks.aspx?groupId={groupId}");
                }
            }
        }
    }
}