using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Dto.Presentation;
namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class GenericRequestDetails : UIBaseUC
    {
        public List<AdHocSAPHRFLINSRequestDetail> FlinsEntitlementList
        {
            get
            {
                if (ViewState["V_FlinsEntitlements"] != null)
                {
                    return (List<AdHocSAPHRFLINSRequestDetail>)ViewState["V_FlinsEntitlements"];
                }
                return new List<AdHocSAPHRFLINSRequestDetail>();
            }
            set
            {
                ViewState["V_FlinsEntitlements"] = value;
            }
        }
        public bool UpdateOneKeyToken
        {
            set
            {
                ViewState["UpdateOneKeyToken"] = value;
            }
            get
            {
                if (ViewState["UpdateOneKeyToken"] != null)
                    return (bool)ViewState["UpdateOneKeyToken"];
                return false;
            }
        }
        public long AdHocRequestId
        {
            get
            {
                if (ViewState["AdHocAccessRequestId"] != null)
                {
                    return Convert.ToInt64(ViewState["AdHocAccessRequestId"]);
                }
                return 0;
            }
            set
            {
                ViewState["AdHocAccessRequestId"] = value;
            }
        }

        public bool ConvertLinkButtonToLabel
        {
            get
            {
                if (ViewState["ConvertLinkButtonToLabel"] != null)
                {
                    return Convert.ToBoolean(ViewState["ConvertLinkButtonToLabel"]);
                }
                return false;
            }
            set
            {
                ViewState["ConvertLinkButtonToLabel"] = value;
            }
        }
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
            }
        }
        public long EndpointSystemId
        {
            get
            {
                if (ViewState["EndpointSystemId"] != null)
                {
                    return Convert.ToInt64(ViewState["EndpointSystemId"]);
                }
                return 0;
            }
            set
            {
                ViewState["EndpointSystemId"] = value;
            }
        }
        public PresentationMyRequestData AccessRequest
        {
            get
            {
                if (ViewState["PresentationMyRequestData"] != null)
                {
                    return (PresentationMyRequestData)ViewState["PresentationMyRequestData"];
                }
                return null;
            }
            set
            {
                ViewState["PresentationMyRequestData"] = value;
            }
        }
        public ApplicationType ApplicationType
        {
            set
            {
                ViewState["ApplicationType"] = value;
            }
            get
            {
                ApplicationType applicationType;
                if(Enum.TryParse<ApplicationType>(ViewState["ApplicationType"].ToString(), out applicationType))
                {
                    return applicationType;
                }
                return ApplicationType.None;
            }
        }
        public AdHocActionType AdHocActionType
        {
            set
            {
                ViewState["AdHocActionType"] = value;
            }
            get
            {
                AdHocActionType adHocActionType;
                if (Enum.TryParse<AdHocActionType>(ViewState["AdHocActionType"].ToString(), out adHocActionType))
                {
                    return adHocActionType;
                }
                return AdHocActionType.None;
            }
        }
        private const string V_CONFLICTS = "V_CONFLICTS";
        ApplicationType[] applicationTypes = new ApplicationType[] { ApplicationType.EBS, ApplicationType.MSSL, ApplicationType.MSSL_ISU, ApplicationType.MSSL_CRM, ApplicationType.EBS_ISU, ApplicationType.EBS_CRM, ApplicationType.SAP_GRC, ApplicationType.FLINS_BI };
        public List<string> ConflictRemark
        {
            get
            {
                if (ViewState[V_CONFLICTS] != null)
                {
                    return (List<string>)ViewState[V_CONFLICTS];
                }
                return new List<string>();
            }
            set
            {
                ViewState[V_CONFLICTS] = value;
            }
        }
        public bool IsEBSMSSLConflictRemark
        {
            get
            {
                if (ViewState["IsEBSMSSLConflictRemark"] != null)
                {
                    return Convert.ToBoolean(ViewState["IsEBSMSSLConflictRemark"]);
                }
                return false;
            }
            set
            {
                ViewState["IsEBSMSSLConflictRemark"] = value;
            }
        }
        private const string V_MSSLEBSConflictRemark = "V_MSSLEBSConflictRemark";
        public List<PresentationConflictRemarks> MSSLEBSConflictRemark
        {
            get
            {
                if (ViewState[V_MSSLEBSConflictRemark] != null)
                {
                    return (List<PresentationConflictRemarks>)ViewState[V_MSSLEBSConflictRemark];
                }
                return new List<PresentationConflictRemarks>();
            }
            set
            {
                ViewState[V_MSSLEBSConflictRemark] = value;
            }
        }
        public string AdName { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if(!string.IsNullOrWhiteSpace(Request.QueryString["HRMSRequest"]) && Request.QueryString["HRMSRequest"] == "1")
                {

                }
                else
                {
                    InitializeForm();
                }
                LabelVisibles();
            }
        }


        public void PopulateHRMSRequestDetails(Dto.AccessRequest accessRequest)
        {
            try
            {
                this.RequestIDLabel.Text = accessRequest.Id.ToString();
                this.SystemNameLabel.Text = UtilityMethods.GetEnumAttribute<Description>(accessRequest.ApplicationType);
                AccountTypeLabel.Text = UtilityMethods.GetEnumAttribute<Description>(accessRequest.AccountType);
                RequestTypeLabel.Text = UtilityMethods.GetEnumAttribute<Description>(accessRequest.RequestSource);
                AccessNametr.Visible = false;
                GenericPanel.Visible = true;
                bulkrequestidgeneraltr.Visible = false;
                AccessNametr.Visible = false;
                this.lblValidFrom.Text = (accessRequest.ActivationDate.HasValue) ? accessRequest.ActivationDate.Value.ToString("dd-MM-yyyy") : "";
                ValidFromRow.Visible = false;
                ValidToRow.Visible = false;
                LogonUserNametr.Visible = false;
                Remarkstr.Visible = false;
                if (!string.IsNullOrWhiteSpace(accessRequest.Description))
                {
                    RequestorCommentsRow.Visible = true;
                    lblRequestorCommentstxt.Text = accessRequest?.Description?.Trim()?.Replace("\n", "<br/>"); ;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void InitializeForm()
        {
            this.PopulateRequestDetails();
        }
        public void SetWithDrawVisibility(Dto.User user, Dto.EmployeeEventTrigger trigger, Dto.Task task)
        {
            if (user.RequestStatus == AccessRequestStatus.Withdrawn)
            {
                this.pnlWReason.Visible = true;
                this.pnlWDate.Visible = true;
                lblNEWReason.Text = trigger.Comments;
                lblNPWithDate.Text = task.LastModifiedDate?.ToString("dd-MM-yyyy hh:mm:ss tt");
            }
            
        }
        public void SetNonEmployeeInfo(Dto.User user, Dto.EmployeeEventTrigger trigger)
        {
            GenericPanel.Visible = false;
            pnlNonEmployee.Visible = true;
            lblNonEmployeeName.Text = user.UserName;
            lblOldDisplayName.Text = user.DisplayName;
            lblNewDisplayName.Text = user.ProjectDescription;
            lblEmployeeType.Text = user.EmployeeType.GetDescription();
            lblJoinDate.Text = user.JoinDate.HasValue ? user.JoinDate.Value.ToString("dd-MM-yyyy") : string.Empty;
            lblTerminationDate.Text = user.TerminationDate.HasValue ? user.TerminationDate.Value.ToString("dd-MM-yyyy") : string.Empty;
            lblProject.Text = user.ProjectDescription;
            var oldEffectiveDate = trigger.OldEffectiveDate.HasValue ? trigger.OldEffectiveDate.Value.ToString("dd-MM-yyyy") : string.Empty;
            var newUser = UtilityMethods.Deserialize<User>(trigger.UserDetails);
            var oldUser = UtilityMethods.Deserialize<User>(trigger.OldUserDetails);
            if (newUser != null && oldUser != null)
            {
                lblTerminationDate.Text = oldUser.TerminationDate.HasValue ? oldUser.TerminationDate.Value.ToString("dd-MM-yyyy") : string.Empty;
                if (!newUser.DisplayName.Equals(oldUser.DisplayName, StringComparison.InvariantCultureIgnoreCase))
                {
                    divNewDisplayName.Visible = true;
                    lblNewDisplayName.Text = newUser.DisplayName;
                }
                else
                {
                    divNewDisplayName.Visible = false;
                }
                if (newUser.TerminationDate != oldUser.TerminationDate)
                {
                    divNewTerminationDate.Visible = true;
                    lblNewTerminationDate.Text = newUser.TerminationDate.HasValue ? newUser.TerminationDate.Value.ToString("dd-MM-yyyy") : string.Empty;
                }
                else
                {
                    divNewTerminationDate.Visible = false;
                }
                if (trigger.RequestStatus == AccessRequestStatus.Rejected)
                {
                    pnlUserInfo.Visible = true;
                    MbNumber.Text = newUser.MobileNumber;
                    LabelCompany.Text = newUser.CompanyName;
                    lblCostCenter.Text = newUser.CostCenterText;
                    lblJoinDate.Text = newUser.JoinDate.HasValue ? newUser.JoinDate.Value.ToString("dd-MM-yyyy") : string.Empty;
                    lblRO.Text = GetUsername(newUser.ROEmployeeId);
                    lblCO.Text = GetUsername(newUser.COEmployeeId);
                    lblOrgName.Text = newUser.OrgDepartment;
                    if (newUser.NOrganisationId.HasValue)
                    {
                        var orgInfo = DomainApiClient<NOrganisation>.GetById(newUser.NOrganisationId.Value);
                        lblOrganisation.Text = orgInfo?.Name;
                        if (orgInfo != null)
                        {
                            lblOrgName.Text = orgInfo.Code + " - " + orgInfo.Name;
                        }
                    }
                    lblBranch.Text = newUser.OrgBranch;
                    lblSection.Text = newUser.OrgSection;
                    lblUnit.Text = newUser.OrgUnit;


                }
            }
            else if(newUser != null)
            {
                if(user.TerminationDate != newUser.TerminationDate)
                {
                    lblTerminationDate.Text = newUser.TerminationDate.HasValue ? newUser.TerminationDate.Value.ToString("dd-MM-yyyy") : string.Empty;
                }
            }
            
        }
        string GetUsername(string employeeId)
        {
            var userDisplayName = string.Empty;
            if (string.IsNullOrWhiteSpace(employeeId))
                return userDisplayName;

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("employeeId", employeeId);
            var user = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByEmployeeId, param);
            if (user != null)
                return user.DisplayName;
            return userDisplayName;
        }
        private void LabelVisibles()
        {
            if (RequestIDLabel == null)
            {
                trrequest.Visible = false;
            }
            if (SystemNameLabel == null)
            {
                systemname.Visible = false;
            }
            if (RequestTypeLabel == null)
            {
                trrequesttype.Visible = false;
            }
            if (AccountTypeLabel == null)
            {
                traccounttype.Visible = false;
            }
            if (AccessNameLabel == null)
            {
                AccessNametr.Visible = false;
            }
            if (RequestInfoLabel == null)
            {
                SAPEPRequestInfotr.Visible = false;
            }
            if (lblLogonUserName == null)
            {
                LogonUserNametr.Visible = false;
            }
            if (lblValidFrom == null)
            {
                ValidFromRow.Visible = false;
            }
            if (lblValidTo == null)
            {
                ValidToRow.Visible = false;
            }
        }

        private void PopulateRequestDetails()
        {

            PresentationMyRequestData adhocRequest = DomainApiClient<PresentationMyRequestData>.GetByAction<PresentationMyRequestData>(DomainApiAction.PresentationMyRequestData.GetByAdhocId, new Dictionary<string, string> { { "requestId", this.AdHocRequestId.ToString() }, { "deepLoad", "true" } });

            if (adhocRequest == null) return;
            this.AccessRequest = adhocRequest;

            this.ApplicationType = this.AccessRequest.ApplicationType;
            this.AdHocActionType = this.AccessRequest.AdhocActionType;

            EndpointSystemId = Convert.ToInt64(adhocRequest.EndpointSystemId);
            ConvertLinkButtonToLabel = !applicationTypes.Contains(adhocRequest.ApplicationType);

            //if (ConvertLinkButtonToLabel)
            {
                AccessNameLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#666");
                AccessNameLabel.Style.Add("text-decoration", "none");
                AccessNameLabel.Style.Add("cursor", "text");
            }

            if (adhocRequest.ApplicationType == ApplicationType.HRMS_FLINS)
            {
                ConflictBusinessRoles(adhocRequest.UserID, adhocRequest.AdHocAccessRequestId, false);
            }

            if ((adhocRequest.ApplicationType == ApplicationType.MSSL_CRM || adhocRequest.ApplicationType == ApplicationType.MSSL_ISU || adhocRequest.ApplicationType == ApplicationType.EBS_CRM || adhocRequest.ApplicationType == ApplicationType.EBS_ISU) && (adhocRequest.AdhocActionType == AdHocActionType.NewAccount || adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
            {
                string applicationType = "EBS";
                if(adhocRequest.ApplicationType == ApplicationType.MSSL_CRM || adhocRequest.ApplicationType == ApplicationType.MSSL_ISU)
                {
                    applicationType = "MSSL";
                }
                if ((adhocRequest.ProgressStatus == AccessRequestProgress.Completed || adhocRequest.ProgressStatus == AccessRequestProgress.ManuallyResolved || adhocRequest.ProgressStatus == AccessRequestProgress.Withdrawn
                    || adhocRequest.ProgressStatus == AccessRequestProgress.RequestRejected || adhocRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute || adhocRequest.ProgressStatus == AccessRequestProgress.ExecutionFailed
                    || adhocRequest.ProgressStatus == AccessRequestProgress.Executing
                    ))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(adhocRequest.ConflictRemarks))
                        {
                            var data = UtilityMethods.Deserialize<List<PresentationConflictRemarks>>(adhocRequest.ConflictRemarks);
                            if (data != null && data.Count > 0)
                            {
                                MSSLEBSConflictRemark = data;
                                rgEBSMSSLConflictsRemarks.DataSource = data;
                                rgEBSMSSLConflictsRemarks.Rebind();
                                rgEBSMSSLConflictsRemarks.Visible = true;
                                divEBSMSSLConflictsRemarks.Visible = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    
                    SAPSODConflictBusinessRoles(adhocRequest.UserID, adhocRequest.AdHocAccessRequestId, false, applicationType);
                } 
            }
            if ((adhocRequest.ApplicationType == ApplicationType.MSSL || adhocRequest.ApplicationType == ApplicationType.EBS) && (adhocRequest.AdhocActionType == AdHocActionType.NewAccount || adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
            {
                string applicationType = "";
                var ep = DomainApiClient<EndPoint>.GetById(adhocRequest.EndpointSystemId.Value);
                applicationType = ep.ApplicationCode;

                if ((adhocRequest.ProgressStatus == AccessRequestProgress.Completed || adhocRequest.ProgressStatus == AccessRequestProgress.ManuallyResolved || adhocRequest.ProgressStatus == AccessRequestProgress.Withdrawn
                    || adhocRequest.ProgressStatus == AccessRequestProgress.RequestRejected || adhocRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute || adhocRequest.ProgressStatus == AccessRequestProgress.ExecutionFailed
                    || adhocRequest.ProgressStatus == AccessRequestProgress.Executing
                    ))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(adhocRequest.ConflictRemarks))
                        {
                            var data = UtilityMethods.Deserialize<List<PresentationConflictRemarks>>(adhocRequest.ConflictRemarks);
                            if (data != null && data.Count > 0)
                            {
                                MSSLEBSConflictRemark = data;
                                rgEBSMSSLConflictsRemarks.DataSource = data;
                                rgEBSMSSLConflictsRemarks.Rebind();
                                rgEBSMSSLConflictsRemarks.Visible = true;
                                divEBSMSSLConflictsRemarks.Visible = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    SAPHANASODConflictBusinessRoles(adhocRequest.UserID, adhocRequest.AdHocAccessRequestId, false, applicationType);
                }
            }

            #region Start Date End Date Added
            ValidFromRow.Visible = adhocRequest.StartDate.HasValue;
            this.lblValidFrom.Text = (adhocRequest.StartDate.HasValue) ? adhocRequest.StartDate.Value.ToString("dd-MM-yyyy") : "";
            //ValidToRow.Visible = adhocRequest.EndDate.HasValue;
            this.lblValidTo.Text = (adhocRequest.EndDate.HasValue) ? adhocRequest.EndDate.Value.ToString("dd-MM-yyyy") : "";
            #endregion

            #region Adding Requestor Comments
            if (!string.IsNullOrWhiteSpace(adhocRequest.Comments))
            {
                RequestorCommentsRow.Visible = true;
                adhocRequest.Comments = adhocRequest.Comments.Trim().Replace("\n", "<br/>");
                lblRequestorCommentstxt.Text = adhocRequest.Comments;

            }
            #endregion

            #region Withdraw Reason
            if (adhocRequest.RequestStatus == AccessRequestStatus.Withdrawn)
            {
                //var request = new AccessRequestFacade().GetAccessRequestById(adhocRequest.AccessRequestID);
                WithdrawCommentsRow.Visible = true;
                WithdrawReasonLabelLabel.Text = adhocRequest.AccessRequestComments?.Trim().Replace("\n", "<br/>");

                WithdrawnOnRow.Visible = true;
                WithdrawnOnLabel.Text = adhocRequest.Remarks;
            }
            #endregion

            if (adhocRequest.AdHocAccessRequests.AccountType == AccountType.Service && adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {

                var showGrid = adhocRequest.AdHocAccessRequests.GetSecurityQuestions == null ? false : true;
                TRServiceaAccQstns.Visible = showGrid;
                List<Dto.SecurityResponseGridData> list = new List<Dto.SecurityResponseGridData>();

                var data = adhocRequest.AdHocAccessRequests.GetSecurityQuestions != null ? adhocRequest.AdHocAccessRequests.GetSecurityQuestions.SecurityResponseDetails : null;
                if (data != null)
                {
                    data.ForEach(item =>
                    {
                        Dto.SecurityResponseGridData gridData = new Dto.SecurityResponseGridData();
                        gridData.Comments = item.Comments;
                        gridData.ComplianceResponse = item.Compliance.ToString();
                        gridData.Question = item.Question;
                        gridData.QuestionID = item.QuestionID;
                        list.Add(gridData);
                    });
                }
                SVCQstnGrid.SecurityResponse = list;
            }
            string action = "Add";
            if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                action = "Remove";
            }

            if (adhocRequest.ApplicationType == ApplicationType.VPN)
            {
                if (adhocRequest.EndpointSystemId.HasValue && adhocRequest.EndPoint != null)
                {
                    this.SystemNameLabel.Text = adhocRequest.EndPoint.DisplayName;
                }
                else
                {
                    this.SystemNameLabel.Text = UtilityMethods.GetEnumAttribute<Description>(adhocRequest.ApplicationType);

                }
                GenericPanel.Visible = true;

                if (adhocRequest.AdHocAccessRequests.AccountType == AccountType.Service && adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {

                    var showGrid = adhocRequest.AdHocAccessRequests.GetSecurityQuestions == null ? false : true;
                    trSVCVPNQstn.Visible = showGrid;
                    List<Dto.SecurityResponseGridData> list = new List<Dto.SecurityResponseGridData>();

                    var data = adhocRequest.AdHocAccessRequests.GetSecurityQuestions != null ? adhocRequest.AdHocAccessRequests.GetSecurityQuestions.SecurityResponseDetails : null;
                    if (data != null)
                    {
                        data.ForEach(item =>
                        {
                            Dto.SecurityResponseGridData gridData = new Dto.SecurityResponseGridData();
                            gridData.Comments = item.Comments;
                            gridData.ComplianceResponse = item.Compliance.ToString();
                            gridData.Question = item.Question;
                            gridData.QuestionID = item.QuestionID;
                            list.Add(gridData);
                        });
                    }
                    svcQstnGridVPN.SecurityResponse = list;


                }
                //this.VPNRequestIDLabel.Text = adhocRequest.AccessRequestID.ToString();
                //this.BulkRequestIDLinkButton.Text = adhocRequest.BulkRequestId != null ? Convert.ToString(adhocRequest.BulkRequestId) : string.Empty;
                //bulkrequesttr.Visible = !(string.IsNullOrWhiteSpace(this.BulkRequestIDLinkButton.Text));
                this.VPNPanel.Visible = true;
                OthersGrid.Visible = false;
            }
            else
            {
                if (adhocRequest.EndpointSystemId.HasValue && adhocRequest.EndPoint != null)
                {
                    this.SystemNameLabel.Text = adhocRequest.EndPoint.DisplayName;
                }
                else
                {
                    this.SystemNameLabel.Text = UtilityMethods.GetEnumAttribute<Description>(adhocRequest.ApplicationType);

                }
                this.GenericPanel.Visible = true;
            }
            vpnremarks.Text = RemarksLabel.Text = adhocRequest.Remarks;
            Remarkstr.Visible = !string.IsNullOrWhiteSpace(RemarksLabel.Text);
            vpnremarkstr.Visible = !string.IsNullOrWhiteSpace(vpnremarks.Text);
            vpnremarks.Visible = false;
            this.RequestIDLabel.Text = adhocRequest.AccessRequestID.ToString();
            this.bulkrequestgenerallinkbutton.Text = adhocRequest.BulkRequestId != null ? Convert.ToString(adhocRequest.BulkRequestId) : string.Empty;
            bulkrequesthidden.Value = adhocRequest.BulkRequestId != null ? Convert.ToString(adhocRequest.BulkRequestId) : string.Empty;
            bulkrequestidgeneraltr.Visible = !(string.IsNullOrWhiteSpace(this.bulkrequestgenerallinkbutton.Text));
            if (adhocRequest != null)
            {
                bool showAccessName = true;
                string hstName = adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndPoint.DisplayName : (adhocRequest.ApplicationType == ApplicationType.SRM ? "SRP" : UtilityMethods.GetEnumAttribute<Description>(adhocRequest.ApplicationType));
                string requestName = hstName + " - " + UtilityMethods.GetEnumAttribute<Description>(adhocRequest.AdhocActionType);
                string acctType = UtilityMethods.GetEnumAttribute<Description>(adhocRequest.AdHocAccessRequests.AccountType);
                this.RequestTypeLabel.Text = requestName;
                this.AccountTypeLabel.Text = acctType;

                #region SSIDS GUI & SMS

                if (adhocRequest.ApplicationType == ApplicationType.SSIDS_GUI || adhocRequest.ApplicationType == ApplicationType.SSIDS_SMS)
                {
                    AdHocSSIDSRequest SSIDSRequest = (AdHocSSIDSRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (SSIDSRequest != null)
                    {
                        lblLogonUserName.Text = SSIDSRequest?.DisplayAccountName;
                        EntitlementTypetr.Visible = true;
                        EntitlementTypeLabel.Text = UtilityMethods.GetEnumAttribute<Description>(SSIDSRequest.EntitlementType);
                        AccessNameLabel.Text = SSIDSRequest.Entitlement;
                        if (adhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                        {
                            var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", SSIDSRequest.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                            lblLogonUserName.Text = accountRecon?.AccountName;
                            LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        }
                        else
                        {
                            LogonUserNametr.Visible = false;
                        }
                    }
                }


                #endregion

                #region IDMS
                if (adhocRequest.ApplicationType == ApplicationType.IDMS)
                {
                    AdHocIACMRequest adHocIACMRequest = (AdHocIACMRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (adHocIACMRequest != null)
                    {
                        if(adHocIACMRequest.AdHocIACMRequestDetails != null && adHocIACMRequest.AdHocIACMRequestDetails.Count > 0)
                        {
                            DivIDMS.Visible = true;
                            RadGridIacm.DataSource = adHocIACMRequest.AdHocIACMRequestDetails.Select(item => new { EntitlementType = UtilityMethods.GetEnumAttribute<Description>(item.EntitlementType), Entitlement = item.Entitlement });
                            RadGridIacm.DataBind();
                            RadGridIacm.Visible = true;
                        }

                        lblLogonUserName.Text = adHocIACMRequest?.DisplayAccountName;
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                    }
                }

                #endregion

                #region ADS
                if (adhocRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)//|| adhocRequest.ApplicationType == ApplicationType.ADSDCLocal)
                {

                    tblgroup.Visible = true;
                    //trADGroup.Visible = true;
                    Session["EndpointSystemIdValue"] = adhocRequest.EndpointSystemId.Value;

                    AdHocADRequest ADRequest = (AdHocADRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (ADRequest != null)
                    {
                        trreqComment.Visible = !string.IsNullOrWhiteSpace(ADRequest.RequesterComment);
                        if (ADRequest.RequesterComment != null)
                            lblRequesterComment.Text = ADRequest.RequesterComment.Replace("\n", "<br/>");
                    }

                    AdHocADRequest adDetail = (AdHocADRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (null == adDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        return;
                    }
                    else
                    {
                        if(adhocRequest != null && adhocRequest.AccountType == AccountType.User && adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                            lblLogonUserName.Text = "";
                        else
                        {
                            lblLogonUserName.Text = adDetail?.DisplayAccountName;
                        }
                    }
                    this.AccessNameLabel.Text = adDetail.Entitlement;

                    if (adDetail != null && adDetail.AdHocADSharedAccount != null)
                    {
                        pnlShareAccountDetails.Visible = true;
                        lblAccountName.Text = adDetail.AdHocADSharedAccount.AccountName;
                        lblDescription.Text = adDetail.AdHocADSharedAccount.Description;
                        lblDisplayName.Text = adDetail.AdHocADSharedAccount.DisplayName;
                        lblOU.Text = adDetail.AdHocADSharedAccount.OU;


                        IList<object> listofrowsNeeded = new List<object>();
                        listofrowsNeeded.Add(AccountNamediv.ID);
                        listofrowsNeeded.Add(DisplayNamediv.ID);
                        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                        {
                            listofrowsNeeded.Add(Descriptiondiv.ID);
                        }
                        if (!string.IsNullOrWhiteSpace(lblOU.Text))
                        {
                            listofrowsNeeded.Add(Oudiv.ID);
                        }
                        RowsVisibility(listofrowsNeeded);
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblRequesterComment.Text.Replace("\n", "<br/>");
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adDetail.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                            lblLogonUserName.Text = accountRecon.AccountName;
                    }
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                    showAccessName = !ShowEntitlements(adDetail);
                    if (adhocRequest.AdhocActionType == AdHocActionType.NewAccount || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;
                        //this.Label3.Visible = false;
                        return;
                    }

                }
                #endregion
                #region SAP.Local

                if (adhocRequest.ApplicationType == ApplicationType.SAPLocal)
                {

                    tblgroup.Visible = true;
                    //trADGroup.Visible = true;
                    Session["EndpointSystemIdValue"] = adhocRequest.EndpointSystemId.Value;

                    AdHocSAPLocalRequest ADRequest = (AdHocSAPLocalRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (ADRequest != null)
                    {

                        if (ADRequest.RequesterComment != null)
                            lblRequesterComment.Text = ADRequest.RequesterComment.Replace("\n", "<br/>");
                    }

                    AdHocSAPLocalRequest adDetail = (AdHocSAPLocalRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (null == adDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        return;
                    }
                    else
                    {
                        lblLogonUserName.Text = adDetail?.DisplayAccountName;
                    }
                    this.AccessNameLabel.Text = adDetail.Entitlement;

                    if (adDetail != null && adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        pnlShareAccountDetails.Visible = true;
                        lblAccountName.Text = adDetail.AccountName;
                        lblDescription.Text = adDetail.Description;
                        lblDisplayName.Text = adDetail.DisplayName;

                        string configKeyValue = string.Empty;

                        if (adhocRequest.AdHocAccessRequests.AccountType == AccountType.User)
                        {
                            configKeyValue = ConfigConstants.AD_SAP_LOCAL_USER_ACCOUNT_OU;
                        }
                        else if (adhocRequest.AdHocAccessRequests.AccountType == AccountType.Service)
                        {
                            configKeyValue = ConfigConstants.AD_SAP_LOCAL_SERVICE_ACCOUNT_OU;
                        }
                        else if (adhocRequest.AdHocAccessRequests.AccountType == AccountType.Shared)
                        {
                            configKeyValue = ConfigConstants.AD_SAP_LOCAL_SHARED_ACCOUNT_OU;
                        }
                        else if (adhocRequest.AdHocAccessRequests.AccountType == AccountType.Secondary)
                        {
                            configKeyValue = ConfigConstants.AD_SAP_LOCAL_SECONDARY_ACCOUNT_OU;
                        }

                        if (!string.IsNullOrWhiteSpace(configKeyValue))
                        {
                            lblOU.Text = DI.Chakra.DBConfigHelper.MasterConfigHelper.GetConfigValue(configKeyValue, Product.IM)?.ConfigValue;
                        }

                        IList<object> listofrowsNeeded = new List<object>();
                        listofrowsNeeded.Add(AccountNamediv.ID);
                        listofrowsNeeded.Add(DisplayNamediv.ID);
                        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                        {
                            listofrowsNeeded.Add(Descriptiondiv.ID);
                        }
                        if (!string.IsNullOrWhiteSpace(lblOU.Text))
                        {
                            listofrowsNeeded.Add(Oudiv.ID);
                        }
                        RowsVisibility(listofrowsNeeded);
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblRequesterComment.Text.Replace("\n", "<br/>");
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adDetail.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                            lblLogonUserName.Text = accountRecon.AccountName;//new UserFacade().GetEndPointUserDetailByEndpointUserID(adDetail.EndpointUserId, "AD");

                    }
                    showAccessName = !ShowEntitlements(adDetail);
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                    //if (adhocRequest.AdhocActionType == AdHocActionType.NewAccount || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    //{
                    //    this.AccessNameLabel.Visible = false;
                    //    this.AccessNametr.Visible = AccessNameLabel.Visible;
                    //    this.Label3.Visible = false;
                    //    return;
                    //}
                }
                #endregion

                #region LNS
                //else if (adhocRequest.EntitlementDetailType == ApplicationType.LotusNotes)
                //{
                //    AdHocLNSRequest lnDetail = (AdHocLNSRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                //    if (null == lnDetail)
                //    {
                //        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                //        return;
                //    }
                //    this.AccessNameLabel.Text = lnDetail.Entitlement;
                //    if (lnDetail != null && lnDetail.AdHocLNSSharedAccount != null)
                //    {
                //        pnlShareAccountDetails.Visible = true;
                //        Oudiv.Visible = false;

                //        lblAccountName.Text = lnDetail.AdHocLNSSharedAccount.AccountName;
                //        lblDescription.Text = lnDetail.AdHocLNSSharedAccount.Description;
                //        lblDisplayNameCaption.Text = "Email Address";
                //        lblDisplayName.Text = lnDetail.AdHocLNSSharedAccount.EmailAddress;
                //        IList<object> listofrowsNeeded = new List<object>();
                //        listofrowsNeeded.Add(AccountNamediv.ID);
                //        listofrowsNeeded.Add(DisplayNamediv.ID);
                //        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                //        {
                //            listofrowsNeeded.Add(Descriptiondiv.ID);
                //        }
                //        RowsVisibility(listofrowsNeeded);
                //    }
                //    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                //    {
                //        lblLogonUserName.Text = new UserFacade().GetEndPointUserDetailByEndpointUserID(lnDetail.EndpointUserId, "LNS");
                //    }
                //    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                //    if (adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                //    {
                //        this.AccessNameLabel.Visible = false;
                //        this.AccessNametr.Visible = AccessNameLabel.Visible;

                //        this.Label3.Visible = false;
                //        return;
                //    }
                //}
                #endregion

                #region HRMSFLINS
                else if (adhocRequest.ApplicationType == ApplicationType.HRMS_FLINS)
                {

                    AdHocSAPHRFLINSRequest flinsDetail = (AdHocSAPHRFLINSRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (null == flinsDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        return;
                    }
                    lblLogonUserName.Text = flinsDetail?.DisplayAccountName;
                    var detail = flinsDetail.AdHocSAPHRFLINSRequestDetails;
                    FlinsEntitlementList = detail;
                    trHRMSFLINS.Visible = HRMSGRID.Visible = detail.Count > 0 ? true : false;
                    HRMSGRID.DataSource = detail;
                    HRMSGRID.DataBind();
                    HRMSGRID.MasterTableView.GetColumn("IsCompanyRetain").Visible = adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove;
                    if (ConflictRemark != null && ConflictRemark.Count > 0)
                    {
                        ConfilctsRemarktr.Visible = true;
                        RadGridConflicts.DataSource = ConflictRemark;
                        RadGridConflicts.Rebind();
                    }
                    else
                    {
                        ConfilctsRemarktr.Visible = false;
                    }

                    if (flinsDetail != null && flinsDetail.AdHocSAPHRFLINSSharedAccount != null)
                    {
                        pnlShareAccountDetails.Visible = true;
                        SAPUserTypeLabel.Text = Convert.ToString(flinsDetail.AdHocSAPHRFLINSSharedAccount.SAPUserType);
                        lblAccountName.Text = flinsDetail.AdHocSAPHRFLINSSharedAccount.AccountName;
                        lblDescription.Text = flinsDetail.AdHocSAPHRFLINSSharedAccount.Description;
                        IList<object> listofrowsNeeded = new List<object>();
                        listofrowsNeeded.Add(SAPUserTypediv.ID);
                        listofrowsNeeded.Add(AccountNamediv.ID);
                        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                        {
                            listofrowsNeeded.Add(Descriptiondiv.ID);
                        }
                        RowsVisibility(listofrowsNeeded);
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", flinsDetail.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                            lblLogonUserName.Text = accountRecon.AccountName;  //new UserFacade().GetEndPointUserDetailByEndpointUserID(flinsDetail.EndpointUserId, flinsDeta "SAP");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                    if (adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;
                        // this.Label3.Visible = false;
                        return;
                    }
                }
                #endregion

                #region FLINSBI
                else if (adhocRequest.ApplicationType == ApplicationType.FLINS_BI)
                {
                    AdHocSAPFLINSBIRequest sapFLINSBIDetail = (AdHocSAPFLINSBIRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (null == sapFLINSBIDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        return;
                    }
                    lblLogonUserName.Text = sapFLINSBIDetail?.DisplayAccountName;
                    this.AccessNameLabel.Text = sapFLINSBIDetail.Entitlement;
                    PositionLinkButton.Visible = false;
                    if (!string.IsNullOrWhiteSpace(sapFLINSBIDetail.Entitlement))
                    {
                        PositionLinkButton.Visible = true;
                    }
                    if (sapFLINSBIDetail != null && sapFLINSBIDetail.AdHocFLINSBISharedAccount != null)
                    {
                        pnlShareAccountDetails.Visible = true;
                        SAPUserTypeLabel.Text = Convert.ToString(sapFLINSBIDetail.AdHocFLINSBISharedAccount.SAPUserType);
                        lblAccountName.Text = sapFLINSBIDetail.AdHocFLINSBISharedAccount.AccountName;
                        lblDescription.Text = sapFLINSBIDetail.AdHocFLINSBISharedAccount.Description;
                        IList<object> listofrowsNeeded = new List<object>();
                        listofrowsNeeded.Add(SAPUserTypediv.ID);
                        listofrowsNeeded.Add(AccountNamediv.ID);
                        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                        {
                            listofrowsNeeded.Add(Descriptiondiv.ID);
                        }
                        RowsVisibility(listofrowsNeeded);
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", sapFLINSBIDetail.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                            lblLogonUserName.Text = accountRecon.AccountName;//new UserFacade().GetEndPointUserDetailByEndpointUserID(sapFLINSBIDetail.EndpointUserId, "SAP");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                }
                #endregion

                #region SAPPI
                else if (adhocRequest.ApplicationType == ApplicationType.SAP_PI)
                {
                    AdHocSAPPIRequest piDetail = (AdHocSAPPIRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (null == piDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        return;
                    }
                    this.AccessNameLabel.Text = piDetail.Entitlement;
                    if (piDetail != null && piDetail.AdHocSAPPISharedAccount != null)
                    {
                        pnlShareAccountDetails.Visible = true;
                        SAPUserTypeLabel.Text = Convert.ToString(piDetail.AdHocSAPPISharedAccount.SAPUserType);
                        lblAccountName.Text = piDetail.AdHocSAPPISharedAccount.AccountName;
                        lblDescription.Text = piDetail.AdHocSAPPISharedAccount.Description;
                        IList<object> listofrowsNeeded = new List<object>();
                        listofrowsNeeded.Add(SAPUserTypediv.ID);
                        listofrowsNeeded.Add(AccountNamediv.ID);
                        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                        {
                            listofrowsNeeded.Add(Descriptiondiv.ID);
                        }
                        RowsVisibility(listofrowsNeeded);
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", piDetail.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                            lblLogonUserName.Text = accountRecon.AccountName; //new UserFacade().GetEndPointUserDetailByEndpointUserID(piDetail.EndpointUserId, "SAP");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                }
                #endregion

                #region SAPGRC
                else if (adhocRequest.ApplicationType == ApplicationType.SAP_GRC)
                {
                    AdHocSAPGRCRequest grcDetail = (AdHocSAPGRCRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (null == grcDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        return;
                    }
                    lblLogonUserName.Text = grcDetail?.DisplayAccountName;

                    this.AccessNameLabel.Text = grcDetail.Entitlement;
                    PositionLinkButton.Visible = false;
                    if (!string.IsNullOrWhiteSpace(grcDetail.Entitlement))
                    {
                        PositionLinkButton.Visible = true;
                    }
                    if (grcDetail != null && grcDetail.AdHocSAPGRCSharedAccount != null)
                    {
                        pnlShareAccountDetails.Visible = true;
                        SAPUserTypeLabel.Text = Convert.ToString(grcDetail.AdHocSAPGRCSharedAccount.SAPUserType);
                        lblAccountName.Text = grcDetail.AdHocSAPGRCSharedAccount.AccountName;
                        lblDescription.Text = grcDetail.AdHocSAPGRCSharedAccount.Description;
                        IList<object> listofrowsNeeded = new List<object>();
                        listofrowsNeeded.Add(SAPUserTypediv.ID);
                        listofrowsNeeded.Add(AccountNamediv.ID);
                        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                        {
                            listofrowsNeeded.Add(Descriptiondiv.ID);
                        }
                        RowsVisibility(listofrowsNeeded);
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", grcDetail.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                            lblLogonUserName.Text = accountRecon.AccountName; //new UserFacade().GetEndPointUserDetailByEndpointUserID(grcDetail.EndpointUserId, "SAP");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                }
                #endregion

                #region PLRElearning
                //else if (adhocRequest.ApplicationType == ApplicationType.eLearning)
                //{
                //    AdHocPLRELearningRequest elDetail = (AdHocPLRELearningRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                //    if (null == elDetail)
                //    {
                //        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                //        return;
                //    }
                //    this.AccessNameLabel.Text = elDetail.Entitlement;
                //    if (elDetail != null && elDetail.AdHocSAPELearningSharedAccount != null)
                //    {
                //        pnlShareAccountDetails.Visible = true;
                //        SAPUserTypeLabel.Text = Convert.ToString(elDetail.AdHocSAPELearningSharedAccount.SAPUserType);
                //        lblAccountName.Text = elDetail.AdHocSAPELearningSharedAccount.AccountName;
                //        lblDescription.Text = elDetail.AdHocSAPELearningSharedAccount.Description;
                //        IList<object> listofrowsNeeded = new List<object>();
                //        listofrowsNeeded.Add(SAPUserTypediv.ID);
                //        listofrowsNeeded.Add(AccountNamediv.ID);
                //        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                //        {
                //            listofrowsNeeded.Add(Descriptiondiv.ID);
                //        }
                //        RowsVisibility(listofrowsNeeded);
                //    }
                //    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                //    {
                //        lblLogonUserName.Text = new UserFacade().GetEndPointUserDetailByEndpointUserID(elDetail.EndpointUserId, "SAP");
                //    }
                //    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                //}
                #endregion

                #region PERErecruitment
                //else if (adhocRequest.EntitlementDetailType == ApplicationType.eRecruitment)
                //{
                //    AdHocPERERecruitmentRequest elDetail = (AdHocPERERecruitmentRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                //    if (null == elDetail)
                //    {
                //        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                //        return;
                //    }
                //    this.AccessNameLabel.Text = elDetail.Entitlement;
                //    if (elDetail != null && elDetail.AdHocSAPERecruitmentSharedAccount != null)
                //    {
                //        pnlShareAccountDetails.Visible = true;
                //        SAPUserTypeLabel.Text = Convert.ToString(elDetail.AdHocSAPERecruitmentSharedAccount.SAPUserType);
                //        lblAccountName.Text = elDetail.AdHocSAPERecruitmentSharedAccount.AccountName;
                //        lblDescription.Text = elDetail.AdHocSAPERecruitmentSharedAccount.Description;
                //        IList<object> listofrowsNeeded = new List<object>();
                //        listofrowsNeeded.Add(SAPUserTypediv.ID);
                //        listofrowsNeeded.Add(AccountNamediv.ID);
                //        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                //        {
                //            listofrowsNeeded.Add(Descriptiondiv.ID);
                //        }
                //        RowsVisibility(listofrowsNeeded);
                //    }
                //    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                //    {
                //        lblLogonUserName.Text = new UserFacade().GetEndPointUserDetailByEndpointUserID(elDetail.EndpointUserId, "SAP");
                //    }
                //    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                //}
                #endregion

                #region VPN
                else if (adhocRequest.ApplicationType == ApplicationType.VPN)
                {
                    ADNametr.Visible = false;
                    LogonUserNametr.Visible = false;
                    AdHocVPNRequest vpnDetail = (AdHocVPNRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (null == vpnDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        return;
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && vpnDetail.UpdateOneKeyToken)
                    {
                        divUpdateOneKeyToken.Visible = true;
                    }
                    else
                    {
                        divUpdateOneKeyToken.Visible = false;
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.NewAccount || adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        divADID.Visible = true;
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("userID", adhocRequest.UserID.ToString());
                        var adUserInfo = DomainApiClient<Dto.ADSUserInfo>.GetByAction<Dto.ADSUserInfo>("GetActiveADAccountForVPN", parameter);
                        lblADIDForVPN.Text = adUserInfo?.sAMAccountName;
                        divADID.Visible = adUserInfo != null;
                        
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", vpnDetail.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                        {
                            ADNametr.Visible = !string.IsNullOrWhiteSpace(accountRecon.AccountName);
                            lblADName.Text = accountRecon.AccountName;
                        }   
                    }
                    if (!string.IsNullOrWhiteSpace(adhocRequest.Comments))
                    {
                        adhocRequest.Comments = adhocRequest.Comments.Replace("\n", "<br>");
                    }
                    
                    if (vpnDetail.UpdateOneKeyToken)
                    {
                        UpdateOneKeyToken = vpnDetail.UpdateOneKeyToken;
                        lblUpdateOneKeyToken.Text = UpdateOneKeyToken ? "Yes" : "No";
                    }
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                    //TODO:commented by sivanandan
                    //this.VPNApplicationsGrid.DataSource = vpnDetail.AdHocVPNRequestDetails.AsEnumerable().Where(c => !c.isOthers).Select(c => c);
                    this.VPNApplicationsGrid.DataSource = vpnDetail.AdHocVPNRequestDetails.AsEnumerable().Select(c => c);
                    this.VPNApplicationsGrid.DataBind();
                    divADID.Visible = !string.IsNullOrWhiteSpace(lblADIDForVPN.Text);
                    //this.OthersGrid.DataSource = vpnDetail.AdHocVPNRequestDetails.AsEnumerable().Where(c => c.isOthers).Select(c => c);
                    //this.OthersGrid.Rebind();
                    divVPNEntitlementLabel.Visible = vpnDetail.AdHocVPNRequestDetails.Count > 0;
                    //if (vpnDetail.AdHocVPNRequestDetails.AsEnumerable().Where(c => c.isOthers).Select(c => c).Count() > 0)
                    //{
                    //    OthersGrid.Visible = true;
                    //}
                    //else
                    //{
                    //    OthersGrid.Visible = false;
                    //}
                    if (vpnDetail.AdHocVPNRequestDetails.AsEnumerable().Select(c => c).Count() > 0)
                    {
                        VPNApplicationsGrid.Visible = true;
                    }
                    else
                    {
                        VPNApplicationsGrid.Visible = false;
                    }



                }
                #endregion

                #region InhouseSystems
                else if (adhocRequest.ApplicationType == ApplicationType.InhouseSystems)
                {
                    Dto.AdHocINHRequest inhDetail = (AdHocINHRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (null == inhDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        return;
                    }
                    else
                    {
                        lblLogonUserName.Text = inhDetail?.DisplayAccountName;
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        if (inhDetail.EndpointUserId != 0)
                        {
                            var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", inhDetail.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                            if (accountRecon != null)
                                lblLogonUserName.Text = accountRecon.AccountName; //new UserFacade().GetEndPointUserDetailByEndpointUserID(inhDetail.EndPointUserID, "INH");
                        }
                        else if (inhDetail.IsOffBoardRequest)
                        {
                            if (!string.IsNullOrWhiteSpace(inhDetail?.DisplayAccountName))
                            {
                                lblLogonUserName.Text = inhDetail?.DisplayAccountName;
                            }
                            else
                            {
                                //Commented below code because we should not take all the account.we made fix 30 Jan 2024 at design level(Request contains accountname).
                                //Dictionary<string, string> param = new Dictionary<string, string>();
                                //param.Add("userId", adhocRequest.UserID.ToString());
                                //param.Add("accountType", adhocRequest.AccountType.ToString());
                                //param.Add("endpointSystemId", adhocRequest.EndpointSystemId.ToString());
                                //var accountNames = DomainApiClient<Dto.AccountReconcilation>.GetByAction<List<Dto.AccountReconcilation>>(DomainApiAction.Provisioning.AccountReconcilation.GetByAccountTypeAndEndpointSystemId, param).Select(x => x.AccountName).ToList();

                                //if (accountNames != null & accountNames.Count > 0)
                                //{
                                //    lblLogonUserName.Text = string.Join(",", accountNames);
                                //}
                            }
                        }
                    }
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                    this.AccessNameLabel.Text = inhDetail.Entitlement;
                    if (inhDetail != null && inhDetail.AdHocINHSharedAccount != null)
                    {
                        pnlShareAccountDetails.Visible = true;
                        lblAccountName.Text = inhDetail.AdHocINHSharedAccount.AccountName;
                        lblDescription.Text = inhDetail.AdHocINHSharedAccount.Description;
                        lblDisplayName.Text = inhDetail.AdHocINHSharedAccount.DisplayName;
                        IList<object> listofrowsNeeded = new List<object>();
                        listofrowsNeeded.Add(AccountNamediv.ID);
                        listofrowsNeeded.Add(DisplayNamediv.ID);
                        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                        {
                            listofrowsNeeded.Add(Descriptiondiv.ID);
                        }
                        RowsVisibility(listofrowsNeeded);
                    }
                    else //UserAccount
                    {
                        Dto.AccountReconcilation accountRecon = null;
                        if (adhocRequest.EndPoint.CanAutomate && adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                        {
                            var accountRecons = DomainApiClient<AccountReconcilation>.GetByAction<List<AccountReconcilation>>(DomainApiAction.Provisioning.AccountReconcilation.GetByAccountTypeAndApplicationType,
                                 new Dictionary<string, string> { { "userId", adhocRequest.UserID.ToString() },
                                                   { "accountType", AccountType.User.ToString() },
                                                   { "applicationType",ApplicationType.ADSingaporePowerLocal.ToString() }
                                                               });
                            if (accountRecons != null && accountRecons.Count > 0)
                                accountRecon = accountRecons.FirstOrDefault();

                            if (accountRecon != null)
                            {
                                lblLogonUserName.Text = accountRecon.AccountName;
                                LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                            }
                        }
                    }
                }
                #endregion

                #region Miscellaneous
                //else if (adhocRequest.EntitlementDetailType == ApplicationType.Miscellaneous)
                //{
                //    this.AccessNameLabel.Text = adhocRequest.MiscSystem.SystemName;
                //    AdHocMiscRequest adhocMisc = (AdHocMiscRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                //    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                //    {
                //        var miscSystem = new InhouseSystemFacade().GetMiscSystemById(adhocRequest.AdHocAccessRequests.MiscSystemId.Value);
                //        if (miscSystem.MiscSystemType == MiscSystemType.ExternalEmailAccess)
                //        {
                //            lblLogonUserName.Text = new UserFacade().GetEndPointUserDetailByEndpointUserID(adhocMisc.EndpointUserId, "LNS");
                //        }
                //        else
                //        {
                //            lblLogonUserName.Text = new UserFacade().GetEndPointUserDetailByEndpointUserID(adhocMisc.EndpointUserId, "AD");
                //        }
                //    }
                //    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                //}
                #endregion

                #region GEMS
                else if (adhocRequest.ApplicationType == ApplicationType.GEMS)
                {
                    AdHocGEMSRequest adHocGEMSRequest = (AdHocGEMSRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;
                    }
                    else
                    {
                        if (adHocGEMSRequest != null)
                        {
                            this.AccessNameLabel.Text = this.AccessNameLabel.Text = adHocGEMSRequest.Entitlement;
                            this.AccessNameLabel.Visible = string.IsNullOrWhiteSpace(adHocGEMSRequest.Entitlement) ? false : true;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                        }
                        else
                            this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;
                        //Label3.Visible = this.AccessNameLabel.Visible;
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adHocGEMSRequest.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                            lblLogonUserName.Text = accountRecon.AccountName;//new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocGEMSRequest.EndpointUserId, "GEMS");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                    if (adHocGEMSRequest != null && adHocGEMSRequest.AdHocGEMSSharedAccount != null)
                    {
                        pnlShareAccountDetails.Visible = true;
                        lblAccountName.Text = adHocGEMSRequest.AdHocGEMSSharedAccount.AccountName;
                        lblDescription.Text = adHocGEMSRequest.AdHocGEMSSharedAccount.Description;
                        lblDisplayName.Text = adHocGEMSRequest.AdHocGEMSSharedAccount.DisplayName;
                        lblOU.Text = adHocGEMSRequest.AdHocGEMSSharedAccount.EmailAddress;
                        lblOUCaption.InnerText = "Email:";
                        IList<object> listofrowsNeeded = new List<object>();
                        listofrowsNeeded.Add(AccountNamediv.ID);
                        listofrowsNeeded.Add(Oudiv.ID);
                        listofrowsNeeded.Add(DisplayNamediv.ID);
                        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                        {
                            listofrowsNeeded.Add(Descriptiondiv.ID);
                        }
                        RowsVisibility(listofrowsNeeded);
                    }
                }
                #endregion

                #region MSSL
                else if (adhocRequest.ApplicationType == ApplicationType.MSSL_ISU || adhocRequest.ApplicationType == ApplicationType.MSSL_CRM)// || adhocRequest.ApplicationType == ApplicationType.MSSL_BI)
                {
                    var data = adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (data is AdHocMSSLRequest)
                    {
                        AdHocMSSLRequest adHocMSSLRequest = (AdHocMSSLRequest)data;
                        lblLogonUserName.Text = adHocMSSLRequest?.DisplayAccountName;
                        // if (adHocMSSLRequest.Entitlement == EntitlementType.Position)
                        this.SystemNameLabel.Text = "MSSL ISU & CRM";

                        if (MSSLEBSConflictRemark != null && MSSLEBSConflictRemark.Count > 0)
                        {
                            divEBSMSSLConflictsRemarks.Visible = true;
                            rgEBSMSSLConflictsRemarks.DataSource = MSSLEBSConflictRemark;
                            rgEBSMSSLConflictsRemarks.Rebind();
                        }
                        else
                        {
                            //divEBSMSSLConflictsRemarks.Visible = false;
                        }

                        if (adHocMSSLRequest != null && !string.IsNullOrWhiteSpace(adHocMSSLRequest.Position))
                        {
                            PositionLinkButton.Visible = true;
                        }
                        if (adHocMSSLRequest != null && adhocRequest.AccountType == AccountType.User && (adhocRequest.AdhocActionType == AdHocActionType.NewAccount || (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocMSSLRequest.IsReallocate)))
                        {
                            if (!string.IsNullOrWhiteSpace(adHocMSSLRequest.PositionPrefix))
                            {
                                lblPrefix.Text = adHocMSSLRequest.PositionPrefix;
                                PositionPrefix.Visible = true;
                            }
                        }
                        if (adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                        {
                            this.AccessNameLabel.Visible = false;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                        }
                        else
                        {
                            if (adHocMSSLRequest != null)
                            {
                                if (adHocMSSLRequest.IsReallocate && adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                                {
                                    divReallocate.Visible = true;
                                    lblIsReAllocate.Text = adHocMSSLRequest.IsReallocate.GetBoolToString();
                                }
                                //this.AccessNameLabel.Text = adHocMSSLRequest.Entitlement;
                                this.AccessNameLabel.Text = string.IsNullOrEmpty(adHocMSSLRequest.Position) ? "Roles : " + adHocMSSLRequest.Roles : string.IsNullOrEmpty(adHocMSSLRequest.Roles) ? "Position : " + adHocMSSLRequest.Position : "Position : " + adHocMSSLRequest.Position + "<br>" + "Roles    : " + adHocMSSLRequest.Roles;
                                this.AccessNameLabel.Visible = string.IsNullOrWhiteSpace(adHocMSSLRequest.Entitlement) ? false : true;
                                this.AccessNametr.Visible = AccessNameLabel.Visible;
                                //if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocMSSLRequest.Entitlement == adHocMSSLRequest.Position)
                                //{
                                //    if (adHocMSSLRequest.IsReallocate)
                                //    {
                                //        this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Reallocate Position)";
                                //    }
                                //    else
                                //    {
                                //        this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Add Position)";
                                //    }
                                //}
                                if (!string.IsNullOrWhiteSpace(this.AccessNameLabel.Text))
                                {
                                    PositionLinkButton.Visible = true;
                                }
                                if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocMSSLRequest.IsReallocate)
                                {
                                    this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Reallocate Position)";
                                }
                            }
                            else
                                this.AccessNameLabel.Visible = false;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                            //Label3.Visible = this.AccessNameLabel.Visible;
                            if (adHocMSSLRequest != null && adHocMSSLRequest.AdHocSAPMSSLSharedAccount != null)
                            {
                                pnlShareAccountDetails.Visible = true;
                                SAPUserTypeLabel.Text = Convert.ToString(adHocMSSLRequest.AdHocSAPMSSLSharedAccount.SAPUserType);
                                lblAccountName.Text = adHocMSSLRequest.AdHocSAPMSSLSharedAccount.AccountName;
                                lblDescription.Text = adHocMSSLRequest.AdHocSAPMSSLSharedAccount.Description;
                                IList<object> listofrowsNeeded = new List<object>();
                                listofrowsNeeded.Add(SAPUserTypediv.ID);
                                listofrowsNeeded.Add(AccountNamediv.ID);
                                if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                                {
                                    listofrowsNeeded.Add(Descriptiondiv.ID);
                                }
                                RowsVisibility(listofrowsNeeded);
                            }
                        }
                        if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                        {
                            var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adHocMSSLRequest.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                            if (accountRecon != null)
                                lblLogonUserName.Text = accountRecon.AccountName; //new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocMSSLRequest.EndpointUserId, "SAP");
                        }
                    }

                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                }
                #endregion

                #region EBS
                else if (adhocRequest.ApplicationType == ApplicationType.EBS_ISU || adhocRequest.ApplicationType == ApplicationType.EBS_CRM)// || adhocRequest.ApplicationType == ApplicationType.EBS_BI)
                {
                    var data = adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (data is AdHocEBSRequest)
                    {
                        AdHocEBSRequest adHocEBSRequest = (AdHocEBSRequest)data;
                        lblLogonUserName.Text = adHocEBSRequest?.DisplayAccountName;
                        
                        if (MSSLEBSConflictRemark != null && MSSLEBSConflictRemark.Count > 0)
                        {
                            divEBSMSSLConflictsRemarks.Visible = true;
                            rgEBSMSSLConflictsRemarks.DataSource = MSSLEBSConflictRemark;
                            rgEBSMSSLConflictsRemarks.Rebind();
                        }

                        if (adHocEBSRequest.Entitlement == adHocEBSRequest.Position)
                            this.SystemNameLabel.Text = "EBS ISU & CRM";
                        if (adHocEBSRequest != null && adhocRequest.AccountType == AccountType.User && (adhocRequest.AdhocActionType == AdHocActionType.NewAccount || (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocEBSRequest.IsReallocate)))
                        {
                            if (adHocEBSRequest != null && !string.IsNullOrWhiteSpace(adHocEBSRequest.Position))
                            {
                                PositionLinkButton.Visible = true;
                            }
                            if (!string.IsNullOrWhiteSpace(adHocEBSRequest.PositionPrefix))
                            {
                                lblPrefix.Text = adHocEBSRequest.PositionPrefix;
                                PositionPrefix.Visible = true;
                            }
                        }
                        if (adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                        {
                            this.AccessNameLabel.Visible = false;
                            this.AccessNametr.Visible = string.IsNullOrWhiteSpace(this.AccessNameLabel.Text) ? false : true;
                        }
                        else
                        {
                            if (adHocEBSRequest != null)
                            {
                                //this.AccessNameLabel.Text = adHocEBSRequest.Entitlement;
                                this.AccessNameLabel.Text = string.IsNullOrEmpty(adHocEBSRequest.Position) ? "Roles : " + adHocEBSRequest.Roles : string.IsNullOrEmpty(adHocEBSRequest.Roles) ? "Position : " + adHocEBSRequest.Position : "Position : " + adHocEBSRequest.Position + "<br>" + "Roles    : " + adHocEBSRequest.Roles;
                                this.AccessNameLabel.Visible = string.IsNullOrWhiteSpace(adHocEBSRequest.Entitlement) ? false : true;
                                this.AccessNametr.Visible = AccessNameLabel.Visible;
                                //if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocEBSRequest.Entitlement == adHocEBSRequest.Position)
                                //{
                                //    if (adHocEBSRequest.IsReallocate)
                                //    {
                                //        this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Reallocate Position)";
                                //    }
                                //    else
                                //    {
                                //        this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Add Position)";
                                //    }
                                //}
                                if (!string.IsNullOrWhiteSpace(this.AccessNameLabel.Text))
                                {
                                    PositionLinkButton.Visible = true;
                                }
                                if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocEBSRequest.IsReallocate)
                                {
                                    this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Reallocate Position)";
                                }
                            }
                            else
                                this.AccessNameLabel.Visible = false;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                            //Label3.Visible = this.AccessNameLabel.Visible;
                            if (adHocEBSRequest != null && adHocEBSRequest.AdHocSAPEBSSharedAccount != null)
                            {
                                pnlShareAccountDetails.Visible = true;
                                SAPUserTypeLabel.Text = Convert.ToString(adHocEBSRequest.AdHocSAPEBSSharedAccount.SAPUserType);
                                lblAccountName.Text = adHocEBSRequest.AdHocSAPEBSSharedAccount.AccountName;
                                lblDescription.Text = adHocEBSRequest.AdHocSAPEBSSharedAccount.Description;
                                IList<object> listofrowsNeeded = new List<object>();
                                listofrowsNeeded.Add(SAPUserTypediv.ID);
                                listofrowsNeeded.Add(AccountNamediv.ID);
                                if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                                {
                                    listofrowsNeeded.Add(Descriptiondiv.ID);
                                }
                                RowsVisibility(listofrowsNeeded);
                            }
                        }
                        if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                        {
                            var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adHocEBSRequest.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                            if (accountRecon != null)
                                lblLogonUserName.Text = accountRecon.AccountName; //new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocEBSRequest.EndpointUserId, "SAP");
                        }
                    }

                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                }
                #endregion

                #region NPAC
                else if (adhocRequest.ApplicationType == ApplicationType.NPAC)
                {
                    AdHocNPACRequest adHocNPACRequest = (AdHocNPACRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    lblLogonUserName.Text = adHocNPACRequest?.DisplayAccountName;
                    if (adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = string.IsNullOrWhiteSpace(this.AccessNameLabel.Text) ? false : true;
                    }
                    else
                    {
                        if (adHocNPACRequest != null)
                        {
                            this.AccessNameLabel.Text = this.AccessNameLabel.Text = "Level " + adHocNPACRequest.Entitlement;
                            this.AccessNameLabel.Visible = string.IsNullOrWhiteSpace(adHocNPACRequest.Entitlement) ? false : true;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                            divNPACSection.Visible = divNPACUnit.Visible = true;
                            lblNPACSection.Text = adHocNPACRequest.Section;
                            lblNPACUnit.Text = adHocNPACRequest.Unit;
                        }
                        else
                            this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;
                        // Label3.Visible = this.AccessNameLabel.Visible;
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adHocNPACRequest.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                            lblLogonUserName.Text = accountRecon.AccountName; //new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocNPACRequest.EndpointUserId, "NPAC");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                }
                #endregion

                #region SAPEP
                else if (adhocRequest.ApplicationType == ApplicationType.SAP_EP || adhocRequest.ApplicationType == ApplicationType.CE)// || adhocRequest.ApplicationType == ApplicationType.InternetPortal)
                {

                    AdhocSAPEPRequest adHocSAPEPRequest = (AdhocSAPEPRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();

                    if (null == adHocSAPEPRequest)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        return;
                    }
                    lblLogonUserName.Text = adHocSAPEPRequest?.DisplayAccountName;
                    if (adHocSAPEPRequest != null && !Page.ToString().Contains("myrequestdetails"))
                    {
                        if (!string.IsNullOrWhiteSpace(adHocSAPEPRequest.Groups))
                        {

                            // AccessNameLabel.Text = adHocSAPEPRequest.Groups;
                        }
                        if ((!string.IsNullOrWhiteSpace(adHocSAPEPRequest.Roles) && (!string.IsNullOrWhiteSpace(adHocSAPEPRequest.Roles))))
                        {
                            // AccessNameLabel.Text = adHocSAPEPRequest.Roles + "," + adHocSAPEPRequest.Groups;
                        }
                    }
                    this.RequestInfoLabel.Text = adHocSAPEPRequest?.RequestDetail?.Replace("\n", "<br/>");
                    SAPEPRequestInfotr.Visible = string.IsNullOrWhiteSpace(adHocSAPEPRequest?.RequestDetail) ? false : true;
                    if (adHocSAPEPRequest != null && adHocSAPEPRequest.AdHocSAPEPSharedAccount != null)
                    {
                        pnlShareAccountDetails.Visible = true;
                        lblAccountName.Text = adHocSAPEPRequest.AdHocSAPEPSharedAccount.AccountName;
                        lblDescription.Text = adHocSAPEPRequest.AdHocSAPEPSharedAccount.Description;
                        IList<object> listofrowsNeeded = new List<object>();
                        listofrowsNeeded.Add(AccountNamediv.ID);
                        if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                        {
                            listofrowsNeeded.Add(Descriptiondiv.ID);
                        }
                        RowsVisibility(listofrowsNeeded);
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adHocSAPEPRequest.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                            lblLogonUserName.Text = accountRecon.AccountName; //new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocSAPEPRequest.EndpointUserId, "SAPEP");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);

                    if (adHocSAPEPRequest != null)
                    {
                        if (!string.IsNullOrWhiteSpace(adHocSAPEPRequest.Groups))
                        {
                            trPortalGrp.Visible = true;
                            lblSelectedGrp.Text = adHocSAPEPRequest.Groups.Replace(",", "<br/>");
                        }
                        if (!string.IsNullOrWhiteSpace(adHocSAPEPRequest.Roles))
                        {
                            trPortalrole.Visible = true;
                            lblSelectedRole.Text = adHocSAPEPRequest.Roles.Replace(",", "<br/>");
                        }
                    }
                }
                #endregion

                #region O365
                else if (adhocRequest.ApplicationType == ApplicationType.Office365)
                {
                    AdHocO365Request adhocO365Request = (AdHocO365Request)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (adhocO365Request != null)
                    {
                        lblLogonUserName.Text = adhocO365Request?.DisplayAccountName;
                        if (adhocO365Request.LicenseAction != LicenseActionType.None)
                        {
                            trLicenseAction.Visible = true;
                            lblLicenseAction.Text = adhocO365Request.LicenseAction.ToString();
                            if (adhocO365Request.LicenseInfo != null)
                            {
                                trLicenseInfo.Visible = true;
                                ServicePlanGrid.Visible = true;
                                ServicePlanGrid.DataSource = adhocO365Request.LicenseInfo;
                                ServicePlanGrid.DataBind();
                            }
                        }
                        if (adhocO365Request.ExternalEmailAction != ExternalEmailActionType.None)
                        {
                            trAlloExternalEmail.Visible = true;
                            lblExternalEmail.Text = adhocO365Request.ExternalEmailAction == ExternalEmailActionType.Enable ? "Yes" : "No";
                        }
                        if (adhocO365Request.MailBoxConversionType != MailBoxConversionType.None)
                        {
                            if ((adhocRequest.AdHocAccessRequests.AccountType == AccountType.Shared || adhocRequest.AdHocAccessRequests.AccountType == AccountType.Secondary))
                            {
                                string configKeyValue = string.Empty;

                                trMailboxConversionType.Visible = true;
                                if (adhocO365Request.MailBoxConversionType == MailBoxConversionType.SharedToUser)
                                {
                                    displayMBCT.InnerText = "Mail box Type:";
                                    configKeyValue = ConfigConstants.RegularMailboxText;
                                }
                                if (adhocO365Request.MailBoxConversionType == MailBoxConversionType.UserToShared)
                                {
                                    displayMBCT.InnerText = "Mail box Type:";
                                    configKeyValue = ConfigConstants.SharedMailboxText;
                                }

                                if (!string.IsNullOrWhiteSpace(configKeyValue))
                                {
                                    lblMailboxConversion.Text = MasterConfigHelper.GetConfigValue(configKeyValue, Product.IACM).ConfigValue;
                                }

                            }
                            //else
                            //{
                            //    trMailboxConversionType.Visible = true;
                            //    lblMailboxConversion.Text = UtilityMethods.GetEnumAttribute<Description>(adhocO365Request.MailBoxConversionType);
                            //    if (adhocRequest.AdHocAccessRequests.AccountType == AccountType.Shared && adhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                            //    {
                            //        lblMailboxConversion.Text = adhocO365Request.MailBoxConversionType == MailBoxConversionType.UserToShared ? "License Shared Mail" : "Internal Shared Mail";
                            //    }
                            //}
                        }
                        if (adhocO365Request.SharedMailBoxAction != SharedMailBoxActionType.None)
                        {
                            trSharedMailboxactionType.Visible = true;
                            lblSharedMailboxAction.Text = adhocO365Request.SharedMailBoxAction.ToString();
                            if (adhocO365Request.Members != null)
                            {
                                trMembers.Visible = true;
                                List<string> Result = new List<string>();
                                Result.Add(string.Join(",", adhocO365Request.Members.Select(item => item.Value).ToList()));
                                MembersGrid.DataSource = Result;
                                MembersGrid.DataBind();
                            }
                        }

                        if (adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                        {
                            trLicenseRmark.Visible = true;
                            LicenseRemarks.Text = MasterConfigHelper.GetConfigValue(ConfigConstants.AdhocLicenseMsg, Product.IACM).ConfigValue;
                        }
                        lblDisplayName.Text = string.Empty;
                        lblAliasName.Text = string.Empty;
                        if (adhocO365Request.AdHocO365SharedAccount != null && adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                        {
                            pnlShareAccountDetails.Visible = true;
                            lblDisplayName.Text = adhocO365Request.AdHocO365SharedAccount.DisplayName;
                            lblAliasName.Text = adhocO365Request.AdHocO365SharedAccount.AccountName;
                        }
                        if (adhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                        {
                            pnlShareAccountDetails.Visible = true;
                            //var userInfo = DomainApiClient<Dto.Office365UserInfo>.GetByAction<Dto.Office365UserInfo>(DomainApiAction.Endpoint.O365.GetO365UserByUserId,
                            //                                                    new Dictionary<string, string> {    { "userId", adhocO365Request.EndpointUserId.ToString() },
                            //                                                        { "accountType", ((int)adhocRequest.AccountType).ToString() },
                            //                                                        { "endpointsystemid", adhocRequest.EndpointSystemId.ToString() }
                            //                                                    });

                            var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adhocO365Request.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });

                            if (accountRecon != null)
                            {
                                LogonUserNametr.Visible = true;
                                lblLogonUserName.Text = accountRecon.FullName;
                                lblDisplayName.Text = accountRecon.FullName;
                                var temp = accountRecon.AccountName.Split('@');
                                lblAliasName.Text = temp.Length > 0 ? temp[0] : accountRecon.AccountName;
                            }
                        }
                        IList<object> listofrowsNeeded = new List<object>();
                        string id = DisplayNamediv.ID;
                        if (!string.IsNullOrWhiteSpace(lblDisplayName.Text))
                        {
                            listofrowsNeeded.Add(DisplayNamediv.ID);
                        }

                        if (!string.IsNullOrWhiteSpace(lblAliasName.Text))
                        {
                            listofrowsNeeded.Add(AliasNamediv.ID);
                        }
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        RowsVisibility(listofrowsNeeded);
                    }
                }
                #endregion

                #region SMS
                else if (adhocRequest.ApplicationType == ApplicationType.OneCard)
                {
                    AdHocSMSRequest adhocSMSRequest = (AdHocSMSRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (adhocSMSRequest != null)
                    {
                        lblLogonUserName.Text = adhocSMSRequest?.DisplayAccountName;
                        IList<object> listofrowsNeeded = new List<object>();
                        if (adhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                        {
                            listofrowsNeeded.Add(TRSMSAccessGrid.ID);
                            RowsVisibility(listofrowsNeeded);
                            SMSObjGrid.Visible = true;
                            SMSObjGrid.DataSource = adhocSMSRequest.GridInfo;
                            SMSObjGrid.DataBind();
                        }
                        if (adhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                        {
                            var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adhocSMSRequest.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                            if (accountRecon != null)
                                lblLogonUserName.Text = accountRecon.AccountName;
                        }
                        AccessNameLabel.Text = adhocSMSRequest.Entitlement;
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                    }
                }
                #endregion

                #region SRM
                else if (adhocRequest.ApplicationType == ApplicationType.SRM)
                {
                    AdHocSAPSRMRequest adhocSRMRequest = (AdHocSAPSRMRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (adhocSRMRequest != null)
                    {
                        IList<object> listofrowsNeeded = new List<object>();
                        if (adhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                            listofrowsNeeded.Add(TRSRMAccessGrid.ID);
                        RowsVisibility(listofrowsNeeded);
                        SRMObjGrid.Visible = true;
                        //AccessNameLabel.Text = adhocSRMRequest.Entitlement;
                        SRMObjGrid.DataSource = adhocSRMRequest.AdHocSAPSRMRequestDetails;
                        SRMObjGrid.DataBind();
                        lblLogonUserName.Text = adhocSRMRequest.AccountName;
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        if (adhocRequest.BulkRequestId.HasValue == false && adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                        {
                            if (adhocRequest.ProgressStatus != AccessRequestProgress.Completed)
                            {
                                SRMObjGrid.Columns.FindByUniqueName("Details").Visible = true;
                            }
                        }
                    }
                }
                #endregion
                #region NOVA
                else if (adhocRequest.ApplicationType == ApplicationType.EAM)
                {
                    var adhocNovaRequest = (AdHocNovaRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (adhocNovaRequest != null)
                    {
                        lblLogonUserName.Text = adhocNovaRequest?.DisplayAccountName;
                        if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                        {
                            var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adhocNovaRequest.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                            if (accountRecon != null)
                            {
                                lblLogonUserName.Text = accountRecon.AccountName;
                            }
                        }

                        if (adhocRequest.AdhocActionType == AdHocActionType.NewAccount || adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                        {
                            this.pnlNova.Visible = true;
                        }

                        this.NovaRoleGrid.Visible = true;
                        this.NovaRoleGrid.DataSource = adhocNovaRequest.RequestDetails;
                        this.NovaRoleGrid.Rebind();

                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);

                        if (adhocNovaRequest.AdHocSAPSharedAccountDetail != null)
                        {
                            pnlShareAccountDetails.Visible = true;
                            SAPUserTypeLabel.Text = Convert.ToString(adhocNovaRequest.AdHocSAPSharedAccountDetail.SAPUserType);
                            lblAccountName.Text = adhocNovaRequest.AdHocSAPSharedAccountDetail.AccountName;
                            lblDescription.Text = adhocNovaRequest.AdHocSAPSharedAccountDetail.Description;
                            lblDisplayName.Text = adhocNovaRequest.AdHocSAPSharedAccountDetail.DisplayName;
                            IList<object> listofrowsNeeded = new List<object>();
                            listofrowsNeeded.Add(SAPUserTypediv.ID);
                            listofrowsNeeded.Add(AccountNamediv.ID);
                            listofrowsNeeded.Add(Descriptiondiv.ID);
                            listofrowsNeeded.Add(DisplayNamediv.ID);
                            RowsVisibility(listofrowsNeeded);
                        }
                    }
                }
                #endregion
                #region SkypeForBusiness
                else if (adhocRequest.ApplicationType == ApplicationType.MicrosoftTeams)
                {
                    var sfbDetail = (AdhocSkypeForBusinessRequest)adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (null == sfbDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                        return;
                    }
                    else
                    {
                        lblLogonUserName.Text = sfbDetail?.DisplayAccountName;
                    }
                    if (adhocRequest.AdhocActionType == AdHocActionType.NewAccount || adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", sfbDetail.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                        if (accountRecon != null)
                        {
                            lblLogonUserName.Text = accountRecon.AccountName;
                        }
                        divEntitlementType.Visible = true;
                        lblEntitlementType.Text = sfbDetail.EntitlementType.ToString();
                        if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && sfbDetail.EntitlementType == EntitlementType.Default)
                        {
                            lblEntitlementType.Text = $"{sfbDetail.EntitlementType.ToString()} (Reserved Number)";
                        }
                        pnlSkypeForBusiness.Visible = true;
                        userLineURI.Text = sfbDetail.DisplayCurrentLineURI;
                        if (sfbDetail.EntitlementType == EntitlementType.ResponseGroup)
                        {
                            pnlResponseGroup.Visible = true;
                            ResponseGroupGrid.Visible = true;
                            ResponseGroupGrid.DataSource = sfbDetail.SkypeForBusinessResponseGroups;
                            ResponseGroupGrid.DataBind();
                            divPolicy.Visible = false;
                        }
                        //else if (sfbDetail.EntitlementType == EntitlementType.Move)
                        //{
                        //    var status = string.Empty;
                        //    if (sfbDetail.MoveToOnline != null)
                        //    {
                        //        status = sfbDetail.MoveToOnline.Value ? "To Online" : $"To OnPrem ({sfbDetail.RegistrarPoolName})";
                        //    }

                        //    lblEntitlementType.Text = $"{sfbDetail.EntitlementType.ToString()} {status}";
                        //}
                        //else if (sfbDetail.EntitlementType == EntitlementType.DCC)
                        //{
                        //    divPolicy.Visible = true;
                        //    lblLineURI.Text = sfbDetail.LineUri;

                        //}
                        else
                        {
                            pnlResponseGroup.Visible = false;
                            divPolicy.Visible = false;
                            //lblVoicePolicy.Text = sfbDetail.VoicePolicy;
                            //lblVoiceRoutingPolicy.Text = sfbDetail.VoiceRoutingPolicy;
                            //lblDialPlan.Text = sfbDetail.DialPlan;
                            lblLineURI.Text = sfbDetail.LineUri;
                            if (adhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                            {
                                var sfbUser = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetById(sfbDetail.EndpointUserId);
                                if (sfbUser != null)
                                {
                                    userLineURI.Text = sfbUser.LineURI;
                                }
                            }
                            else
                            {
                                if (sfbDetail.SkypeForBusinessResponseGroups.Count > 0)
                                {
                                    pnlResponseGroup.Visible = true;
                                    ResponseGroupGrid.Visible = true;
                                    ResponseGroupGrid.DataSource = sfbDetail.SkypeForBusinessResponseGroups;
                                    ResponseGroupGrid.DataBind();
                                }
                            }
                        }
                        if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && sfbDetail.EntitlementType == EntitlementType.Default)
                        {
                            divPolicy.Visible = true;
                        }
                    }
                }
                #endregion
                #region SAPHANA
                else if (adhocRequest.ApplicationType == ApplicationType.EBS || adhocRequest.ApplicationType == ApplicationType.MSSL)
                {
                    var data = adhocRequest.AdHocAccessRequests.GetEntitlementDetail();
                    if (data is AdHocSAPHANARequest)
                    {
                        var adHocSAPHANARequest = (AdHocSAPHANARequest)data;
                        lblLogonUserName.Text = adHocSAPHANARequest?.DisplayAccountName;

                        if (MSSLEBSConflictRemark != null && MSSLEBSConflictRemark.Count > 0)
                        {
                            divEBSMSSLConflictsRemarks.Visible = true;
                            rgEBSMSSLConflictsRemarks.DataSource = MSSLEBSConflictRemark;
                            rgEBSMSSLConflictsRemarks.Rebind();
                        }

                        if (adHocSAPHANARequest.Entitlement == adHocSAPHANARequest.Position)
                            this.SystemNameLabel.Text = "EBS";
                        if (adHocSAPHANARequest != null && adhocRequest.AccountType == AccountType.User && (adhocRequest.AdhocActionType == AdHocActionType.NewAccount || (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocSAPHANARequest.IsReallocate)))
                        {
                            if (adHocSAPHANARequest != null && !string.IsNullOrWhiteSpace(adHocSAPHANARequest.Position))
                            {
                                PositionLinkButton.Visible = true;
                            }
                            if (!string.IsNullOrWhiteSpace(adHocSAPHANARequest.PositionPrefix))
                            {
                                lblPrefix.Text = adHocSAPHANARequest.PositionPrefix;
                                PositionPrefix.Visible = true;
                            }
                        }
                        if (adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                        {
                            this.AccessNameLabel.Visible = false;
                            this.AccessNametr.Visible = string.IsNullOrWhiteSpace(this.AccessNameLabel.Text) ? false : true;
                        }
                        else
                        {
                            if (adHocSAPHANARequest != null)
                            {
                                //this.AccessNameLabel.Text = adHocEBSRequest.Entitlement;
                                this.AccessNameLabel.Text = string.IsNullOrEmpty(adHocSAPHANARequest.Position) ? "Roles : " + adHocSAPHANARequest.Roles : string.IsNullOrEmpty(adHocSAPHANARequest.Roles) ? "Position : " + adHocSAPHANARequest.Position : "Position : " + adHocSAPHANARequest.Position + "<br>" + "Roles    : " + adHocSAPHANARequest.Roles;
                                this.AccessNameLabel.Visible = string.IsNullOrWhiteSpace(adHocSAPHANARequest.Entitlement) ? false : true;
                                this.AccessNametr.Visible = AccessNameLabel.Visible;
                                //if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocEBSRequest.Entitlement == adHocEBSRequest.Position)
                                //{
                                //    if (adHocEBSRequest.IsReallocate)
                                //    {
                                //        this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Reallocate Position)";
                                //    }
                                //    else
                                //    {
                                //        this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Add Position)";
                                //    }
                                //}
                                if (!string.IsNullOrWhiteSpace(this.AccessNameLabel.Text))
                                {
                                    PositionLinkButton.Visible = true;
                                }
                                if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocSAPHANARequest.IsReallocate)
                                {
                                    this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Reallocate Position)";
                                }
                            }
                            else
                                this.AccessNameLabel.Visible = false;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                            //Label3.Visible = this.AccessNameLabel.Visible;
                            if (adHocSAPHANARequest != null && adHocSAPHANARequest.AdHocSAPHANASharedAccount != null)
                            {
                                pnlShareAccountDetails.Visible = true;
                                SAPUserTypeLabel.Text = Convert.ToString(adHocSAPHANARequest.AdHocSAPHANASharedAccount.SAPUserType);
                                lblAccountName.Text = adHocSAPHANARequest.AdHocSAPHANASharedAccount.AccountName;
                                lblDescription.Text = adHocSAPHANARequest.AdHocSAPHANASharedAccount.Description;
                                IList<object> listofrowsNeeded = new List<object>();
                                listofrowsNeeded.Add(SAPUserTypediv.ID);
                                listofrowsNeeded.Add(AccountNamediv.ID);
                                if (!string.IsNullOrWhiteSpace(lblDescription.Text))
                                {
                                    listofrowsNeeded.Add(Descriptiondiv.ID);
                                }
                                RowsVisibility(listofrowsNeeded);
                            }
                        }
                        if (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || adhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                        {
                            var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, new Dictionary<string, string> { { "endpointUserId", adHocSAPHANARequest.EndpointUserId.ToString() }, { "endpointSystemId", adhocRequest.EndpointSystemId.HasValue ? adhocRequest.EndpointSystemId.Value.ToString() : "0" } });
                            if (accountRecon != null)
                                lblLogonUserName.Text = accountRecon.AccountName; //new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocEBSRequest.EndpointUserId, "SAP");
                        }
                    }

                    LogonUserNametr.Visible = !string.IsNullOrWhiteSpace(lblLogonUserName.Text);
                }
                #endregion
                if (showAccessName)
                {
                    this.AccessNametr.Visible = string.IsNullOrWhiteSpace(this.AccessNameLabel.Text) ? false : true;
                }
            }

            //SetGroupRoleInfo(adhocRequest);
        }

        private bool ShowEntitlements(IEntitlementDetail adDetail)
        {
            this.AccessNametr.Visible = false;
            divEntitlements.Visible = false;
            if (adDetail != null)
            {
                if (!string.IsNullOrWhiteSpace(adDetail.Entitlement))
                {
                    var delimiter = new char[] { ',' };
                    var data = adDetail.Entitlement.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)?.ToList();
                    if (data != null && data.Count > 0)
                    {
                        ADEntitlements.Visible = true;
                        ADEntitlements.DataSource = data;
                        ADEntitlements.DataBind();
                        divEntitlements.Visible = true;
                    }
                }
            }
            return divEntitlements.Visible;
        }

        void RowsVisibility(IList<object> listoftr)
        {

            SAPUserTypediv.Visible = listoftr.Contains(SAPUserTypediv.ID) ? true : false;
            AccountNamediv.Visible = listoftr.Contains(AccountNamediv.ID) ? true : false;
            DisplayNamediv.Visible = listoftr.Contains(DisplayNamediv.ID) ? true : false;
            Descriptiondiv.Visible = listoftr.Contains(Descriptiondiv.ID) ? true : false;
            TRSMSAccessGrid.Visible = listoftr.Contains(TRSMSAccessGrid.ID) ? true : false;
            TRSRMAccessGrid.Visible = listoftr.Contains(TRSRMAccessGrid.ID) ? true : false;
            AliasNamediv.Visible = listoftr.Contains(AliasNamediv.ID) ? true : false;
            Oudiv.Visible = listoftr.Contains(Oudiv.ID) ? true : false;
            if (Session["RedirectFrom"] == "MyRequest")
                Oudiv.Visible = false;

        }
        /// <summary>
        /// Not using this for now. Dont know who return this. But corrected the code. Check for revision for more information.
        /// </summary>
        /// <param name="adhocRequest"></param>
        void SetGroupRoleInfo(PresentationMyRequestData adhocRequest)
        {
            if (adhocRequest == null)
                return;
            if (adhocRequest.EndPoint == null)
                return;
            if (adhocRequest.AdHocAccessRequests == null)
                return;
            if (string.IsNullOrWhiteSpace(adhocRequest.AdHocAccessRequests.Entitlement))
                return;

            if (adhocRequest.EndPoint.ApplicationType == ApplicationType.InhouseSystems && adhocRequest.EndPoint.CanAutomate && !adhocRequest.EndPoint.IsFunctionLevel)
            {
                var permissions = adhocRequest.AdHocAccessRequests.Entitlement;
                var param = new Dictionary<string, string>();
                param.Add("endPointSystemId", Convert.ToString(adhocRequest.EndPoint.Id));
                var inhPermissions = DomainApiClient<InhousePermission>.GetByAction<List<InhousePermission>>
                    (DomainApiAction.Endpoint.InhouseSystems.InhousePermission.GetBulkPermissionsByNames, param, new List<string> { permissions });
                var lstADGroups = new List<string>();
                var lstEPGroups = new List<string>();
                var lstEPRoles = new List<string>();
                var lstCEGroups = new List<string>();
                var lstCERoles = new List<string>();
                foreach (var permission in inhPermissions)
                {
                    if (!string.IsNullOrWhiteSpace(permission.ADGroups))
                    {
                        lstADGroups.Add(permission.ADGroups);
                    }
                    if (!string.IsNullOrWhiteSpace(permission.EPGroups))
                    {
                        lstEPGroups.Add(permission.EPGroups);
                    }
                    if (!string.IsNullOrWhiteSpace(permission.EPRoles))
                    {
                        lstEPRoles.Add(permission.EPRoles);
                    }
                    if (!string.IsNullOrWhiteSpace(permission.CEGroups))
                    {
                        lstCEGroups.Add(permission.CEGroups);
                    }
                    if (!string.IsNullOrWhiteSpace(permission.CERoles))
                    {
                        lstCERoles.Add(permission.CERoles);
                    }
                }

                var sb = new StringBuilder();
                var comma = ",";
                if (lstADGroups.Count > 0)
                {
                    sb.Append("AD Groups: ");
                    sb.Append(string.Join(comma, lstADGroups.Distinct()));
                }
                if (lstEPGroups.Count > 0)
                {
                    sb.Append("EP Groups: ");
                    sb.Append(string.Join(comma, lstEPGroups.Distinct()));
                }
                if (lstEPRoles.Count > 0)
                {
                    sb.Append("EP Roles: ");
                    sb.Append(string.Join(comma, lstEPRoles.Distinct()));
                }
                if (lstCEGroups.Count > 0)
                {
                    sb.Append("CE Groups: ");
                    sb.Append(string.Join(comma, lstCEGroups.Distinct()));
                }
                if (lstCERoles.Count > 0)
                {
                    sb.Append("CE Roles: ");
                    sb.Append(string.Join(comma, lstCERoles.Distinct()));
                }

                var groupRoleInfo = sb.ToString();
                if (!string.IsNullOrWhiteSpace(groupRoleInfo))
                {
                    divPartiallyAutomateInfo.Visible = true;
                    lblGroupRoleInfo.Text = groupRoleInfo;
                }
            }
        }


        Dictionary<int, string> SelectedRoles
        {
            get
            {
                if (ViewState["SelectedRoles"] != null)
                {
                    return (Dictionary<int, string>)ViewState["SelectedRoles"];
                }
                return new Dictionary<int, string>();
            }
            set
            {
                ViewState["SelectedRoles"] = value;
            }
        }

        public bool IsWintelGroup(long ActionuserId)
        {
            int GroupID = MasterConfigHelper.WintelGroupId;
            bool IsWintelGroup = DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.CheckUserGroup,
                new Dictionary<string, string> { { "userId", ActionuserId.ToString() }, { "GroupID", GroupID.ToString() } });
            return IsWintelGroup;
        }
        public void BindADGroupInfo(Dto.Task mytask)
        {
            if (mytask == null || mytask.AccessRequest == null || mytask.AccessRequest.AdHocAccessRequest == null) return;
            var task = mytask.AccessRequest;
            if (task.AdHocAccessRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal //|| task.AdHocAccessRequest.ApplicationType == ApplicationType.ADSDCLocal
                || task.AdHocAccessRequest.ApplicationType == ApplicationType.SAPLocal)
            {
                if (mytask.IsGroupTask && mytask.ActionGroupId.HasValue && mytask.ActionGroupId.Value == MasterConfigHelper.WintelGroupId)
                {
                    hdnADRequest.Value = task.Id.ToString();
                    trGroups.Visible = !(task.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount) && mytask.IsActionOnlyRequiredTask;
                    bool visible = (task.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || task.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount) && mytask.IsActionOnlyRequiredTask;
                    trDiscoveryGroups.Visible = visible;
                    trDiscoveryGroupsNotes.Visible = visible;
                    tdbtn.Visible = !(task.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove || task.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount);
                    tdgrid.Visible = !(task.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove || task.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount);
                    tdgroup.Visible = !(task.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove || task.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount);
                    tdexistinggroup.Visible = task.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove && IsAttestationCompaign == false;
                    if (task.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                        BindExistingGroups(mytask);
                }
            }
        }

        private void BindExistingGroups(Dto.Task mytask)
        {
            if (mytask == null || mytask.AccessRequest == null || mytask.AccessRequest.AdHocAccessRequest == null) return;
            var task = mytask.AccessRequest;
            //var task = taskFacade.GetProvisioningTask(this.TaskId, true);
            AdHocADRequest ADRequest = null;
            AdHocSAPLocalRequest ADSAPLocalRequest = null;
            if (task.AdHocAccessRequest.ApplicationType != ApplicationType.SAPLocal)
            {
                ADRequest = (AdHocADRequest)task.AdHocAccessRequest.GetEntitlementDetail();
            }
            else
            {
                ADSAPLocalRequest = (AdHocSAPLocalRequest)task.AdHocAccessRequest.GetEntitlementDetail();
            }
            long endpointUserId = 0;
            if (ADRequest != null)
            {
                endpointUserId = ADRequest.EndpointUserId;
                
            }
            else
            {
                endpointUserId = ADSAPLocalRequest.EndpointUserId;
            }
            if (endpointUserId > 0)
            {
                if (IsAttestationCompaign)
                {

                    var entitlement = ADRequest != null ? ADRequest.Entitlement : ADSAPLocalRequest.Entitlement;
                    var item = new RadComboBoxItem(entitlement, entitlement);
                    item.Checked = true;
                    this.ExistingGroupComboBox.Items.Add(item);
                    this.ExistingGroupComboBox.DataBind();
                }
                else
                {
                    string endpointid = "0";
                    if (task.AdHocAccessRequest != null && task.AdHocAccessRequest.EndpointSystemId.HasValue)
                        endpointid = task.AdHocAccessRequest.EndpointSystemId.Value.ToString();
                    var groups = DomainApiClient<Dto.ADSGroupInfo>.GetByAction<List<Dto.ADSGroupInfo>>(DomainApiAction.Endpoint.ActiveDirectory.ADSGroupInfo.GetADGroupsByUserIdAndEndpointId, new Dictionary<string, string> { { "userId", endpointUserId.ToString() }, { "EndpointId", endpointid } });

                    this.ExistingGroupComboBox.DataSource = groups.Select(x => new { Key = x.Id, Value = x.name })
                                .Distinct().ToDictionary(o => o.Key, o => o.Value);

                    this.ExistingGroupComboBox.DataBind();
                }
            }
            else
            {
                this.ExistingGroupComboBox.Items.Clear();
            }
        }
        protected void btnDiscoveryAD_Click(object sender, EventArgs e)
        {

            try
            {
                PresentationMyRequestData adhocRequest = DomainApiClient<PresentationMyRequestData>.GetByAction<PresentationMyRequestData>(DomainApiAction.PresentationMyRequestData.GetByAdhocId, new Dictionary<string, string> { { "requestId", this.AdHocRequestId.ToString() }, { "deepLoad", "false" } });

                string strJobs = string.Empty;

                if (adhocRequest.ApplicationType != ApplicationType.SAPLocal)
                {
                    strJobs = MasterConfigHelper.ADDiscoveryJobForWintelTeam;
                }
                else
                {
                    strJobs = MasterConfigHelper.SAPLOCALDiscoveryJobForWintelTeam;
                }

                List<string> jobs = new List<string>();

                if (string.IsNullOrWhiteSpace(strJobs))
                    return;
                else
                    jobs = strJobs.Split(',').ToList();

                foreach (string strJob in jobs)
                {
                    ScheduleForRunNow(Convert.ToInt32(strJob));
                }

            }
            catch (Exception)
            {

            }
        }

        private void ScheduleForRunNow(long ProcessId)
        {

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("discoveryProcessId", Convert.ToString(ProcessId));

            Dto.Scheduler schedluer = DomainApiClient<Dto.Scheduler>.GetByAction<Dto.Scheduler>
                (DomainApiAction.Schedulers.Scheduler.GetDiscoveryScheduler, parameter);


            schedluer.Occurence = Occurence.RunNow;
            schedluer.ReccurEveryDays = null;
            schedluer.ReccurEveryMonths = null;
            schedluer.ReccurEveryWeeks = null;
            schedluer.StartDate = DateTime.Now.Date;
            schedluer.StartTime = new TimeSpan(DateTime.Now.Hour, (DateTime.Now.Minute + 1), DateTime.Now.Second);
            schedluer.WeekDay = string.Empty;

            Dictionary<string, string> parameterprocess = new Dictionary<string, string>();
            parameterprocess.Add("ProcessId", Convert.ToString(ProcessId));
            parameterprocess.Add("processType", ProcessType.Discovery.ToString());
            Dto.Process process = DomainApiClient<Dto.Process>.GetByAction<Dto.Process>
                (DomainApiAction.Schedulers.Process.GetByProcessType, parameterprocess);

            if (process != null)
            {
                process.Scheduler = schedluer;
                process.ErrorMessage = "";
                process.ProcessStartAt = null;
                process.ProcessEndAt = null;
                process.Status = "Pending";
                DomainApiClient<Dto.Process>.Save(process);
            }
        }
        protected void btnRefreshStatus_Click(object sender, EventArgs e)
        {
            try
            {
                PresentationMyRequestData adhocRequest = DomainApiClient<PresentationMyRequestData>.GetByAction<PresentationMyRequestData>(DomainApiAction.PresentationMyRequestData.GetByAdhocId, new Dictionary<string, string> { { "requestId", this.AdHocRequestId.ToString() }, { "deepLoad", "false" } });
                string strJobs = string.Empty;

                if (adhocRequest.ApplicationType != ApplicationType.SAPLocal)
                {
                    strJobs = MasterConfigHelper.ADDiscoveryJobForWintelTeam;
                }
                else
                {
                    strJobs = MasterConfigHelper.SAPLOCALDiscoveryJobForWintelTeam;
                }

                List<string> jobs = new List<string>();

                if (string.IsNullOrWhiteSpace(strJobs))
                    return;
                else
                    jobs = strJobs.Split(',').ToList();

                foreach (string strJob in jobs)
                {
                    Dto.DiscoveryProcess process = DomainApiClient<Dto.DiscoveryProcess>.GetByAction<Dto.DiscoveryProcess>(DomainApiAction.Schedulers.Process.GetDiscoveryProcessByProcessId, new Dictionary<string, string> { { "processId", strJob } });

                    if (process == null)
                        return;

                    lblStatus.Text = process.process.Status;

                    if (lblStatus.Text == "InProgress")
                    {
                        lblStatus.CssClass = "description label label-warning";
                        // imgStatus.ImageUrl = "~/Images/progblue.gif";
                    }
                    if (lblStatus.Text == "Pending")
                    {
                        lblStatus.CssClass = "description label label-warning";
                        //// imgStatus.ImageUrl = "~/Images/warning32.png";
                    }
                    if (lblStatus.Text == "Completed")
                    {
                        lblStatus.CssClass = "description label label-success";
                        //imgStatus.ImageUrl = "~/Images/ok32.png";
                    }
                    if (lblStatus.Text == "Failed")
                    {
                        lblStatus.CssClass = "description label label-danger";
                        //imgStatus.ImageUrl = "~/Images/error32.png";
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        protected void btnAdd_Click1(object sender, EventArgs e)
        {
            if (this.RolesAutoCompleteBox.Entries.Count > 0)
            {
                foreach (AutoCompleteBoxEntry item in this.RolesAutoCompleteBox.Entries)
                {
                    int roleId = Convert.ToInt32(item.Value);
                    string role = item.Text;
                    if (this.ApplicationType == ApplicationType.ADSingaporePowerLocal && (this.AdHocActionType == AdHocActionType.AmendmentAdd
                            || this.AdHocActionType == AdHocActionType.NewAccount
                            ))
                    {
                        var param = new Dictionary<string, string>();
                        param.Add("selectedADGroup", role);
                        var message = DomainApiClient<ADSGroupInfo>.GetByAction<string>("CheckADGroupExistsInOtherSystemMatrix", param);
                        if (!string.IsNullOrWhiteSpace(message))
                        {
                            MasterPage.ShowAlert(AlertType.Error, $"{MasterConfigHelper.ADOtherSystemGroupValidationMessage} {message}");
                            return;
                        }
                    }
                    var roles = this.SelectedRoles;
                    if (!roles.ContainsKey(roleId))
                    {
                        roles.Add(roleId, role);
                    }
                    
                    this.SelectedRoles = roles;
                }
                this.RebindRolesGrid();
                this.RolesAutoCompleteBox.Entries.Clear();
            }
            else
            {
                ShowAlertMessage(AlertType.Error, "Please select at least one group");
            }
        }
        public void ConflictBusinessRoles(long dIUserId, long requestId, bool isBulkRequest)
        {
            List<string> AllRequest = DomainApiClient<Dto.SAPUserinfo>.GetByAction<List<string>>(DomainApiAction.Endpoint.SAP.SAPUserinfo.GetConflictRemarks,
                          new Dictionary<string, string> { { "dIUserId", dIUserId.ToString() }, { "requestId", requestId.ToString() }, { "isBulkRequest", "false" } });
            ConflictRemark = AllRequest;
        }
        public void SAPSODConflictBusinessRoles(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            List<PresentationConflictRemarks> AllRequest = DomainApiClient<Dto.SAPUserinfo>.GetByAction<List<PresentationConflictRemarks>>(DomainApiAction.Endpoint.SAP.SAPUserinfo.GetSAPSODConflictsShowingrid,
                          new Dictionary<string, string> { { "dIUserId", dIUserId.ToString() }, { "requestId", requestId.ToString() }, { "isBulkRequest", "false" },{ "applicationType", applicationType } });
            if(AllRequest != null && AllRequest.Count > 0)
            {
                MSSLEBSConflictRemark = AllRequest;
                IsEBSMSSLConflictRemark = true;
            }
            else
            {
                MSSLEBSConflictRemark = new List<PresentationConflictRemarks>();
                IsEBSMSSLConflictRemark = false;
            }
        }
        public void SAPHANASODConflictBusinessRoles(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            List<PresentationConflictRemarks> AllRequest = DomainApiClient<Dto.SAPHANAUserinfo>.GetByAction<List<PresentationConflictRemarks>>(DomainApiAction.Endpoint.SAPHANA.SAPHANAUserinfo.GetSAPSODConflictsShowingrid,
                          new Dictionary<string, string> { { "dIUserId", dIUserId.ToString() }, { "requestId", requestId.ToString() }, { "isBulkRequest", "false" }, { "applicationType", applicationType } });
            if (AllRequest != null && AllRequest.Count > 0)
            {
                MSSLEBSConflictRemark = AllRequest;
                IsEBSMSSLConflictRemark = true;
            }
            else
            {
                MSSLEBSConflictRemark = new List<PresentationConflictRemarks>();
                IsEBSMSSLConflictRemark = false;
            }
        }
        private void RebindRolesGrid()
        {
            //if (this.SelectedRoles.Count > 0)
            //    trGrdid.Visible = true;
            //else
            //    trGrdid.Visible = false;

            this.RoleGrid.DataSource = this.SelectedRoles;
            this.RoleGrid.Rebind();
        }

        protected void DeleteRoleButton_Click(object sender, EventArgs e)
        {
            int roleId = Convert.ToInt32(((LinkButton)sender).CommandArgument);
            var roles = this.SelectedRoles;
            roles.Remove(roleId);
            this.SelectedRoles = roles;

            this.RebindRolesGrid();
        }

        public void UpdateADEntitlement(Dto.Task mytask, out string ValidationMessage)
        {
            string role = "";
            ValidationMessage = "";
            // Dto.Task mytask = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.GetProvisioningTask, new Dictionary<string, string> { { "id", TaskId.ToString() } });
            if (mytask == null || mytask.AccessRequest == null) return;
            var task = mytask.AccessRequest;
            if ((task.AdHocAccessRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal // || task.AdHocAccessRequest.ApplicationType == ApplicationType.ADSDCLocal
                || task.AdHocAccessRequest.ApplicationType == ApplicationType.SAPLocal))
            {
                int GroupID = MasterConfigHelper.WintelGroupId;

                bool IsWintelGroup = mytask.ActionGroupId.HasValue && mytask.ActionGroupId.Value == GroupID;// DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.CheckUserGroup, new Dictionary<string, string> { { "userId", mytask.ActionUserId.HasValue ? mytask.ActionUserId.Value.ToString() : "0" }, { "GroupID", GroupID.ToString() } });
                if (IsWintelGroup)
                {
                    if (task.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        var selectedRoles = this.ExistingGroupComboBox.CheckedItems.Select(item => item.Text).ToList();
                        if (selectedRoles.Count == 0)
                        {
                            ValidationMessage = "Please remove at least one Group Name";
                        }
                        else
                        {
                            foreach (string Role in selectedRoles)
                            {
                                if (role == "")
                                    role = role + Role;
                                else
                                    role = role + "," + Role;
                            }
                        }
                    }
                    else
                    {
                        if (SelectedRoles.Count == 0 && (task.AdHocAccessRequest.AdhocActionType != AdHocActionType.NewAccount && task.AdHocAccessRequest.AdhocActionType != AdHocActionType.DeleteAccount))
                        {
                            ValidationMessage = "Please add at least one Group Name";
                        }
                        else if (SelectedRoles.Count == 0 && (task.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount && (task.AdHocAccessRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal && task.AdHocAccessRequest.AccountType == AccountType.User)))
                        {
                            ValidationMessage = "Please add at least one Group Name";
                        }
                        else
                        {
                            foreach (KeyValuePair<int, string> Role in this.SelectedRoles)
                            {
                                if (role == "")
                                    role = role + Role.Value;
                                else
                                    role = role + "," + Role.Value;
                            }
                        }
                        
                    }
                    if (!string.IsNullOrWhiteSpace(role))
                    {
                        var adhocAcessRequest = task.AdHocAccessRequest;
                        if (adhocAcessRequest.ApplicationType != ApplicationType.SAPLocal)
                        {
                            AdHocADRequest ADRequest = (AdHocADRequest)task.AdHocAccessRequest.GetEntitlementDetail();
                            if (ADRequest != null)
                            {
                                ADRequest.Entitlement = role;
                                adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(ADRequest);
                                DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                            }
                        }
                        else
                        {
                            AdHocSAPLocalRequest ADSapLocalRequest = (AdHocSAPLocalRequest)task.AdHocAccessRequest.GetEntitlementDetail();
                            if (ADSapLocalRequest != null)
                            {
                                ADSapLocalRequest.Entitlement = role;
                                adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(ADSapLocalRequest);
                                DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocAcessRequest);
                            }
                        }
                    }

                }
            }
        }

        protected void RolesAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {

        }
        protected void HRMSGRID_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                Label VarianceLabel = (Label)item.FindControl("VarianceLabel");
                string CostCenter = Convert.ToString(item.GetDataKeyValue("CostCenter"));
                string PurchaseOrg = Convert.ToString(item.GetDataKeyValue("PurchaseOrg"));
                string PurchaseGroup = Convert.ToString(item.GetDataKeyValue("PurchaseGroup"));
                string Plant = Convert.ToString(item.GetDataKeyValue("Plant"));
                string Company = Convert.ToString(item.GetDataKeyValue(""));
                string StorageLocation = Convert.ToString(item.GetDataKeyValue("StorageLocation"));
                string MRPController = Convert.ToString(item.GetDataKeyValue("MRPController"));
                if (VarianceLabel != null)
                {
                    if (!string.IsNullOrWhiteSpace(CostCenter))
                        VarianceLabel.Text = "Cost Center : " + CostCenter;
                    else if (!string.IsNullOrWhiteSpace(MRPController))
                        VarianceLabel.Text = "MRP Controller : " + MRPController;
                    else if (!string.IsNullOrWhiteSpace(Plant))
                        VarianceLabel.Text = "Plant : " + Plant;
                    else if (!string.IsNullOrWhiteSpace(PurchaseGroup))
                        VarianceLabel.Text = "Purchase Group : " + PurchaseGroup;
                    else if (!string.IsNullOrWhiteSpace(PurchaseOrg))
                        VarianceLabel.Text = "Purchase Org : " + PurchaseOrg;
                    else if (!string.IsNullOrWhiteSpace(StorageLocation))
                        VarianceLabel.Text = "Storage Location : " + StorageLocation;
                }
            }
        }

        protected void SMSObjGrid_DataBound(object sender, EventArgs e)
        {
            RadGrid grid = sender as RadGrid;
            int gridItems = grid.MasterTableView.Items.Count;
            if (gridItems <= 3)
            {
                grid.ClientSettings.Scrolling.AllowScroll = false;
            }
            else
            {
                grid.ClientSettings.Scrolling.ScrollHeight = System.Web.UI.WebControls.Unit.Pixel(150);
                grid.ClientSettings.Scrolling.AllowScroll = true;
            }
        }
        protected void SRMObjGrid_DataBound(object sender, EventArgs e)
        {
            RadGrid grid = sender as RadGrid;
            int gridItems = grid.MasterTableView.Items.Count;
            if (gridItems <= 3)
            {
                grid.ClientSettings.Scrolling.AllowScroll = false;
            }
            else
            {
                grid.ClientSettings.Scrolling.ScrollHeight = System.Web.UI.WebControls.Unit.Pixel(150);
                grid.ClientSettings.Scrolling.AllowScroll = true;
            }

        }

        protected void SRMObjGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem)
            {
                var dataItem = e.Item as GridEditableItem;
                if (dataItem != null)
                {
                    ImageButton editbutton = (ImageButton)dataItem["Details"].Controls[0];
                    var ctl = ((GridEditableItem)e.Item).GetDataKeyValue("RetainMainRole");
                    if (ctl != null && editbutton != null)
                    {
                        var RetainMainRole = Convert.ToBoolean(ctl);
                        editbutton.Visible = !RetainMainRole;
                    }
                }
            }
        }

        protected void SRMObjGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Details")
            {
                var adHocRequest = DomainApiClient<Dto.AdHocAccessRequest>.GetById(this.AdHocRequestId, true);
                if (adHocRequest != null && adHocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    var adhocDetails = adHocRequest.GetEntitlementDetail() as AdHocSAPSRMRequest;
                    var Entitlement = (string)((GridDataItem)e.Item).GetDataKeyValue("Entitlement");
                    var userCurrentRoles = DomainApiClient<Dto.SAPSRMBusinessRoleMapping>.GetByAction<List<Dto.PresentationUserBusinessRole>>(DomainApiAction.Endpoint.SRM.SAPSRMBusinessRoleMappings.GetSAPSRMUserRoles,
                                                    new Dictionary<string, string> {   {"adhocActionType",Convert.ToString((int)adHocRequest.AdhocActionType) },
                                                        { "accType" ,Convert.ToString((int)adHocRequest.AccountType)},
                                                        { "Userid", adHocRequest.RequesteeUserId.ToString() },
                                                        { "UserName", adhocDetails.AccountName},
                                                        { "businessRole", Entitlement},{ "EndpointId", adHocRequest.EndpointSystemId.ToString()}
                                                        });
                    var selectedBRole = userCurrentRoles.Where(i => i.Value.ToUpper() == Entitlement.ToUpper()).FirstOrDefault();
                    StringBuilder content = new StringBuilder();
                    if (selectedBRole != null && selectedBRole != null)
                    {
                        content.AppendLine($"<b>Business Role:</b> {selectedBRole.RoleName} <br/>");
                        foreach (var POrg in selectedBRole.POrgs)
                        {
                            if (POrg != null)
                            {
                                content.AppendLine($"<br/> <b>Purchase Org:</b> {POrg.Value} ");
                                content.AppendLine($"<br/> <b>Purchase Group(s):</b>");
                                if (POrg != null)
                                {
                                    content.Append($" {string.Join(",", POrg?.PGroups?.Select(i => i.PGrpName))} ");
                                }
                                content.Append($"<br/>");
                            }
                        }
                    }
                    lblInformation.Text = content.ToString();
                    ShowWindow();
                }
            }
        }
        void ShowWindow()
        {
            var key = "showSRMWindow";
            string script = "Sys.Application.remove_load(showSRMWindow);Sys.Application.add_load(showSRMWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindow()
        {
            var key = "closeSRMWindow";
            string script = "Sys.Application.remove_load(closeSRMWindow);Sys.Application.add_load(closeSRMWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        protected void btnDetailOk_Click(object sender, EventArgs e)
        {
            CloseWindow();
        }

        protected void AccessNameLabel_Click(object sender, EventArgs e)
        {
            if (ConvertLinkButtonToLabel)
                return;
            //var param = new Dictionary<string, string>();
            //var entitlememt = AccessNameLabel.Text;
            //if (AccessRequest != null && AccessRequest.AdHocAccessRequests != null)
            //{
            //    if (AccessRequest.ApplicationType == ApplicationType.MSSL_CRM || AccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
            //    {
            //        var data = AccessRequest.AdHocAccessRequests.GetEntitlementDetail();
            //        AdHocMSSLRequest adHocMSSLRequest = (AdHocMSSLRequest)data;
            //        entitlememt = adHocMSSLRequest.Entitlement;
            //    }
            //    if (AccessRequest.ApplicationType == ApplicationType.EBS_CRM || AccessRequest.ApplicationType == ApplicationType.EBS_ISU)
            //    {
            //        var data = AccessRequest.AdHocAccessRequests.GetEntitlementDetail();
            //        AdHocEBSRequest adHocEBSRequest = (AdHocEBSRequest)data;
            //        entitlememt = adHocEBSRequest.Entitlement;
            //    }
            //}

            //param.Add("endpointSystemId", EndpointSystemId.ToString());
            //param.Add("entitlement", entitlememt);
            //radWindowEntitlementDetail.Width = GetUnit(70);
            //List<KeyValuePair<string, string>> entitlementDetail = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<List<KeyValuePair<string, string>>>("GetEntitlementDetailByEntitlement", param);
            //grdEntitlements.DataSource = entitlementDetail;
            //grdEntitlements.DataBind();
            //radWindowEntitlementDetail.VisibleOnPageLoad = true;
        }

        //protected void btnCancel_Click(object sender, EventArgs e)
        //{
        //    radWindowEntitlementDetail.VisibleOnPageLoad = false;
        //}

        protected void HRMSGRID_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ViewTechicalRole")
            {
                GridDataItem item = e.Item as GridDataItem;
                string role = Convert.ToString(item.GetDataKeyValue("Entitlement"));
                string company = Convert.ToString(item.GetDataKeyValue("Company"));
                if (string.IsNullOrWhiteSpace(company)) company = null;

                AdHocSAPHRFLINSRequest entiltement = new AdHocSAPHRFLINSRequest();
                entiltement.AdHocSAPHRFLINSRequestDetails.AddRange(FlinsEntitlementList);
                var detail = entiltement.AdHocSAPHRFLINSRequestDetails.FirstOrDefault(lq => lq.Entitlement == role && lq.Company == company);
                List<string> list = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetHrFlinsTechnicalRolesByAdhocRequestObject, null, detail);
                ViewTechnicalSource.DataSource = list;
                ViewTechnicalSource.DataBind();
                ViewTechnicalSourceWindow.VisibleOnPageLoad = true;
            }
        }

        protected void btnCancelViewDetails_Click(object sender, EventArgs e)
        {
            ViewTechnicalSourceWindow.VisibleOnPageLoad = false;
        }

        protected void PositionLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (AccessRequest != null && AccessRequest.AdHocAccessRequests != null)
                {
                    
                    
                    var positionTecRoles = new List<string>();
                    if (AccessRequest.ApplicationType == ApplicationType.MSSL_CRM || AccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                    {
                        var data = AccessRequest.AdHocAccessRequests.GetEntitlementDetail();
                        AdHocMSSLRequest adHocMSSLRequest = (AdHocMSSLRequest)data;
                        var para = new Dictionary<string, string>();
                        para.Add("position", adHocMSSLRequest.Position);
                        var positionMatrix = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<Dto.SAPXLMSSLPositionsMatrix>(DomainApiAction.Matrix.SAP_MSSL.GetMSSLPositionMatrixByPosition, para);

                        if (positionMatrix != null)
                        {
                            List<string> result = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAP_MSSL.GetRolesByPosition, new Dictionary<string, string> { { "position", AccessRequest.ApplicationType == ApplicationType.MSSL_ISU ? positionMatrix.Position_MSP : positionMatrix.Position_MCP }, { "endpointSystemId", Convert.ToInt32(AccessRequest.EndpointSystemId).ToString() } });
                            positionTecRoles.AddRange(result);
                           
                        }
                    }
                    else if (AccessRequest.ApplicationType == ApplicationType.EBS_CRM || AccessRequest.ApplicationType == ApplicationType.EBS_ISU)
                    {
                        var data = AccessRequest.AdHocAccessRequests.GetEntitlementDetail();
                        AdHocEBSRequest adHocEBSRequest = (AdHocEBSRequest)data;

                        var para = new Dictionary<string, string>();
                        para.Add("position", adHocEBSRequest.Position);
                        var positionMatrix = DomainApiClient<Dto.SAPEBSXLPositionsMatrix>.GetByAction<Dto.SAPEBSXLPositionsMatrix>(DomainApiAction.Matrix.SAP_EBS.GetEBSPositionMatrixByPosition, para);
                        if (positionMatrix != null)
                        {
                            List<string> result = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAP_MSSL.GetRolesByPosition, new Dictionary<string, string> { { "position", AccessRequest.ApplicationType == ApplicationType.EBS_ISU ? positionMatrix.Position_ISP : positionMatrix.Position_CRP }, { "endpointSystemId", Convert.ToInt32(AccessRequest.EndpointSystemId).ToString() } });
                            positionTecRoles.AddRange(result);
                        }
                    }
                    else if (AccessRequest.ApplicationType == ApplicationType.EBS || AccessRequest.ApplicationType == ApplicationType.MSSL)
                    {
                        var data = AccessRequest.AdHocAccessRequests.GetEntitlementDetail();
                        var adHocEBSRequest = (AdHocSAPHANARequest)data;

                        var para = new Dictionary<string, string>();
                        para.Add("position", adHocEBSRequest.Position);
                        para.Add("endpointSystemId", AccessRequest.EndpointSystemId.Value.ToString());
                        var positionMatrix = DomainApiClient<Dto.SAPHANAXLPositionsMatrix>.GetByAction<Dto.SAPHANAXLPositionsMatrix>(DomainApiAction.Matrix.SAPHANA.GetSAPHANAPositionMatrixByPosition, para);
                        if (positionMatrix != null)
                        {
                            List<string> result = DomainApiClient<Dto.SAPHANAXLPositionsMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAPHANA.GetSAPHANARolesByPosition, new Dictionary<string, string> { { "position", positionMatrix.Position }, { "endpointSystemId", Convert.ToInt32(AccessRequest.EndpointSystemId).ToString() } });
                            positionTecRoles.AddRange(result);
                        }
                    }
                    if (positionTecRoles != null && positionTecRoles.Count > 0)
                        panelViewTechnicalSource.Visible = true;
                    else
                        panelViewTechnicalSource.Visible = false;

                    ViewTechnicalSource.DataSource = positionTecRoles;
                    ViewTechnicalSource.DataBind();

                    if (!ConvertLinkButtonToLabel)
                    {

                        //For Role Access
                        var param = new Dictionary<string, string>();
                        var entitlememt = AccessNameLabel.Text;
                        if (AccessRequest.ApplicationType == ApplicationType.MSSL_CRM || AccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                        {
                            var data = AccessRequest.AdHocAccessRequests.GetEntitlementDetail();
                            AdHocMSSLRequest adHocMSSLRequest = (AdHocMSSLRequest)data;
                            entitlememt = adHocMSSLRequest.Entitlement;
                        }
                        else if (AccessRequest.ApplicationType == ApplicationType.EBS_CRM || AccessRequest.ApplicationType == ApplicationType.EBS_ISU)
                        {
                            var data = AccessRequest.AdHocAccessRequests.GetEntitlementDetail();
                            AdHocEBSRequest adHocEBSRequest = (AdHocEBSRequest)data;
                            entitlememt = adHocEBSRequest.Entitlement;
                        }
                        else if (AccessRequest.ApplicationType == ApplicationType.EBS || AccessRequest.ApplicationType == ApplicationType.MSSL)
                        {
                            var data = AccessRequest.AdHocAccessRequests.GetEntitlementDetail();
                            var adHocSAPHANARequest = (AdHocSAPHANARequest)data;
                            entitlememt = adHocSAPHANARequest.Entitlement;
                        }

                        if (!string.IsNullOrWhiteSpace(entitlememt))
                        {
                            param.Add("endpointSystemId", EndpointSystemId.ToString());
                            param.Add("entitlement", entitlememt);
                            List<KeyValuePair<string, string>> entitlementDetail = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<List<KeyValuePair<string, string>>>("GetEntitlementDetailByEntitlement", param);
                            if (entitlementDetail != null && entitlementDetail.Count > 0)
                                pnlEntitlementRole.Visible = true;
                            else
                                pnlEntitlementRole.Visible = false;

                            grdEntitlements.DataSource = entitlementDetail;
                            grdEntitlements.DataBind();

                        }
                    }
                    ViewTechnicalSourceWindow.VisibleOnPageLoad = true;
                }
            }
            catch (Exception ex)
            {
            }
        }
        protected void ResponseGroupGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                var groupName = (((GridDataItem)e.Item).GetDataKeyValue("Name")).ToString();
                lblGrpName.Text = groupName;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", groupName);
                var data = DomainApiClient<Dto.SkypeForBusinessResponseGroupInfo>.GetByAction<List<Dto.Presentation.PresentationResponseGroupUserMember>>("GetResponseGroupUser", parameter);
                ViewRoleGroupUserSource.DataSource = data;
                ViewRoleGroupUserSource.DataBind();
                ViewRoleGroupUserWindow.VisibleOnPageLoad = true;

            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ViewRoleGroupUserWindow.VisibleOnPageLoad = false;
        }
    }
}