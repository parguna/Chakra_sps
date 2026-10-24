using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using Telerik.Web.UI;
using Application;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Web.Request.BulkRequest.UC
{

    public partial class BulkGenericRequestDetails : UIBaseUC
    {

        public long BulkRequestID { get; set; }

        public long GroupID { get; set; }
        public ApplicationType EntitlementDetailType { get; set; }
        public bool IsActionOnlyRequiredTask { get; set; }
        public AdHocActionType AdHocActionType { get; set; }

        private const string V_REQUEST = "V_REQUEST";
        private const string V_REQUESTDETAIL = "V_REQUESTDETAIL";
        private const string V_CONFLICTS = "V_CONFLICTS";
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
        public Dto.BulkRequest BulkRequest
        {
            get
            {
                if (ViewState["BulkRequestDetail"] != null)
                {
                    return (Dto.BulkRequest)ViewState["BulkRequestDetail"];
                }
                return null;
            }
            set
            {
                ViewState["BulkRequestDetail"] = value;
            }
        }

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
        [Serializable]
        public class BulkRequestUserDetails
        {
            public User User { get; set; }
            public string AccountName { get; set; }
            public string HandPhone { get; set; }
        }
        private List<User> RequestedUser
        {
            get
            {
                if (ViewState[V_REQUEST] != null)
                {
                    return (List<User>)ViewState[V_REQUEST];
                }
                return new List<User>();
            }
            set
            {
                ViewState[V_REQUEST] = value;
            }
        }

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
        public ApplicationType AppType
        {
            set
            {
                ViewState["ApplicationType"] = value;
            }
            get
            {
                ApplicationType applicationType;
                if (Enum.TryParse<ApplicationType>(ViewState["ApplicationType"].ToString(), out applicationType))
                {
                    return applicationType;
                }
                return ApplicationType.None;
            }
        }
        public AdHocActionType IdmsAdhocActionType
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
        private List<BulkRequestUserDetails> RequestedUserDetails
        {
            get
            {
                if (ViewState[V_REQUESTDETAIL] != null)
                {
                    return (List<BulkRequestUserDetails>)ViewState[V_REQUESTDETAIL];
                }
                return new List<BulkRequestUserDetails>();
            }
            set
            {
                ViewState[V_REQUESTDETAIL] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeForm();
                LabelVisibles();
            }
        }

        private void InitializeForm()
        {
            this.PopulateRequestDetails();
        }

        // Conflict to be written
        public void ConflictBusinessRoles(long dIUserId, string accountName, string UserName, long requestId, bool isBulkRequest)
        {
            List<string> AllRequest = DomainApiClient<Dto.SAPUserinfo>.GetByAction<List<string>>(DomainApiAction.Endpoint.SAP.SAPUserinfo.GetConflictRemarks,
                          new Dictionary<string, string> { { "dIUserId", dIUserId.ToString() }, { "requestId", requestId.ToString() }, { "isBulkRequest", "true" } });

            if (AllRequest != null && AllRequest.Count > 0)
                ConflictRemark = new List<string>();

            foreach (var str in AllRequest)
                ConflictRemark.Add(str + $" for the User - {UserName}({accountName}) ");
        }
        public void SAPSODConflictBusinessRoles(long dIUserId, string accountName, string UserName, long requestId, bool isBulkRequest, string applicationType)
        {
            //List<string> AllRequest = DomainApiClient<Dto.SAPUserinfo>.GetByAction<List<string>>(DomainApiAction.Endpoint.SAP.SAPUserinfo.GetSAPSODConflicts,
            //              new Dictionary<string, string> { { "dIUserId", dIUserId.ToString() }, { "requestId", requestId.ToString() }, { "isBulkRequest", "true" }, { "applicationType", applicationType } });

            //if (AllRequest != null && AllRequest.Count > 0)
            //{
            //    foreach (var str in AllRequest)
            //        ConflictRemark.Add(str + $" for the User - {UserName}({accountName}) ");
            //}
            List<PresentationConflictRemarks> AllRequest = DomainApiClient<Dto.SAPUserinfo>.GetByAction<List<PresentationConflictRemarks>>(DomainApiAction.Endpoint.SAP.SAPUserinfo.GetSAPSODConflictsShowingrid,
                          new Dictionary<string, string> { { "dIUserId", dIUserId.ToString() }, { "requestId", requestId.ToString() }, { "isBulkRequest", "true" }, { "applicationType", applicationType } });
            if (AllRequest != null && AllRequest.Count > 0)
            {
                foreach (var item in AllRequest)
                {
                    item.UserWithAccountName = $" for the User - {UserName}({accountName})";
                    MSSLEBSConflictRemark.Add(item);
                }
                IsEBSMSSLConflictRemark = true;
            }
            
        }

        ApplicationType[] applicationTypes = new ApplicationType[] { ApplicationType.EBS,ApplicationType.MSSL, ApplicationType.MSSL, ApplicationType.MSSL_ISU, ApplicationType.MSSL_CRM, ApplicationType.EBS_ISU, ApplicationType.EBS_CRM, ApplicationType.SAP_GRC, ApplicationType.FLINS_BI };
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

        User GetUser(long Id)
        {
            return DomainApiClient<Dto.User>.GetById(Id);
        }
        //protected void AccessNameLabel_Click(object sender, EventArgs e)
        //{
        //    if (ConvertLinkButtonToLabel)
        //        return;
           
        //    radWindowEntitlementDetail.VisibleOnPageLoad = true;
        //}
        private void PopulateRequestDetails()
        {
            var request = DomainApiClient<Dto.BulkRequest>.GetById(BulkRequestID, true);

            if (request == null) return;
            BulkRequest = request;
            this.AppType = request.ApplicationType;
            this.IdmsAdhocActionType = request.AdhocActionType;
            EndpointSystemId = Convert.ToInt64(request.EndpointSystemId);
            ConvertLinkButtonToLabel = !applicationTypes.Contains(request.ApplicationType);

            if (ConvertLinkButtonToLabel)
            {
                AccessNameLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#666");
                AccessNameLabel.Style.Add("text-decoration", "none");
                AccessNameLabel.Style.Add("cursor", "text");
            }
            if (request.ApplicationType == ApplicationType.HRMS_FLINS)
            {
                foreach (var detail in request.BulkRequestDetails)
                    ConflictBusinessRoles(detail.UserId, detail.UserName, detail.Name, request.Id, true);
            }
            if ((request.ApplicationType == ApplicationType.MSSL_CRM || request.ApplicationType == ApplicationType.MSSL_ISU || request.ApplicationType == ApplicationType.EBS_CRM || request.ApplicationType == ApplicationType.EBS_ISU) && (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.NewAccount))
            {
                string applicationType = "EBS";
                if (request.ApplicationType == ApplicationType.MSSL_CRM || request.ApplicationType == ApplicationType.MSSL_ISU)
                {
                    applicationType = "MSSL";
                }
                divEBSMSSLConflictsRemarks.Visible = false;
                if ((request.ProgressStatus == AccessRequestProgress.Completed || request.ProgressStatus == AccessRequestProgress.ManuallyResolved || request.ProgressStatus == AccessRequestProgress.Withdrawn
                   || request.ProgressStatus == AccessRequestProgress.RequestRejected || request.ProgressStatus == AccessRequestProgress.ReadytoExecute || request.ProgressStatus == AccessRequestProgress.ExecutionFailed
                    || request.ProgressStatus == AccessRequestProgress.Executing
                   ))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(request.ConflictsRemarks))
                        {
                            var data = UtilityMethods.Deserialize<List<PresentationConflictRemarks>>(request.ConflictsRemarks);
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
                    MSSLEBSConflictRemark = new List<PresentationConflictRemarks>();
                    foreach (var detail in request.BulkRequestDetails)
                        SAPSODConflictBusinessRoles(detail.UserId, detail.UserName, detail.Name, request.Id, true, applicationType);
                }
            }
            if ((request.ApplicationType == ApplicationType.MSSL || request.ApplicationType == ApplicationType.EBS) && (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.NewAccount))
            {
                string applicationType = "";
                var ep = DomainApiClient<EndPoint>.GetById(EndpointSystemId);
                applicationType = ep.ApplicationCode;

                divEBSMSSLConflictsRemarks.Visible = false;
                if ((request.ProgressStatus == AccessRequestProgress.Completed || request.ProgressStatus == AccessRequestProgress.ManuallyResolved || request.ProgressStatus == AccessRequestProgress.Withdrawn
                   || request.ProgressStatus == AccessRequestProgress.RequestRejected || request.ProgressStatus == AccessRequestProgress.ReadytoExecute || request.ProgressStatus == AccessRequestProgress.ExecutionFailed
                    || request.ProgressStatus == AccessRequestProgress.Executing
                   ))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(request.ConflictsRemarks))
                        {
                            var data = UtilityMethods.Deserialize<List<PresentationConflictRemarks>>(request.ConflictsRemarks);
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
                    MSSLEBSConflictRemark = new List<PresentationConflictRemarks>();
                    foreach (var detail in request.BulkRequestDetails)
                        SAPHANASODConflictBusinessRoles(detail.UserId, detail.UserName, detail.Name, request.Id, true, applicationType);
                }
            }
            #region Start Date End Date Added
            ValidFromRow.Visible = request.ActivationDate != null;
            this.lblValidFrom.Text = (request.ActivationDate != null) ? request.ActivationDate.Value.ToString("dd-MM-yyyy") : "";
            ValidToRow.Visible = request.ActivationDate != null;
            this.lblValidTo.Text = (request.DeactivationDate != null) ? request.DeactivationDate.Value.ToString("dd-MM-yyyy") : "";
            #endregion

            #region Adding Requestor Comments
            if (request.Description != null && request.Description.Trim() != string.Empty)
            {
                RequestorCommentsRow.Visible = true;
                request.Description = request.Description.Trim().Replace("\n", "<br/>");
                lblRequestorCommentstxt.Text = request.Description;

            }
            #endregion

            string action = "Add";
            if (request.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                action = "Remove";
            }
            RequestedUser.Clear();

            request.BulkRequestDetails.ForEach(l =>
            {
                RequestedUser = RequestedUser;
                if (!(RequestedUser.Select(lq => lq.Id).Contains(l.UserId)))
                    RequestedUser.Add(GetUser(l.UserId));
            });

            RequestedUserDetails.Clear();
            request.BulkRequestDetails.ForEach(l =>
            {
                RequestedUserDetails = RequestedUserDetails;
                if (!(RequestedUserDetails.Select(lq => lq.User.Id).Contains(l.UserId)))
                {
                    BulkRequestUserDetails obj = new BulkRequestUserDetails();
                    obj.User = GetUser(l.UserId);
                    obj.AccountName = l.AccountName;
                    obj.HandPhone = l.HandPhone;
                    RequestedUserDetails.Add(obj);
                }
            });
            if (request.AdhocActionType == AdHocActionType.NewAccount)
            {
                UserRequestGrid.MasterTableView.Columns.FindByUniqueName("AccountName").Visible = false;
            }
            UserRequestGrid.Rebind();

            if (request.ApplicationType == ApplicationType.VPN)
            {
                this.VPNRequestIDLabel.Text = request.Id.ToString();
                this.VPNPanel.Visible = true;
            }
            else
            {
                if (request.EndpointSystemId > 0 && request.EndPoint != null)
                {
                    this.SystemNameLabel.Text = request.EndPoint.DisplayName;
                }
                else
                {
                    this.SystemNameLabel.Text = UtilityMethods.GetEnumAttribute<Description>(request.ApplicationType);

                }
                this.GenericPanel.Visible = true;
            }
            this.RequestIDLabel.Text = request.Id.ToString();
            if (request != null)
            {
                if (request.RequestStatus == AccessRequestStatus.Withdrawn)
                {
                    WithdrawCommentsRow.Visible = true;
                    WithdrawReasonLabelLabel.Text = request.Comments;
                    WithdrawnOnRow.Visible = true;
                    WithdrawnOnLabel.Text = request.LastModifiedDate.ToString();
                }
                string hstName = request.EndpointSystemId > 0 && request.EndPoint != null ? request.EndPoint.DisplayName : "";
                string requestName = hstName + " - " + UtilityMethods.GetEnumAttribute<Description>(request.AdhocActionType);
                string acctType = UtilityMethods.GetEnumAttribute<Description>(AccountType.User);
                this.RequestTypeLabel.Text = requestName;
                this.AccountTypeLabel.Text = acctType;

                #region SSIDS
                if (request.ApplicationType == ApplicationType.SSIDS_GUI || request.ApplicationType == ApplicationType.SSIDS_SMS)
                {
                    AdHocSSIDSRequest SSIDSRequest = (AdHocSSIDSRequest)request.GetEntitlementDetail();
                    if (SSIDSRequest != null)
                    {
                        EntitlementTypetr.Visible = true;
                        EntitlementTypeLabel.Text = UtilityMethods.GetEnumAttribute<Description>(SSIDSRequest.EntitlementType);
                        AccessNameLabel.Text = SSIDSRequest.Entitlement;
                    }
                }

                #endregion

                #region IDMS
                if (request.ApplicationType == ApplicationType.IDMS)
                {
                    AdHocIACMRequest adHocIACMRequest = (AdHocIACMRequest)request.GetEntitlementDetail();
                    if (adHocIACMRequest != null && adHocIACMRequest.AdHocIACMRequestDetails != null && adHocIACMRequest.AdHocIACMRequestDetails.Count > 0)
                    {
                        DivIDMS.Visible = true;
                        RadGridIacm.DataSource = adHocIACMRequest.AdHocIACMRequestDetails.Select(item => new { EntitlementType = UtilityMethods.GetEnumAttribute<Description>(item.EntitlementType), Entitlement = item.Entitlement });
                        RadGridIacm.DataBind();
                        RadGridIacm.Visible = true;
                    }
                }

                #endregion


                #region ADS
                if (request.ApplicationType == ApplicationType.ADSingaporePowerLocal)// || request.ApplicationType == ApplicationType.ADSDCLocal)
                {

                    tblgroup.Visible = true;
                    //trADGroup.Visible = true;
                    Session["EndpointSystemIdValue"] = request.EndpointSystemId;
                    hdnADRequest.Value = request.Id.ToString();
                    AdHocADRequest ADRequest = (AdHocADRequest)request.GetEntitlementDetail();
                    if (ADRequest != null)
                    {
                        trreqComment.Visible = !string.IsNullOrEmpty(ADRequest.RequesterComment);
                        if (ADRequest.RequesterComment != null)
                            lblRequesterComment.Text = ADRequest.RequesterComment.Replace("\n", "<br/>");
                    }

                    AdHocADRequest adDetail = (AdHocADRequest)request.GetEntitlementDetail();
                    if (null == adDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                        return;
                    }
                    this.AccessNameLabel.Text = adDetail.Entitlement;
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblRequesterComment.Text.Replace("\n", "<br/>");


                        lblLogonUserName.Text = "";
                    }
                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                    if (request.AdhocActionType == AdHocActionType.NewAccount || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;
                        return;
                    }
                }
                #endregion

                #region LNS
                //else if (request.ApplicationType == ApplicationType.LotusNotes)
                //{
                //    AdHocLNSRequest lnDetail = (AdHocLNSRequest)request.GetEntitlementDetail();
                //    if (null == lnDetail)
                //    {
                //        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                //        return;
                //    }
                //    this.AccessNameLabel.Text = lnDetail.Entitlement;
                //    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                //    {
                //        lblLogonUserName.Text = new UserFacade().GetEndPointUserDetailByEndpointUserID(lnDetail.EndpointUserId, "LNS");
                //    }
                //    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                //    if (request.AdhocActionType == AdHocActionType.DeleteAccount)
                //    {
                //        this.AccessNameLabel.Visible = false;
                //        this.AccessNametr.Visible = AccessNameLabel.Visible;
                //        this.Label3.Visible = false;
                //        return;
                //    }
                //}
                #endregion

                #region SMS
                else if (request.ApplicationType == ApplicationType.OneCard)
                {
                    AdHocSMSRequest adhocSMSRequest = (AdHocSMSRequest)request.GetEntitlementDetail();
                    if (adhocSMSRequest != null)
                    {
                        IList<object> listofrowsNeeded = new List<object>();
                        SMSTR.Visible = true;
                        SMSObjGrid.Visible = true;
                        AccessNameLabel.Text = adhocSMSRequest.Entitlement;
                        SMSObjGrid.DataSource = adhocSMSRequest.GridInfo;
                        SMSObjGrid.DataBind();
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                    }
                }
                #endregion

                #region SRM
                else if (request.ApplicationType == ApplicationType.SRM)
                {
                    AdHocSAPSRMRequest adhocSRMRequest = (AdHocSAPSRMRequest)request.GetEntitlementDetail();
                    if (adhocSRMRequest != null)
                    {
                        IList<object> listofrowsNeeded = new List<object>();
                        TRSRMAccessGrid.Visible = true;
                        SRMObjGrid.Visible = true;
                        //AccessNameLabel.Text = adhocSRMRequest.;
                        SRMObjGrid.DataSource = adhocSRMRequest.AdHocSAPSRMRequestDetails;
                        SRMObjGrid.DataBind();
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                    }
                }
                #endregion

                #region O365
                else if (request.ApplicationType == ApplicationType.Office365)
                {
                    AdHocO365Request adhocO365Request = (AdHocO365Request)request.GetEntitlementDetail();


                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);

                    this.AccessNameLabel.Visible = false;
                    this.AccessNametr.Visible = AccessNameLabel.Visible;


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

                    return;

                }
                #endregion

                #region HRMSFLINS
                else if (request.ApplicationType == ApplicationType.HRMS_FLINS)
                {

                    AdHocSAPHRFLINSRequest flinsDetail = (AdHocSAPHRFLINSRequest)request.GetEntitlementDetail();
                    if (null == flinsDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                        return;
                    }

                    var detail = flinsDetail.AdHocSAPHRFLINSRequestDetails;
                    FlinsEntitlementList = detail;
                    trHRMSFLINS.Visible = HRMSGRID.Visible = detail.Count > 0 ? true : false;
                    HRMSGRID.DataSource = detail;
                    HRMSGRID.DataBind();
                    HRMSGRID.MasterTableView.GetColumn("IsCompanyRetain").Visible = request.AdhocActionType == AdHocActionType.AmendmentRemove;
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
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(flinsDetail.EndpointUserId, "SAP");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                    if (request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;

                        return;
                    }
                }
                #endregion

                #region FLINSBI
                else if (request.ApplicationType == ApplicationType.FLINS_BI)
                {
                    AdHocSAPFLINSBIRequest sapFLINSBIDetail = (AdHocSAPFLINSBIRequest)request.GetEntitlementDetail();
                    if (null == sapFLINSBIDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                        return;
                    }
                    this.AccessNameLabel.Text = sapFLINSBIDetail.Entitlement;
                    if (!string.IsNullOrWhiteSpace(sapFLINSBIDetail.Entitlement))
                    {
                        PositionLinkButton.Visible = true;
                    }
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(sapFLINSBIDetail.EndpointUserId, "SAP");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                }
                #endregion

                #region SAPPI
                else if (request.ApplicationType == ApplicationType.SAP_PI)
                {
                    AdHocSAPPIRequest piDetail = (AdHocSAPPIRequest)request.GetEntitlementDetail();
                    if (null == piDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                        return;
                    }
                    this.AccessNameLabel.Text = piDetail.Entitlement;
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(piDetail.EndpointUserId, "SAP");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                }
                #endregion

                #region SAPGRC
                else if (request.ApplicationType == ApplicationType.SAP_GRC)
                {
                    AdHocSAPGRCRequest grcDetail = (AdHocSAPGRCRequest)request.GetEntitlementDetail();
                    if (null == grcDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                        return;
                    }
                    this.AccessNameLabel.Text = grcDetail.Entitlement;
                    if (!string.IsNullOrWhiteSpace(grcDetail.Entitlement))
                    {
                        PositionLinkButton.Visible = true;
                    }
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(grcDetail.EndpointUserId, "SAP");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                }
                #endregion

                #region PLRElearning
                //else if (request.ApplicationType == ApplicationType.eLearning)
                //{
                //    AdHocPLRELearningRequest elDetail = (AdHocPLRELearningRequest)request.GetEntitlementDetail();
                //    if (null == elDetail)
                //    {
                //        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                //        return;
                //    }
                //    this.AccessNameLabel.Text = elDetail.Entitlement;
                //    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                //    {
                //        lblLogonUserName.Text = new UserFacade().GetEndPointUserDetailByEndpointUserID(elDetail.EndpointUserId, "SAP");
                //    }
                //    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                //}
                #endregion

                #region PERErecruitment
                //else if (request.EntitlementDetailType == ApplicationType.eRecruitment)
                //{
                //    AdHocPERERecruitmentRequest elDetail = (AdHocPERERecruitmentRequest)request.GetEntitlementDetail();
                //    if (null == elDetail)
                //    {
                //        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                //        return;
                //    }
                //    this.AccessNameLabel.Text = elDetail.Entitlement;
                //    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                //    {
                //        lblLogonUserName.Text = new UserFacade().GetEndPointUserDetailByEndpointUserID(elDetail.EndpointUserId, "SAP");
                //    }
                //    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                //}
                #endregion

                #region VPN
                else if (request.ApplicationType == ApplicationType.VPN)
                {
                    AdHocVPNRequest vpnDetail = (AdHocVPNRequest)request.GetEntitlementDetail();
                    if (null == vpnDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                        return;
                    }
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(vpnDetail.EndpointUserId, "AD");
                    }
                    if (request.Description != null)
                    {
                        request.Description = request.Description.Replace("\n", "<br>");
                    }
                    lblAdhoccomments.Text = request.Description;


                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                    this.VPNApplicationsGrid.DataSource = vpnDetail.AdHocVPNRequestDetails.AsEnumerable().Select(c => c);
                    this.VPNApplicationsGrid.Rebind();
                    VPNApplicationsGrid.Visible = true;
                    if (request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        VPNApplicationsGrid.Visible = false;
                    }

                    //this.OthersGrid.DataSource = vpnDetail.AdHocVPNRequestDetails.AsEnumerable().Where(c => c.isOthers).Select(c => c);
                    //this.OthersGrid.Rebind();

                    //if (vpnDetail.AdHocVPNRequestDetails.AsEnumerable().Where(c => c.isOthers).Select(c => c).Count() > 0)
                    //{
                    //    OthersGrid.Visible = true;
                    //}
                    //else
                    //{
                    //    OthersGrid.Visible = false;
                    //}

                }
                #endregion

                #region InhouseSystems
                else if (request.ApplicationType == ApplicationType.InhouseSystems)
                {
                    AdHocINHRequest inhDetail = (AdHocINHRequest)request.GetEntitlementDetail();
                    if (null == inhDetail)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                        return;
                    }
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(inhDetail.EndPointUserID, "INH");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                    this.AccessNameLabel.Text = inhDetail.Entitlement;
                }
                #endregion

                #region Miscellaneous
                //else if (request.ApplicationType == ApplicationType.Miscellaneous)
                //{
                //    this.AccessNameLabel.Text = request.MiscSystem.SystemName;
                //    AdHocMiscRequest adhocMisc = (AdHocMiscRequest)request.GetEntitlementDetail();
                //    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                //    {
                //        var miscSystem = new InhouseSystemFacade().GetMiscSystemById(request.MiscSystemId.Value);
                //        if (miscSystem.MiscSystemType == MiscSystemType.ExternalEmailAccess)
                //        {
                //            lblLogonUserName.Text = new UserFacade().GetEndPointUserDetailByEndpointUserID(adhocMisc.EndpointUserId, "LNS");
                //        }
                //        else
                //        {
                //            lblLogonUserName.Text = new UserFacade().GetEndPointUserDetailByEndpointUserID(adhocMisc.EndpointUserId, "AD");
                //        }
                //    }
                //    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                //}
                #endregion

                #region GEMS
                else if (request.ApplicationType == ApplicationType.GEMS)
                {
                    AdHocGEMSRequest adHocGEMSRequest = (AdHocGEMSRequest)request.GetEntitlementDetail();
                    if (request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;
                    }
                    else
                    {
                        if (adHocGEMSRequest != null)
                        {
                            this.AccessNameLabel.Text = this.AccessNameLabel.Text = adHocGEMSRequest.Entitlement;
                            this.AccessNameLabel.Visible = string.IsNullOrEmpty(adHocGEMSRequest.Entitlement) ? false : true;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                        }
                        else
                            this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;

                    }
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocGEMSRequest.EndpointUserId, "GEMS");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                }
                #endregion

                #region MSSL
                else if (request.ApplicationType == ApplicationType.MSSL_ISU || request.ApplicationType == ApplicationType.MSSL_CRM)// || request.ApplicationType == ApplicationType.MSSL_BI)
                {
                    var data = request.GetEntitlementDetail();
                    divEBSMSSLConflictsRemarks.Visible = false;
                    AdHocMSSLRequest adHocMSSLRequest = (AdHocMSSLRequest)data;
                    //if (request.ApplicationType != ApplicationType.MSSL_BI)
                    //    this.SystemNameLabel.Text = "MSSL ISU & CRM";
                    if (MSSLEBSConflictRemark != null && MSSLEBSConflictRemark.Count > 0)
                    {
                        divEBSMSSLConflictsRemarks.Visible = true;
                        rgEBSMSSLConflictsRemarks.DataSource = MSSLEBSConflictRemark;
                        rgEBSMSSLConflictsRemarks.Rebind();
                    }

                    if (request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;
                    }
                    else
                    {
                        if (adHocMSSLRequest != null)
                        {
                            if (!string.IsNullOrWhiteSpace(adHocMSSLRequest.Position))
                            {
                                PositionLinkButton.Visible = true;
                            }
                            //this.AccessNameLabel.Text = adHocMSSLRequest.Entitlement;
                            this.AccessNameLabel.Text = string.IsNullOrEmpty(adHocMSSLRequest.Position) ? "Roles : " + adHocMSSLRequest.Roles : string.IsNullOrEmpty(adHocMSSLRequest.Roles) ? "Position : " + adHocMSSLRequest.Position : "Position : " + adHocMSSLRequest.Position + "<br>" + "Roles    : " + adHocMSSLRequest.Roles;
                            this.AccessNameLabel.Visible = string.IsNullOrEmpty(adHocMSSLRequest.Entitlement) ? false : true;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                            if (request.AdhocActionType == AdHocActionType.AmendmentAdd)
                            {
                                if (adHocMSSLRequest.IsReallocate)
                                {
                                    this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Reallocate Position)";
                                }
                                //else
                                //{
                                //    this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Add Position)";
                                //}
                            }
                        }
                        else
                            this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;

                    }
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocMSSLRequest.EndpointUserId, "SAP");
                    }

                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                }
                #endregion

                #region EBS
                else if (request.ApplicationType == ApplicationType.EBS_ISU || request.ApplicationType == ApplicationType.EBS_CRM)// || request.ApplicationType == ApplicationType.EBS_BI)
                {
                    var data = request.GetEntitlementDetail();

                    AdHocEBSRequest adHocEBSRequest = (AdHocEBSRequest)data;
                    //if (request.ApplicationType != ApplicationType.EBS_BI)
                    //    this.SystemNameLabel.Text = "EBS ISU & CRM";
                    divEBSMSSLConflictsRemarks.Visible = false;
                    if (MSSLEBSConflictRemark != null && MSSLEBSConflictRemark.Count > 0)
                    {
                        divEBSMSSLConflictsRemarks.Visible = true;
                        rgEBSMSSLConflictsRemarks.DataSource = MSSLEBSConflictRemark;
                        rgEBSMSSLConflictsRemarks.Rebind();
                    }

                    if (request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = string.IsNullOrEmpty(this.AccessNameLabel.Text) ? false : true;
                    }
                    else
                    {
                        if (adHocEBSRequest != null)
                        {
                            if (!string.IsNullOrWhiteSpace(adHocEBSRequest.Position))
                            {
                                PositionLinkButton.Visible = true;
                            }
                            //this.AccessNameLabel.Text = adHocEBSRequest.Entitlement;
                            this.AccessNameLabel.Text = string.IsNullOrEmpty(adHocEBSRequest.Position) ? "Roles : " + adHocEBSRequest.Roles : string.IsNullOrEmpty(adHocEBSRequest.Roles) ? "Position : " + adHocEBSRequest.Position : "Position : " + adHocEBSRequest.Position + "<br>" + "Roles    : " + adHocEBSRequest.Roles;
                            this.AccessNameLabel.Visible = string.IsNullOrEmpty(adHocEBSRequest.Entitlement) ? false : true;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                            if (request.AdhocActionType == AdHocActionType.AmendmentAdd)
                            {
                                if (adHocEBSRequest.IsReallocate)
                                {
                                    this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Reallocate Position)";
                                }
                                //else
                                //{
                                //    this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Add Position)";
                                //}
                            }
                        }
                        else
                            this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;

                    }
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocEBSRequest.EndpointUserId, "SAP");
                    }

                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                }
                #endregion
                #region SAPHANA
                else if (request.ApplicationType == ApplicationType.EBS || request.ApplicationType == ApplicationType.MSSL)
                {
                    var data = request.GetEntitlementDetail();

                    var adHocSAPHANARequest = (AdHocSAPHANARequest)data;
                    divEBSMSSLConflictsRemarks.Visible = false;
                    if (MSSLEBSConflictRemark != null && MSSLEBSConflictRemark.Count > 0)
                    {
                        divEBSMSSLConflictsRemarks.Visible = true;
                        rgEBSMSSLConflictsRemarks.DataSource = MSSLEBSConflictRemark;
                        rgEBSMSSLConflictsRemarks.Rebind();
                    }

                    if (request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = string.IsNullOrEmpty(this.AccessNameLabel.Text) ? false : true;
                    }
                    else
                    {
                        if (adHocSAPHANARequest != null)
                        {
                            if (!string.IsNullOrWhiteSpace(adHocSAPHANARequest.Position))
                            {
                                PositionLinkButton.Visible = true;
                            }
                            //this.AccessNameLabel.Text = adHocEBSRequest.Entitlement;
                            this.AccessNameLabel.Text = string.IsNullOrEmpty(adHocSAPHANARequest.Position) ? "Roles : " + adHocSAPHANARequest.Roles : string.IsNullOrEmpty(adHocSAPHANARequest.Roles) ? "Position : " + adHocSAPHANARequest.Position : "Position : " + adHocSAPHANARequest.Position + "<br>" + "Roles    : " + adHocSAPHANARequest.Roles;
                            this.AccessNameLabel.Visible = string.IsNullOrEmpty(adHocSAPHANARequest.Entitlement) ? false : true;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                            if (request.AdhocActionType == AdHocActionType.AmendmentAdd)
                            {
                                if (adHocSAPHANARequest.IsReallocate)
                                {
                                    this.AccessNameLabel.Text = this.AccessNameLabel.Text + " (Reallocate Position)";
                                }
                            }
                        }
                        else
                            this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;

                    }
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";
                    }

                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                }
                #endregion
                #region NPAC
                else if (request.ApplicationType == ApplicationType.NPAC)
                {
                    AdHocNPACRequest adHocNPACRequest = (AdHocNPACRequest)request.GetEntitlementDetail();
                    if (request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = string.IsNullOrEmpty(this.AccessNameLabel.Text) ? false : true;
                    }
                    else
                    {
                        if (adHocNPACRequest != null)
                        {
                            this.AccessNameLabel.Text = this.AccessNameLabel.Text = "Level " + adHocNPACRequest.Entitlement;
                            this.AccessNameLabel.Visible = string.IsNullOrEmpty(adHocNPACRequest.Entitlement) ? false : true;
                            this.AccessNametr.Visible = AccessNameLabel.Visible;
                        }
                        else
                            this.AccessNameLabel.Visible = false;
                        this.AccessNametr.Visible = AccessNameLabel.Visible;

                    }
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocNPACRequest.EndpointUserId, "NPAC");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                }
                #endregion

                #region SAPEP
                else if (request.ApplicationType == ApplicationType.SAP_EP || request.ApplicationType == ApplicationType.CE)// || request.ApplicationType == ApplicationType.InternetPortal)
                {

                    AdhocSAPEPRequest adHocSAPEPRequest = (AdhocSAPEPRequest)request.GetEntitlementDetail();

                    if (null == adHocSAPEPRequest)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                        return;
                    }
                    if (adHocSAPEPRequest != null && !Page.ToString().Contains("myrequestdetails"))
                    {
                        if (!string.IsNullOrEmpty(adHocSAPEPRequest.Groups))
                        {

                            // AccessNameLabel.Text = adHocSAPEPRequest.Groups;
                        }
                        if ((!string.IsNullOrEmpty(adHocSAPEPRequest.Roles) && (!string.IsNullOrEmpty(adHocSAPEPRequest.Roles))))
                        {
                            // AccessNameLabel.Text = adHocSAPEPRequest.Roles + "," + adHocSAPEPRequest.Groups;
                        }
                    }
                    this.RequestInfoLabel.Text = adHocSAPEPRequest.RequestDetail.Replace("\n", "<br/>");
                    SAPEPRequestInfotr.Visible = string.IsNullOrEmpty(adHocSAPEPRequest.RequestDetail) ? false : true;
                    if (request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove || request.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        lblLogonUserName.Text = "";// new UserFacade().GetEndPointUserDetailByEndpointUserID(adHocSAPEPRequest.EndpointUserId, "SAPEP");
                    }
                    LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);

                    if (adHocSAPEPRequest != null)
                    {
                        if (!string.IsNullOrEmpty(adHocSAPEPRequest.Groups))
                        {
                            trPortalGrp.Visible = true;
                            lblSelectedGrp.Text = adHocSAPEPRequest.Groups.Replace(",", "<br/>");
                        }
                        if (!string.IsNullOrEmpty(adHocSAPEPRequest.Roles))
                        {
                            trPortalrole.Visible = true;
                            lblSelectedRole.Text = adHocSAPEPRequest.Roles.Replace(",", "<br/>");
                        }
                    }
                }
                #endregion
                #region skypeforbusines
                else if (request.ApplicationType == ApplicationType.MicrosoftTeams)
                {
                    LogonUserNametr.Visible = false;
                    divSkypeForBusiness.Visible = true;
                    ResponseGroupGrid.Visible = true;
                    AdhocSkypeForBusinessRequest adhocSkypeForBusinessRequest = (AdhocSkypeForBusinessRequest)request.GetEntitlementDetail();

                    if (null == adhocSkypeForBusinessRequest)
                    {
                        LogonUserNametr.Visible = !string.IsNullOrEmpty(lblLogonUserName.Text);
                        return;
                    }
                    ResponseGroupGrid.DataSource = adhocSkypeForBusinessRequest.SkypeForBusinessResponseGroups;
                    ResponseGroupGrid.DataBind();
                }
                #endregion
                this.AccessNametr.Visible = string.IsNullOrEmpty(this.AccessNameLabel.Text) ? false : true;


            }
        }

        private Dictionary<int, string> SelectedRoles
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



        public void BindADGroupInfo(DI.Chakra.Dto.Task task)
        {
            if (!task.BulkRequestId.HasValue) return;
            Dto.BulkRequest bulkRequest = DomainApiClient<Dto.BulkRequest>.GetById(task.BulkRequestId.Value, true);
            if (bulkRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal) //||  bulkRequest.ApplicationType == ApplicationType.ADSDCLocal)
            {
                int GroupID = DBConfigHelper.MasterConfigHelper.WintelGroupId;
                bool IsWintelGroup = DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.CheckUserGroup,
                    new Dictionary<string, string> { { "userId",
                                                                (task.ActionUserId.HasValue) ? task.ActionUserId.ToString() : ( (task.AssignedTo.HasValue)? task.AssignedTo.ToString() : new SessionStatusImpl().User.Id.ToString() ) }, { "GroupID", GroupID.ToString() } });

                if (IsWintelGroup)
                {
                    trGroups.Visible = !(bulkRequest.AdhocActionType == AdHocActionType.DeleteAccount);
                    bool visible = (bulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd || bulkRequest.AdhocActionType == AdHocActionType.NewAccount);
                    trDiscoveryGroups.Visible = visible;
                    trDiscoveryGroupsNotes.Visible = visible;
                    tdbtn.Visible = !(bulkRequest.AdhocActionType == AdHocActionType.DeleteAccount);
                    tdgrid.Visible = !(bulkRequest.AdhocActionType == AdHocActionType.DeleteAccount);
                    tdgroup.Visible = !(bulkRequest.AdhocActionType == AdHocActionType.DeleteAccount);
                    tdexistinggroup.Visible = false;
                    //if (task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    //    BindExistingGroups(task);
                }
            }
        }

        private void BindExistingGroups(Dto.Task task)
        {
            if (!task.BulkRequestId.HasValue) return;
            Dto.BulkRequest bulkRequest = DomainApiClient<Dto.BulkRequest>.GetById(task.BulkRequestId.Value, true);
            AdHocADRequest ADRequest = (AdHocADRequest)bulkRequest.GetEntitlementDetail();
            long endpointUserId = ADRequest.EndpointUserId;
            if (endpointUserId > 0 && task.AccessRequest != null && task.AccessRequest.AdHocAccessRequest != null)
            {

                var groups = DomainApiClient<Dto.ADSGroupInfo>.GetByAction<List<Dto.ADSGroupInfo>>(DomainApiAction.Endpoint.ActiveDirectory.ADSGroupInfo.GetADGroupsByUserIdAndEndpointId,
                    new Dictionary<string, string>{
                        { "userId", endpointUserId.ToString() },
                        { "EndpointId", task.AccessRequest.AdHocAccessRequest.EndpointSystemId.HasValue? task.AccessRequest.AdHocAccessRequest.EndpointSystemId.Value.ToString() : "0" } });


                // var groups = new ActiveDirectoryFacade().GetADGroupsByUserId(endpointUserId, task.AccessRequest.AdHocAccessRequest.EndpointSystemId.Value);

                this.ExistingGroupComboBox.DataSource = groups.Select(x => new { Key = x.Id, Value = x.name })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);

                this.ExistingGroupComboBox.DataBind();
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
                string strJobs = string.Empty;
                Dto.BulkRequest bulkRequest = DomainApiClient<Dto.BulkRequest>.GetById(this.BulkRequest.Id, true);
                if (bulkRequest.ApplicationType != ApplicationType.SAPLocal)
                {
                    strJobs = DBConfigHelper.MasterConfigHelper.ADDiscoveryJobForWintelTeam;
                }
                else
                {
                    strJobs = DBConfigHelper.MasterConfigHelper.SAPLOCALDiscoveryJobForWintelTeam;
                }

                List<string> jobs = new List<string>();

                if (string.IsNullOrEmpty(strJobs))
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

        void ScheduleForRunNow(long ProcessId)
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
                Dto.BulkRequest bulkRequest = DomainApiClient<Dto.BulkRequest>.GetById(this.BulkRequest.Id, true);
                string strJobs = string.Empty;

                if (bulkRequest.ApplicationType != ApplicationType.SAPLocal)
                {
                    strJobs = DBConfigHelper.MasterConfigHelper.ADDiscoveryJobForWintelTeam;
                }
                else
                {
                    strJobs = DBConfigHelper.MasterConfigHelper.SAPLOCALDiscoveryJobForWintelTeam;
                }

                List<string> jobs = new List<string>();

                if (string.IsNullOrEmpty(strJobs))
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
                    }
                    if (lblStatus.Text == "Pending")
                    {
                        lblStatus.CssClass = "description label label-warning";
                    }
                    if (lblStatus.Text == "Completed")
                    {
                        lblStatus.CssClass = "description label label-success";
                    }
                    if (lblStatus.Text == "Failed")
                    {
                        lblStatus.CssClass = "description label label-danger";
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
                    var roles = this.SelectedRoles;
                    if (this.AppType == ApplicationType.ADSingaporePowerLocal && (this.IdmsAdhocActionType == DI.Chakra.Infrastructure.Enums.AdHocActionType.AmendmentAdd
                            || this.IdmsAdhocActionType == DI.Chakra.Infrastructure.Enums.AdHocActionType.NewAccount
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
            //if (this.RolesAutoCompleteBox.Entries.Count != 0)
            //{

            //    int roleId = Convert.ToInt32(this.RolesAutoCompleteBox.Entries[0].Value);
            //    string role = this.RolesAutoCompleteBox.Entries[0].Text;
            //    var roles = this.SelectedRoles;

            //    if (!roles.ContainsKey(roleId))
            //    {
            //        roles.Add(roleId, role);
            //    }

            //    this.SelectedRoles = roles;

            //    this.RebindRolesGrid();

            //    this.RolesAutoCompleteBox.Entries.Clear();
            //}
            //else
            //{
            //    ShowAlertMessage(AlertType.Error, "Please select at least one group");
            //}
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

        public bool UpdateEntitlement(Dto.Task task)
        {
            if (!task.BulkRequestId.HasValue) return false;
            Dto.BulkRequest bulkRequest = DomainApiClient<Dto.BulkRequest>.GetById(task.BulkRequestId.Value, true);
            if (bulkRequest == null) return false;
            string Message = "For the following user(s) ";
            string UserAffected = "";
            foreach (GridDataItem grow in UserRequestGrid.Items)
            {
                TextBox txtOneKey = (TextBox)grow.FindControl("txtOneKey");
                if (txtOneKey != null)
                {
                    if (string.IsNullOrEmpty(txtOneKey.Text))
                    {
                        UserAffected = string.IsNullOrEmpty(UserAffected) ? Convert.ToString(grow.GetDataKeyValue("DisplayName")) : UserAffected + ", " + Convert.ToString(grow.GetDataKeyValue("DisplayName"));
                    }

                }
            }
            if (UserAffected != "")
            {
                Message += UserAffected.LastIndexOf(",") > 0 ? UserAffected.Substring(0, UserAffected.LastIndexOf(",")) + " and" + UserAffected.Substring(UserAffected.LastIndexOf(",") + 1) : UserAffected;
                Message = Message + " please enter one key" + ".";
                ShowAlertMessage(AlertType.Error, Message);
                return false;
            }
            else
            {
                foreach (GridDataItem grow in UserRequestGrid.Items)
                {
                    TextBox txtOneKey = (TextBox)grow.FindControl("txtOneKey");
                    if (txtOneKey != null)
                    {
                        var userId = grow.GetDataKeyValue("User.UserId");
                        if (userId != null)
                        {
                            User user = DomainApiClient<Dto.User>.GetById(Convert.ToInt32(userId));
                            user.OneKeyTokenId = txtOneKey.Text;

                            if (!string.IsNullOrEmpty(task.GroupName))
                            {
                                Dto.Group group = DomainApiClient<Dto.Group>.GetByAction<Dto.Group>(
                                                        DomainApiAction.Group.GetByGroupName,
                                                        new Dictionary<string, string> { { "groupName", task.GroupName } }
                                                        );
                                long groupId = group.Id;
                                // var groupId = string.IsNullOrEmpty(task.GroupName) ? 0 : DomainApiClient<Dto.Group>.GetByAction<Dto.Group> new GroupFacade().GetGroupIdByName(task.GroupName);
                                if (groupId == DBConfigHelper.MasterConfigHelper.ITSecurityGroupId && bulkRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                                    user.OneKeyTokenId = null;
                            }
                            DomainApiClient<Dto.User>.Save(user);
                        }
                    }
                }
                return true;
            }
        }

        public void UpdateADEntitlement(long TaskId, out string ValidationMessage)
        {
            string role = "";
            ValidationMessage = "";
            Dto.Task task = DomainApiClient<Dto.Task>.GetById(TaskId, true);
            if (task == null || !task.BulkRequestId.HasValue) return;

            Dto.BulkRequest bulkRequest = DomainApiClient<Dto.BulkRequest>.GetById(task.BulkRequestId.Value, true);
            if (bulkRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)//|| task.BulkRequest.EntitlementDetailType == ApplicationType.ADSDCLocal))
            {
                int GroupID = DBConfigHelper.MasterConfigHelper.WintelGroupId;

                bool IsWintelGroup = DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.CheckUserGroup,
                    new Dictionary<string, string> { { "userId",
                                                     (task.ActionUserId.HasValue) ? task.ActionUserId.ToString() : ( (task.AssignedTo.HasValue)? task.AssignedTo.ToString() : new SessionStatusImpl().User.Id.ToString() )  },
                                                     { "GroupID", GroupID.ToString() } });

                if (IsWintelGroup)
                {
                    //if (task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    //{
                    //    var selectedRoles = this.ExistingGroupComboBox.CheckedItems.Select(item => item.Text).ToList();
                    //    if (selectedRoles.Count == 0)
                    //    {
                    //        ValidationMessage = "sss";
                    //    }
                    //    else
                    //    {
                    //        foreach (string Role in selectedRoles)
                    //        {
                    //            if (role == "")
                    //                role = role + Role;
                    //            else
                    //                role = role + "," + Role;
                    //        }
                    //    }
                    //}

                    //else
                    {
                        if (SelectedRoles.Count == 0 && (bulkRequest.AdhocActionType != AdHocActionType.NewAccount && bulkRequest.AdhocActionType != AdHocActionType.DeleteAccount))
                        {
                            ValidationMessage = $"Please add at least one Group Name for {bulkRequest.AdhocActionType}";
                        }
                        else if (SelectedRoles.Count == 0 && (bulkRequest.AdhocActionType == AdHocActionType.NewAccount) && bulkRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
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
                    if (role != "")
                    {
                        var adhocAcessRequest = bulkRequest;
                        AdHocADRequest ADRequest = (AdHocADRequest)bulkRequest.GetEntitlementDetail();
                        if (ADRequest != null)
                        {
                            ADRequest.Entitlement = role;
                            adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(ADRequest);

                            DomainApiClient<Dto.BulkRequest>.Save(adhocAcessRequest);
                        }
                    }
                    if (!string.IsNullOrEmpty(ValidationMessage))
                        ShowAlertMessage(AlertType.Error, ValidationMessage);
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
                    if (!string.IsNullOrEmpty(CostCenter))
                        VarianceLabel.Text = "Cost Center : " + CostCenter;
                    else if (!string.IsNullOrEmpty(MRPController))
                        VarianceLabel.Text = "MRP Controller : " + MRPController;
                    else if (!string.IsNullOrEmpty(Plant))
                        VarianceLabel.Text = "Plant : " + Plant;
                    else if (!string.IsNullOrEmpty(PurchaseGroup))
                        VarianceLabel.Text = "Purchase Group : " + PurchaseGroup;
                    else if (!string.IsNullOrEmpty(PurchaseOrg))
                        VarianceLabel.Text = "Purchase Org : " + PurchaseOrg;
                    else if (!string.IsNullOrEmpty(StorageLocation))
                        VarianceLabel.Text = "Storage Location : " + StorageLocation;
                }
            }
        }
        protected void UserRequestGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            //if (this.RequestedUser == null)
            //{
            //    RequestedUser = new List<User>();
            //}
            //UserRequestGrid.DataSource = this.RequestedUser;
            if (this.RequestedUserDetails == null)
            {
                RequestedUserDetails = new List<BulkRequestUserDetails>();
            }
            UserRequestGrid.DataSource = this.RequestedUserDetails;
        }
        protected void UserRequestGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                Label AccountInfo = (Label)item.FindControl("AccountInfo");
                Label OneKeyTokenId = (Label)item.FindControl("OneKeyTokenId");
                TextBox txtOneKey = (TextBox)item.FindControl("txtOneKey");

                var userInfo = e.Item.DataItem as DI.Chakra.Web.Request.BulkRequest.UC.BulkGenericRequestDetails.BulkRequestUserDetails;
                var userInfoUC = e.Item.FindControl("UserInfoUC") as DI.Chakra.Web.AccessRequest.Common.UserInfoUC;
                userInfoUC.User = DomainApiClient<Dto.User>.GetById(userInfo.User.Id);

                if (EntitlementDetailType == ApplicationType.VPN)
                {
                    UserRequestGrid.MasterTableView.GetColumn("OneKeyTokenVPN").Visible = true;
                    if (GroupID == DBConfigHelper.MasterConfigHelper.NetworkManagement || GroupID == DBConfigHelper.MasterConfigHelper.ITSecurityGroupId)
                    {
                        var dataItem = item.DataItem as BulkRequestUserDetails;
                        var ADdetail = DomainApiClient<Dto.ADSUserInfo>.GetByAction<List<Dto.ADSUserInfo>>(DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.GetByDIUserId, new Dictionary<string, string> { { "DIUserId", dataItem.User.Id.ToString() } });
                        if (ADdetail != null && ADdetail.Any())
                        {
                            AccountInfo.Text = ADdetail.FirstOrDefault()?.sAMAccountName;
                        }
                        else
                            AccountInfo.Text = "(AD account is not created yet, please contact security operations)";
                    }
                    else
                    {
                        UserRequestGrid.MasterTableView.GetColumn("AccountInfo").Visible = false;
                    }
                    var request = DomainApiClient<Dto.BulkRequest>.GetById(BulkRequestID, true);
                    if (request != null)
                    {
                        if (IsActionOnlyRequiredTask && request.AdhocActionType != AdHocActionType.DeleteAccount)
                        {
                            OneKeyTokenId.Visible = false;
                            //txtOneKey.Visible = true;
                        }
                        else
                        {
                            OneKeyTokenId.Visible = true;
                            //if(txtOneKey !=null)
                            //    txtOneKey.Visible = false;
                        }
                    }

                }
                else
                {
                    UserRequestGrid.MasterTableView.GetColumn("AccountInfo").Visible = false;
                    UserRequestGrid.MasterTableView.GetColumn("OneKeyTokenVPN").Visible = false;
                    UserRequestGrid.MasterTableView.GetColumn("HandPhone").Visible = false;
                }

                if (EntitlementDetailType == ApplicationType.SSIDS_SMS)
                { UserRequestGrid.MasterTableView.GetColumn("HandPhone").Visible = true; }
            }
        }

        protected void btnViewAccess_Click(object sender, EventArgs e)
        {
            foreach (GridDataItem item in UserRequestGrid.Items)
            {
                if (item != null)
                {
                    var lnkButton = item.FindControl("lnkViewCurrentAccess") as LinkButton;
                    if (lnkButton != null)
                    {
                        this.MasterPage.Radscriptmanager.RegisterAsyncPostBackControl(lnkButton);
                    }
                }

            }
            CloseWindow();
        }

        void ShowWindow()
        {
            var key = "showWindoww";
            string script = "Sys.Application.remove_load(showWindow);Sys.Application.add_load(showWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindow()
        {
            var key = "closeWindoww";
            string script = "Sys.Application.remove_load(closeWindow);Sys.Application.add_load(closeWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }
        protected void lnkViewCurrentAccess_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "show")
            {

                var userid = Convert.ToInt64(e.CommandArgument);
                ucCurrentAccess.EntityId = userid;
                ucCurrentAccess.HideAccountDetail = true;
                ucCurrentAccess.Rebind();
                ShowWindow();
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
        //protected void btnCancel_Click(object sender, EventArgs e)
        //{
        //    radWindowEntitlementDetail.VisibleOnPageLoad = false;
        //}

        protected void PositionLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (BulkRequest != null && BulkRequest.BulkRequestDetails != null)
                {
                    var positionRoles = new List<string>();
                    if (BulkRequest.ApplicationType == ApplicationType.MSSL_CRM || BulkRequest.ApplicationType == ApplicationType.MSSL_ISU)
                    {
                        var data = BulkRequest.GetEntitlementDetail();
                        AdHocMSSLRequest adHocMSSLRequest = (AdHocMSSLRequest)data;
                        var para = new Dictionary<string, string>();
                        para.Add("position", adHocMSSLRequest.Position);
                        var positionMatrix = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<Dto.SAPXLMSSLPositionsMatrix>(DomainApiAction.Matrix.SAP_MSSL.GetMSSLPositionMatrixByPosition, para);

                        if (positionMatrix != null)
                        {
                            List<string> result = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAP_MSSL.GetRolesByPosition, new Dictionary<string, string> { { "position", BulkRequest.ApplicationType == ApplicationType.MSSL_ISU ? positionMatrix.Position_MSP : positionMatrix.Position_MCP }, { "endpointSystemId", Convert.ToInt32(BulkRequest.EndpointSystemId).ToString() } });
                            positionRoles.AddRange(result);
                            ViewTechnicalSource.DataSource = result;
                            ViewTechnicalSource.DataBind();
                            ViewTechnicalSourceWindow.VisibleOnPageLoad = true;
                        }

                    }
                    else if (BulkRequest.ApplicationType == ApplicationType.EBS_CRM || BulkRequest.ApplicationType == ApplicationType.EBS_ISU)
                    {
                        var data = BulkRequest.GetEntitlementDetail();
                        AdHocEBSRequest adHocEBSRequest = (AdHocEBSRequest)data;

                        var para = new Dictionary<string, string>();
                        para.Add("position", adHocEBSRequest.Position);
                        var positionMatrix = DomainApiClient<Dto.SAPEBSXLPositionsMatrix>.GetByAction<Dto.SAPEBSXLPositionsMatrix>(DomainApiAction.Matrix.SAP_EBS.GetEBSPositionMatrixByPosition, para);
                        if (positionMatrix != null)
                        {
                            List<string> result = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAP_MSSL.GetRolesByPosition, new Dictionary<string, string> { { "position", BulkRequest.ApplicationType == ApplicationType.EBS_ISU ? positionMatrix.Position_ISP : positionMatrix.Position_CRP }, { "endpointSystemId", Convert.ToInt32(BulkRequest.EndpointSystemId).ToString() } });
                            positionRoles.AddRange(result);
                            ViewTechnicalSource.DataSource = positionRoles;
                            ViewTechnicalSource.DataBind();
                        }

                    }
                    else if (BulkRequest.ApplicationType == ApplicationType.EBS || BulkRequest.ApplicationType == ApplicationType.MSSL)
                    {
                        var data = BulkRequest.GetEntitlementDetail();
                        var adHocSAPHANARequest = (AdHocSAPHANARequest)data;

                        var para = new Dictionary<string, string>();
                        para.Add("position", adHocSAPHANARequest.Position);
                        para.Add("endpointSystemId", BulkRequest.EndpointSystemId.ToString());
                        var positionMatrix = DomainApiClient<Dto.SAPHANAXLPositionsMatrix>.GetByAction<Dto.SAPHANAXLPositionsMatrix>(DomainApiAction.Matrix.SAPHANA.GetSAPHANAPositionMatrixByPosition, para);
                        if (positionMatrix != null)
                        {
                            List<string> result = DomainApiClient<Dto.SAPHANAXLPositionsMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAPHANA.GetSAPHANARolesByPosition, new Dictionary<string, string> { { "position", positionMatrix.Position}, { "endpointSystemId", Convert.ToInt32(BulkRequest.EndpointSystemId).ToString() } });
                            positionRoles.AddRange(result);
                            ViewTechnicalSource.DataSource = positionRoles;
                            ViewTechnicalSource.DataBind();
                        }

                    }
                    if (positionRoles != null && positionRoles.Count > 0)
                    {
                        panelViewTechnicalSource.Visible = true;
                    }
                    else
                    {
                        panelViewTechnicalSource.Visible = false;
                    }

                    if (!ConvertLinkButtonToLabel)
                    {
                        var entitlememt = AccessNameLabel.Text;
                        var data = BulkRequest.GetEntitlementDetail();
                        if(BulkRequest.ApplicationType == ApplicationType.MSSL_CRM || BulkRequest.ApplicationType == ApplicationType.MSSL_ISU)
                        {
                            AdHocMSSLRequest adHocMSSLRequest = (AdHocMSSLRequest)data;
                            entitlememt = adHocMSSLRequest.Entitlement;
                        }
                        else if (BulkRequest.ApplicationType == ApplicationType.EBS_CRM || BulkRequest.ApplicationType == ApplicationType.EBS_ISU)
                        {
                            AdHocEBSRequest adHocEBSRequest = (AdHocEBSRequest)data;
                            entitlememt = adHocEBSRequest.Entitlement;
                        }
                        else if(BulkRequest.ApplicationType == ApplicationType.EBS || BulkRequest.ApplicationType == ApplicationType.MSSL)
                        {
                            var adHocSAPHANARequest = (AdHocSAPHANARequest)data;
                            entitlememt = adHocSAPHANARequest.Entitlement;
                        }
                        List<KeyValuePair<string, string>> entitlementDetail = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<List<KeyValuePair<string, string>>>("GetEntitlementDetailByEntitlement",
                             new Dictionary<string, string> { { "endpointSystemId", EndpointSystemId.ToString() }, { "entitlement", entitlememt } });
                        if (entitlementDetail != null && entitlementDetail.Count > 0)
                            pnlEntitlementRoles.Visible = true;
                        else
                            pnlEntitlementRoles.Visible = false;

                        grdEntitlements.DataSource = entitlementDetail;
                        grdEntitlements.DataBind();
                    }
                    

                    ViewTechnicalSourceWindow.VisibleOnPageLoad = true;
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void SAPHANASODConflictBusinessRoles(long dIUserId,string accountName, string UserName, long requestId, bool isBulkRequest, string applicationType)
        {
            List<PresentationConflictRemarks> AllRequest = DomainApiClient<Dto.SAPHANAUserinfo>.GetByAction<List<PresentationConflictRemarks>>(DomainApiAction.Endpoint.SAPHANA.SAPHANAUserinfo.GetSAPSODConflictsShowingrid,
                          new Dictionary<string, string> { { "dIUserId", dIUserId.ToString() }, { "requestId", requestId.ToString() }, { "isBulkRequest", "true" }, { "applicationType", applicationType } });
            if (AllRequest != null && AllRequest.Count > 0)
            {
                foreach (var item in AllRequest)
                {
                    item.UserWithAccountName = $" for the User - {UserName}({accountName})";
                    MSSLEBSConflictRemark.Add(item);
                }
                IsEBSMSSLConflictRemark = true;
            }
        }
    }

}