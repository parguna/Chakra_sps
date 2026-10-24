using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.NACR.Create.App.Step2UC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.Common
{
    public partial class AdhocAccessUC : UIBaseUC
    {
        #region Memebers
        private AdHocAccessRequest mAdHocAccessRequest;
        private const string V_SHOWACCOUNTTYPE = "V_SHOWACCOUNTTYPE";
        private AdHocAccessRequest AdhocRequest = new AdHocAccessRequest();
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        const string V_SUPERVISOR = "V_SUPERVISOR";
        const string V_EXISTING_AdHocAccessRequest = "V_EXISTING_AdHocAccessRequest";
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                WebControl.DisabledCssClass = null;
            }
        }

        #region Properties
        public AdHocAccessRequest AdHocAccessRequest
        {
            get
            {
                mAdHocAccessRequest = GetAdHocAccessRequest();
                return mAdHocAccessRequest;
            }
        }
        public bool ShowAccountType
        {
            get
            {
                if (ViewState[V_SHOWACCOUNTTYPE] == null)
                    return false;
                return (bool)ViewState[V_SHOWACCOUNTTYPE];
            }
            set
            {
                ViewState[V_SHOWACCOUNTTYPE] = value;
            }
        }
        public VPNType VPNType
        {
            get
            {
                if (ViewState["VPNTYPE"] == null)
                    return VPNType.VDI;
                return ((VPNType)ViewState["VPNTYPE"]) == VPNType.VDI ? VPNType.VDI : VPNType.SSLVPNOnly;
            }
            set
            {
                ViewState["VPNTYPE"] = value;
            }
        }
        private Dto.AdHocAccessRequest ExistingAdHocAccessRequest
        {
            get
            {
                if (ViewState[V_EXISTING_AdHocAccessRequest] != null)
                {
                    return (Dto.AdHocAccessRequest)ViewState[V_EXISTING_AdHocAccessRequest];
                }
                return null;
            }
            set
            {
                ViewState[V_EXISTING_AdHocAccessRequest] = value;
            }
        }

        Dto.User GetUser(long Id)
        {
            return DomainApiClient<Dto.User>.GetById(Id);
        }

        public void BindAdhocRequest(Dto.AdHocAccessRequest Request)
        {
            //ResubmitLockPanel.Enabled = false; 
            ExistingAdHocAccessRequest = Request;

            if (Request.IsSelfRequest)
            {
                UserSearchUC.SelectedUser = GetUser(Request.RequesteeUserId);
                SelectedUser_OnChanged(null);
                this.SelfRadioButton.Checked = true;
                this.OthersRadioButton.Checked = false;
            }
            else
            {
                this.OthersRadioButton.Checked = true;
                this.SelfRadioButton.Checked = false;
                UserSearchUC.SelectedUser = GetUser(Request.RequesteeUserId);
                SelectedUser_OnChanged(null);
            }

            if (Request.SelectedSupervisor != null)
            {
                if (SupervisorSearchUC.SelectedUser == null || SupervisorSearchUC.SelectedUser.Id != Request.SelectedSupervisor.Value)
                {
                    SupervisorSearchUC.SelectedUser = GetUser(Request.SelectedSupervisor.Value);
                }
            }
            ddlEndPointSystemType.SelectedValue = Convert.ToString(Convert.ToInt16(Request.EndpointSystemType));
            ddlEndPointSystemType_SelectedIndexChanged(null, null);

            ddlEndpoint.SelectedValue = Convert.ToString(Request.EndpointSystemId);
            ddlEndpoint_SelectedIndexChanged(null, null);

            ddlAccountType.SelectedValue = ((int)Request.AccountType).ToString();
            this.setAccessTypes();
            AddAccess.Checked = Request.AdhocActionType == AdHocActionType.NewAccount;
            AmendAccess.Checked = (Request.AdhocActionType == AdHocActionType.AmendmentAdd || Request.AdhocActionType == AdHocActionType.AmendmentRemove);
            DeleteAccess.Checked = Request.AdhocActionType == AdHocActionType.DeleteAccount;
            RequestTypeSelectionChanged(null, null);
            AmendAdd.Checked = Request.AdhocActionType == AdHocActionType.AmendmentAdd;
            AmendRemove.Checked = Request.AdhocActionType == AdHocActionType.AmendmentRemove;
            AmendmentTypeSelectionChanged(null, null);
            this.ValidFromPicker.SelectedDate = Request.ValidFrom;
            this.ValidToPicker.SelectedDate = Request.ValidTo;
            RequestReason.Text = Request.Comments;
            //this.VPNType = Request.VPNType;
            //ddlVPNType.SelectedValue = Convert.ToString(Request.VPNType);
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            if (Request.QueryString["AdhocRequestId"] != null)
            {
                return;
            }
            if (!IsPostBack)
            {
                RequestChange();
            }
        }

        bool isRemarksMandatory(AccountType type)
        {
            var remarksMandatoryForAccountTypes = new[] { AccountType.Secondary, AccountType.Service, AccountType.Shared };
            var entitlementDetailType = this.EntitlementDetailType;
            return remarksMandatoryForAccountTypes.Contains(type) || entitlementDetailType == ApplicationType.SAPLocal || entitlementDetailType == ApplicationType.MicrosoftTeams;
        }

        public bool Validate()
        {
            var request = this.AdHocAccessRequest;
            if (request.RequesteeUserId == 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please select the Requestee User");
                return false;
            }

            if (SupervisorSearchUC.SelectedUser == null)
            {
                ShowAlertMessage(AlertType.Warning, "User does not hold supervisor. Please select the supervisor");
                return false;
            }

            if (request.RequesteeUserId == SupervisorSearchUC.SelectedUser.Id)
            {
                ShowAlertMessage(AlertType.Warning, "Selected User and Supervisor should not be the same.");
                return false;
            }
            if ((int)request.EndpointSystemType <= 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please select the Application Type.");
                return false;
            }

            if (request.EndpointSystemId == null || request.EndpointSystemId.Value == 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please select the Application.");
                return false;
            }

            if (request.AccountType <= 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please select the Account Type.");
                return false;
            }

            if (!AddAccess.Checked && !AmendAccess.Checked && !DeleteAccess.Checked)
            {
                ShowAlertMessage(AlertType.Warning, "Please select the Action.");
                return false;
            }

            string tomessage = CheckValidTo();
            if (!string.IsNullOrWhiteSpace(tomessage))
            {
                ShowAlertMessage(AlertType.Warning, tomessage);
                return false;
            }

            string validateFromToDate = CheckValidToFromDate();
            if (!string.IsNullOrWhiteSpace(validateFromToDate))
            {
                ShowAlertMessage(AlertType.Warning, validateFromToDate);
                return false;
            }

            if (!CheckValidFrom())
            {
                ShowAlertMessage(AlertType.Warning, "Valid From cannot be empty.");
                return false;
            }

            if (ValidToPanel.Visible && !string.IsNullOrWhiteSpace(ValidToPanel.CssClass) && ValidToPanel.CssClass.ToUpper() == "form-group required".ToUpper()
                && (this.EndpointSystemType == EndpointType.SAP|| this.EndpointSystemType == EndpointType.SAPHANA) && AmendAccess.Checked && AmendAdd.Checked)
            {
                if (!ValidToPicker.SelectedDate.HasValue)
                {
                    ShowAlertMessage(AlertType.Warning, "Valid To cannot be empty.");
                    return false;
                }
            }

            if (isRemarksMandatory(request.AccountType) && string.IsNullOrWhiteSpace(RequestReason.Text))
            {
                SetMandatory(RequestReason, true);
                ShowAlertMessage(AlertType.Warning, "Please enter the comments");
                return false;
            }

            //if (EndpointSystemType != EndpointType.SkypeForBusiness)
            {
                var criteria = new Dto.Presentation.CanProceedSecondStepCriteria
                {
                    RequestTypeId = Convert.ToInt64(RequestType.Provisioning),
                    EndpointSystemId = this.SystemId,
                    UserType = UserSearchUC.SelectedUser.EmployeeType,
                    AccountType = AdhocAccountType,
                    ActionType = AdhocActionType,
                    RequestDate = DateTime.Today,
                    EndPointSystemType = EndpointSystemType,
                    ApplicationType = EntitlementDetailType,
                    UserId = RequestForUser().Id
                };

                var canProceed = DomainApiClient<Object>.GetByAction<BooleanStatus>("WorkflowController", DomainApiAction.Workflow.WorkflowConfiguration.CanProceedToSecondStep, null, criteria);
                if (!canProceed.CanProceed)
                {
                    if (!string.IsNullOrEmpty(canProceed.Message))
                        ShowAlertMessage(AlertType.Error, canProceed.Message);
                    return false;
                }
            }

            return true;
        }


        //bool validateActionType()
        //{
        //    var userID = RequestForUser().Id;
        //    var dict = new Dictionary<string, string>
        //    {
        //        { "userID", userID.ToString() }
        //    };

        //    if ((AddAccess.Checked && AdhocAccountType == AccountType.User))
        //    {

        //        dict.Add("endPointSystemID", this.SystemId.ToString());
        //        dict.Add("EndpointSystemType", Convert.ToString((int)EndpointSystemType));

        //        bool IsUserAccount = DomainApiClient<User>.GetByAction<bool>("IsUserAccount", dict);
        //        if (IsUserAccount)
        //        {
        //            ShowAlertMessage(AlertType.Error, "User cannot have more than one User account for this system.");
        //            return false;
        //        }
        //    }
        //    else if (DeleteAccess.Checked)
        //    {
        //        if (EndpointSystemType == EndpointType.VPN)
        //        {
        //            var adParam = new Dictionary<string, string>();
        //            adParam.Add("applicationType", ((int)ApplicationType.ADSingaporePowerLocal).ToString());
        //            var endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, adParam);
        //            dict.Add("endpointsystemid", endPoint.Id.ToString());
        //            dict.Add("accountType", Convert.ToString((int)AdhocAccountType));
        //            AccountReconDto list = DomainApiClient<ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);
        //            if (list == null || list.AccountReconDictionary.Count == 0)
        //            {
        //                ShowAlertMessage(AlertType.Error, "User does not hold an account. Hence cannot raise an Delete request.");
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            if (!checkIsAccountLocked()) return false;
        //        }
        //    }
        //    else if (AmendmentPanel.Visible && (AmendAdd.Checked || AmendRemove.Checked))
        //    {
        //        if (EndpointSystemType == EndpointType.VPN)
        //        {
        //            var adParam = new Dictionary<string, string>();
        //            adParam.Add("applicationType", ((int)ApplicationType.ADSingaporePowerLocal).ToString());
        //            var endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, adParam);
        //            dict.Add("endpointsystemid", endPoint.Id.ToString());
        //            dict.Add("accountType", Convert.ToString((int)AdhocAccountType));

        //            AccountReconDto listUserIds = DomainApiClient<ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);
        //            if (listUserIds != null && (listUserIds.AccountReconDictionary == null ||
        //                listUserIds.AccountReconDictionary.Count == 0))
        //            {
        //                ShowAlertMessage(AlertType.Error, "User does not hold account in AD. Hence cannot raise an Amendment request.");

        //                return false;
        //            }
        //        }
        //        else if (EndpointSystemType == EndpointType.SkypeForBusiness)
        //        {
        //            var adParam = new Dictionary<string, string>();
        //            adParam.Add("applicationType", ((int)ApplicationType.ADSingaporePowerLocal).ToString());
        //            var endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, adParam);
        //            dict.Add("endpointsystemid", endPoint.Id.ToString());
        //            dict.Add("accountType", Convert.ToString((int)AdhocAccountType));

        //            AccountReconDto listUserIds = DomainApiClient<ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);
        //            if (listUserIds != null && (listUserIds.AccountReconDictionary == null ||
        //                listUserIds.AccountReconDictionary.Count == 0))
        //            {
        //                ShowAlertMessage(AlertType.Error, "User does not hold account in AD. Hence cannot raise the request.");
        //                return false;
        //            }
        //            adParam = new Dictionary<string, string>();
        //            adParam.Add("applicationType", ((int)ApplicationType.Office365).ToString());
        //            endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, adParam);
        //            dict.Remove("endpointsystemid");
        //            dict.Add("endpointsystemid", endPoint.Id.ToString());

        //            listUserIds = DomainApiClient<ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);
        //            if (listUserIds != null && (listUserIds.AccountReconDictionary == null ||
        //                listUserIds.AccountReconDictionary.Count == 0))
        //            {
        //                ShowAlertMessage(AlertType.Error, "User does not hold account in Office365. Hence cannot raise a request.");
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            string outmsg = "";

        //            dict.Add("endPointSystemID", this.SystemId.ToString());
        //            dict.Add("adhocAccountType", Convert.ToString((int)AdhocAccountType));
        //            dict.Add("endpointSystemType", Convert.ToString((int)EndpointSystemType));
        //            AccountStatus status = DomainApiClient<ADSUserInfo>.GetByAction<AccountStatus>("IsAccountLocked", dict);

        //            if (status.IsLocked)
        //            {
        //                ShowAlertMessage(AlertType.Error, "Account is deactivated.  Cannot raise an Delete request.");
        //                return false;
        //            }
        //            else if (!string.IsNullOrEmpty(status.Message))
        //            {
        //                string str = DeleteAccess.Checked ? "a Delete" : "an Add";
        //                str = !AmendAccess.Checked ? str : "an Amendment";
        //                ShowAlertMessage(AlertType.Error, $"does not hold account. Hence cannot raise {str} request.");

        //                return false;
        //            }
        //        }
        //    }

        //    if (AdhocAccountType == AccountType.User)
        //    {
        //        BulkRequestParameter bulkRequestParameter = new BulkRequestParameter()
        //        {
        //            AccountType = AccountType.User,
        //            EndpointSystemId = this.EndpointSystem.Id,
        //            EndpointSystemTypeId = this.EndpointSystemType,
        //            keyValuePairs = new Dictionary<string, string>(),
        //        };

        //        if (AddAccess.Checked)
        //        {
        //            bulkRequestParameter.ActionType = Infrastructure.Enums.ActionType.NewAccount;
        //        }
        //        else if (DeleteAccess.Checked)
        //        {
        //            bulkRequestParameter.ActionType = Infrastructure.Enums.ActionType.DeleteAccount;
        //        }
        //        else if (AmendmentPanel.Visible)
        //        {
        //            if (AmendAdd.Checked)
        //            {
        //                bulkRequestParameter.ActionType = Infrastructure.Enums.ActionType.AmendmentAdd;
        //            }
        //            else if (AmendRemove.Checked)
        //            {
        //                bulkRequestParameter.ActionType = Infrastructure.Enums.ActionType.AmendmentRemove;
        //            }
        //        }

        //        ValidationUser returnValue = null;
        //        returnValue = BulkAdhocAccessRequestUserValidation.ValidateUsers(bulkRequestParameter, new List<User> { RequestForUser() });

        //        if (returnValue != null)
        //        {
        //            string msg = BulkAdhocAccessRequestUserValidation.GetErrormessage(returnValue.ErrorType);
        //            if (!string.IsNullOrEmpty(msg))
        //            {
        //                this.ShowAlertMessage(AlertType.Error, msg);
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        bool checkIsAccountLocked()
        {
            string outmsg = "";
            var userID = RequestForUser().Id;
            var dict = new Dictionary<string, string>
            {
                { "userID", userID.ToString() }
            };
            dict.Add("endPointSystemID", this.SystemId.ToString());
            dict.Add("adhocAccountType", Convert.ToString((int)AdhocAccountType));
            dict.Add("endpointSystemType", Convert.ToString((int)EndpointSystemType));
            AccountStatus status = DomainApiClient<ADSUserInfo>.GetByAction<AccountStatus>("IsAccountLocked", dict);

            if (status.IsLocked)
            {
                ShowAlertMessage(AlertType.Error, "Account is deactivated.  Cannot raise an Delete request.");
                return false;
            }
            else if (!string.IsNullOrEmpty(status.Message))
            {
                ShowAlertMessage(AlertType.Error, "does not hold account. Hence cannot raise an Delete request.");

                return false;
            }
            return true;
        }

        private User RequestForUser()
        {
            if (SelfRadioButton.Checked)
                return SessionUser.User;
            return UserSearchUC.SelectedUser;
        }

        private void RequestChange()
        {
            UserSearchUC.Clear();
            UserSearchUC.SelectedUser = RequestForUser();
            if (SelfRadioButton.Checked == true)
                UserSearchUC.Disabled = true;
            else
                UserSearchUC.Disabled = false;
            BindInfo();
            SetSupervisor();
            BindEndPointSystemType();
            BindEndPointSystem();
            EnableValidTo();
        }

        private void EnableValidTo()
        {
            ValidToPanel.Visible = false;
            ValidToPanel.CssClass = "form-group";
            ValidToPicker.MaxDate = DateTime.MaxValue;
            ValidToPicker.Clear();

            var user = UserSearchUC.SelectedUser;

            if (this.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (user != null && !user.FullTimeEmployee && user.TerminationDate.HasValue && (AdhocAccountType == AccountType.User || AdhocAccountType == AccountType.Secondary))
                {
                    ValidToPicker.SelectedDate = user.TerminationDate;
                }
                else
                {
                    ValidToPicker.SelectedDate = DateTime.MaxValue;
                }
            }
            else if ((this.EndpointSystemType == EndpointType.SAP || this.EndpointSystemType == EndpointType.EAM_NOVA|| this.EndpointSystemType == EndpointType.SAPHANA) && AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                ValidToPanel.Visible = true;
                ValidToPanel.CssClass = "form-group required";
             

                if (user != null && !user.FullTimeEmployee && user.TerminationDate.HasValue && AdhocAccountType != AccountType.Shared && AdhocAccountType != AccountType.Service)
                {
                    ValidToPicker.SelectedDate = user.TerminationDate;
                    ValidToPicker.MaxDate = user.TerminationDate.Value;
                }
                else
                {
                    ValidToPicker.SelectedDate = DateTime.MaxValue;
                    ValidToPicker.MaxDate = DateTime.MaxValue;
                    
                }
            }
        }

        protected void SelectedUser_OnChanged(object sender)
        {
            // RadAutoCompleteBox autoCompleteBox = (RadAutoCompleteBox)sender;
            var user = UserSearchUC.SelectedUser;
            UserInfoUC.User = user;
            UserInfoUC.Visible = true;
            SetSupervisor();
            BindEndPointSystemType();
            BindEndPointSystem();
            onAccessRequestValuesChanged("RequesteeUserId");

        }
        void SetSupervisor()
        {
            SupervisorSearchUC.Clear();
            var user = UserSearchUC.SelectedUser;
            if (user == null)
            {
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", user == null ? "0" : user.Id.ToString());
            var supervisor = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetROByUser, parameter);
            this.SupervisorSearchUC.SelectedUser = supervisor;
            ActualSupervisor = this.SupervisorSearchUC.SelectedUser;

        }

        void BindEndPointSystemType()
        {
            ddlEndPointSystemType.Items.Clear();
            ddlEndPointSystemType.Items.Insert(0, new ListItem("--Select--", "-1"));
            var user = UserSearchUC.SelectedUser;
            if (user == null)
            {
                return;
            }
            var endpointSystemTypeList = Enum.GetValues(typeof(EndpointType)).OfType<EndpointType>().OrderBy(x => x.GetDescription().ToString()).ToList();
            var removeSMS = true;

            //if (user.FullTimeEmployee && (bool)user.FullTimeEmployee && Request.QueryString["AdhocRequestId"] == null)
            //{
            //    removeSMS = false;
            //}

            //if (Request.QueryString["AdhocRequestId"] != null)
            //{
            //    removeSMS = false;
            //}

            //if (removeSMS)
            //{

            //    endpointSystemTypeList.Remove(EndpointType.OneCard);

            //}

            endpointSystemTypeList.Remove(EndpointType.None);
            endpointSystemTypeList.Remove(EndpointType.SuccessFactor);
            endpointSystemTypeList.Remove(EndpointType.GEMS);
            foreach (EndpointType item in endpointSystemTypeList)
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                //if (item == EndpointSystemType.Miscellaneous && turnOffLotusNotes)
                //{
                //    description = description.Replace("External Email / Internet Access", "Internet Access");
                //}
                var listItem = new ListItem(description, ((int)item).ToString());
                ddlEndPointSystemType.Items.Add(listItem);
            }
        }
        private void BindInfo()
        {
            ValidFromPicker.SelectedDate = DateTime.Now;
            if (UserSearchUC.SelectedUser == null)
            {

                UserInfoUC.Visible = false;

            }
            else
            {
                UserInfoUC.User = UserSearchUC.SelectedUser;
                UserInfoUC.Visible = true;
                if (UserSearchUC.SelectedUser.TerminationDate.HasValue)
                    ValidFromPicker.MaxDate = UserSearchUC.SelectedUser.TerminationDate.Value;
            }
            ValidFromPicker.MinDate = DateTime.Now;

            ValidFromPicker.MinDate = DateTime.Now;
            ValidToPicker.MinDate = (DateTime)ValidFromPicker.SelectedDate;
        }

        public string CheckValidTo()
        {
            var strMsg = string.Empty;
            if (
                //this.AdhocActionType == AdHocActionType.AmendmentAdd || 
                this.AdhocActionType == AdHocActionType.NewAccount)
            {
                bool to = ValidToPicker.SelectedDate.HasValue;
                if (!to && !IsUserFullTimeEmployee() && ValidToPanel.Visible)
                    strMsg = "Valid To should not be empty for Non-Employee";
            }
            return strMsg;
        }

        public bool CheckValidFrom()
        {
            return ValidFromPicker.SelectedDate.HasValue;
        }

        public string CheckValidToFromDate()
        {
            if (ValidToPicker.SelectedDate.HasValue && ValidFromPicker.SelectedDate.HasValue)
            {
                var fromDate = ValidFromPicker.SelectedDate;
                var toDate = ValidToPicker.SelectedDate;

                if (toDate < DateTime.Today || fromDate > toDate)
                {
                    return "Valid To should not be earlier than Valid From";
                }
            }
            return string.Empty;

        }

        //private DateTime GetValidTo(DateTime? selectedDate, DateTime? terminationDate, AccountType AdhocAccountType)
        //{
        //    var validTo = DateTime.MaxValue;
        //    if ((AdhocAccountType == AccountType.Service || AdhocAccountType == AccountType.Shared) && !selectedDate.HasValue) return validTo;

        //    if (selectedDate != null && selectedDate.HasValue)
        //        return selectedDate.Value;
        //    else
        //    {
        //        if (terminationDate.HasValue)
        //        {
        //            return terminationDate.Value;
        //        }
        //    }
        //    return validTo;
        //}

        public AdHocAccessRequest GetAdHocAccessRequest()
        {
            //var validTo = DateTime.MaxValue;

            AdhocRequest.RequesterUserId = SessionUser.User.Id;
            AdhocRequest.IsAddAccess = AddAccess.Checked;
            AdhocRequest.Comments = RequestReason.Text;
            AdhocRequest.ValidFrom = ValidFromPicker.SelectedDate;
            AdhocRequest.CreatedDate = DateTime.Now;
            AdhocRequest.LastModifiedDate = DateTime.Now;

            var user = UserSearchUC.SelectedUser;

            if (user != null && !user.FullTimeEmployee && user.TerminationDate.HasValue && user.TerminationDate.Value >= ValidToPicker.MinDate && ValidToPicker.SelectedDate == user.TerminationDate.Value)
            {
                ValidToPicker.SelectedDate = user.TerminationDate.Value;
            }
            AdhocRequest.ValidTo = ValidToPicker.SelectedDate;

            //AdhocRequest.ValidTo = GetValidTo(ValidToPicker.SelectedDate, user.TerminationDate, AdhocAccountType);

            AdhocRequest.AccountType = AdhocAccountType;

            if (SelfRadioButton.Checked)
            {
                AdhocRequest.IsSelfRequest = SelfRadioButton.Checked;
                AdhocRequest.RequesteeUserId = SessionUser.UserId;
            }
            else
            {
                AdhocRequest.RequesteeUserId = UserSearchUC.SelectedUser != null ? UserSearchUC.SelectedUser.Id : 0;
            }

            AdhocRequest.EndpointSystemType = EndpointSystemType;
            AdhocRequest.EndpointSystemId = EndpointSystem.Id;
            AdhocRequest.EndPoint = EndpointSystem;
            AdhocRequest.ApplicationType = EntitlementDetailType;
            AdhocRequest.ActualSupervisor = ActualSupervisor == null ? null : (int?)ActualSupervisor.Id;
            AdhocRequest.SelectedSupervisor = this.SupervisorSearchUC.SelectedUser == null ? null : (int?)this.SupervisorSearchUC.SelectedUser.Id;
            AdhocRequest.AdhocActionType = AdhocActionType;
            if (ExistingAdHocAccessRequest != null)
            {
                // AdhocRequest.Id = ExistingAdHocAccessRequest.Id;
                if (!string.IsNullOrEmpty(ExistingAdHocAccessRequest.EntitlementDetail))
                    AdhocRequest.EntitlementDetail = ExistingAdHocAccessRequest.EntitlementDetail;

                if (!string.IsNullOrEmpty(ExistingAdHocAccessRequest.SecurityQuestions))
                    AdhocRequest.SecurityQuestions = ExistingAdHocAccessRequest.SecurityQuestions;
            }
            return AdhocRequest;
        }
        #endregion

        #region Events
        protected void AccessRequestFor(object sender, EventArgs e)
        {
            RequestChange();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (UserSearchUC.IsEntryModified)
            {
                BindInfo();
            }
        }
        #endregion

        public User ActualSupervisor
        {
            get
            {
                if (ViewState[V_SUPERVISOR] != null)
                {
                    return (User)ViewState[V_SUPERVISOR];
                }
                return null;
            }
            set
            {
                ViewState[V_SUPERVISOR] = value;
            }
        }
        protected void AmendmentTypeSelectionChanged(object sender, EventArgs e)
        {
        }

        protected void AddAccess_CheckedChanged(object sender, EventArgs e)
        {
            Session["ActionType"] = ((RadioButton)sender).ID;
            var actiontype = ((RadioButton)sender).ID;
            SetMandatory(ValidToPanel, actiontype == "AddAccess" || actiontype == "AmendAccess");
            //if (actiontype == "AddAccess" || actiontype == "AmendAccess")
            //{
            //    RequireClass.CssClass = "form-group required";
            //}
            //else
            //{
            //    RequireClass.CssClass = RequireClass.CssClass.Replace("required", "");
            //}
            Session["isRequestTypeChange"] = true;
        }

        protected void SelectedSupervisor_OnChanged(object sender)
        {
            //RadAutoCompleteBox autoCompleteBox = (RadAutoCompleteBox)sender;
            //UserInfoUC.User = UserSearchUC.SelectedUser;
            //UserInfoUC.Visible = true;
            onAccessRequestValuesChanged("SelectedSupervisor");
        }

        bool checkIfEndpointSystemUserByBirth(ApplicationType type)
        {

            var endpointCollection = new List<ApplicationType> { ApplicationType.ADSingaporePowerLocal, ApplicationType.Office365, ApplicationType.HRMS_FLINS, ApplicationType.OneCard, ApplicationType.SAP_EP, ApplicationType.CE, ApplicationType.MicrosoftTeams };
            //if (type == ApplicationType.SkypeForBusiness)
            //{
            //    var userData = DomainApiClient<Dto.User>.GetById(this.AdHocAccessRequest.RequesteeUserId);
            //    if(userData.EmployeeType == EmployeeType.Permanent)
            //    {
            //        endpointCollection.Add(ApplicationType.SkypeForBusiness);
            //    }
            //}
            return endpointCollection.Contains(type);
        }

        bool canSharedAccountEnabled(ApplicationType type)
        {
            if (OthersRadioButton.Checked) return false;
            var endpointCollection = new[] { ApplicationType.NPAC, ApplicationType.VPN, ApplicationType.OneCard, ApplicationType.SSIDS_GUI, ApplicationType.SSIDS_SMS, ApplicationType.IDMS, ApplicationType.MicrosoftTeams };
            return !endpointCollection.Contains(type);
        }

        bool canSecondaryAccountEnabled(ApplicationType type)
        {
            //if (OthersRadioButton.Checked) return false; 

            var endpointCollection = new[] { ApplicationType.NPAC, ApplicationType.VPN, ApplicationType.OneCard, ApplicationType.Office365, ApplicationType.SSIDS_GUI, ApplicationType.SSIDS_SMS, ApplicationType.MicrosoftTeams, ApplicationType.IDMS };

            if (endpointCollection.Contains(type)) return false;

           
            var deptCode = string.IsNullOrWhiteSpace(SessionUser.User.DepartmentCode) ? string.Empty : SessionUser.User.DepartmentCode.Trim(); 

            List<string> SecondaryAccountEnabledDepartments = MasterConfigHelper.SecondaryAccountEnabledDepartments;

            return SecondaryAccountEnabledDepartments!=null && SecondaryAccountEnabledDepartments.Any(s => 
            {
                var dept = s.Replace("*", string.Empty).Trim();

                return deptCode == s.Trim() || deptCode.StartsWith(dept);
                } ); 
        }

        bool canServiceAccountEnabled(ApplicationType type)
        {
            List<string> IT_departmentCode = MasterConfigHelper.ITDDKey;
            if (OthersRadioButton.Checked || !IT_departmentCode.Contains(SessionUser.User.DepartmentCode)) return false;
            var endpointCollection = new[] { ApplicationType.NPAC, ApplicationType.VPN, ApplicationType.OneCard, ApplicationType.Office365, ApplicationType.SSIDS_GUI, ApplicationType.SSIDS_SMS, ApplicationType.MicrosoftTeams, ApplicationType.IDMS };
            return !endpointCollection.Contains(type);
        }

        void validateBusinessLogicOnEndPointSystemChange()
        {
            lblLicenseMsg.Text = "";
            bool isUserFullTimeEmployee = IsUserFullTimeEmployee();
            //if (EndpointSystem.Id == 0) return;

            setRadioButtonState(AddAccess, true, true);
            setRadioButtonState(AmendAccess, true);
            setRadioButtonState(DeleteAccess, true);
            //setRadioButtonState(ReNewAccess, false);

            ApplicationType applicationType = EntitlementDetailType;

            ClearAccountTypes();
            new List<AccountType> { AccountType.User, AccountType.Shared, AccountType.Service, AccountType.Secondary }.ForEach(
               accountType =>
                    {

                        if (accountType == AccountType.Service)
                        {
                            setAccountTypeState(accountType, canServiceAccountEnabled(applicationType));
                        }
                        else if (accountType == AccountType.Shared)
                        {
                            setAccountTypeState(accountType, canSharedAccountEnabled(applicationType));
                        }
                        else if (accountType == AccountType.Secondary)
                        {
                            if (SelectedEndpoint != null && SelectedEndpoint.SupportSecondaryAccount)
                            {
                                setAccountTypeState(accountType, canSecondaryAccountEnabled(applicationType));
                            }
                            else
                            {
                                setAccountTypeState(accountType, false);
                            }
                        }
                        else
                        {
                            setAccountTypeState(accountType, true, accountType == AccountType.User);
                        }

                    });


            if (OthersRadioButton.Checked && !IsUserFullTimeEmployee() && applicationType == ApplicationType.Office365)
            {
                lblLicenseMsg.Text = MasterConfigHelper.AdhocLicenseMsg;
            }
        }

        #region AccountType


        void setAccountTypeState(AccountType applicationType, bool IsEnabled, bool IsSelect = false)
        {
            var listItem = ddlAccountType.Items.Cast<ListItem>().Where(lstItem => lstItem.Value == ((int)applicationType).ToString()).Select(lstItem => lstItem).FirstOrDefault();

            if (IsEnabled)
            {
                if (listItem == null)
                {
                    listItem = new ListItem(UtilityMethods.GetEnumAttribute<Description>(applicationType), Convert.ToString((int)applicationType));
                    ddlAccountType.Items.Add(listItem);
                }
                if (listItem.Attributes.Keys.OfType<string>().Any(attr => attr == "disabled"))
                    listItem.Attributes.Remove("disabled");

                if (IsSelect)
                {
                    ddlAccountType.SelectedValue = listItem.Value;

                }
                else
                {
                    ddlAccountType.SelectedValue = "0";
                }

            }
            //else
            //{
            //    if (ddlAccountType.SelectedValue == listItem.Value) ddlAccountType.SelectedIndex = -1;

            //    listItem.Attributes.Add("disabled", "disabled");
            //    listItem.Enabled = false;
            //}
        }

        void ClearAccountTypes()
        {
            resetAccessRights();
            if (ddlAccountType.Items.Count == 1) return;
            ddlAccountType.Items.Clear();

            Array itemValues = System.Enum.GetValues(typeof(AccountType));
            for (int i = 0; i <= 0; i++)
            {
                var description = (AccountType)itemValues.GetValue(i) == AccountType.None ? "--Select--" : UtilityMethods.GetEnumAttribute<Description>((AccountType)itemValues.GetValue(i));
                var value = (int)((AccountType)itemValues.GetValue(i));
                ListItem item = new ListItem(description, value.ToString());
                ddlAccountType.Items.Add(item);
            }
        }

        #endregion AccountType

        #region AccessRights

        bool IsUserFullTimeEmployee()
        {
            var usr = UserSearchUC.SelectedUser;
            return usr != null && usr.FullTimeEmployee;
        }

        void setRadioButtonState(RadioButton btn, bool state, bool isChecked = false)
        {
            if (btn == null) return;
            btn.Enabled = state;
            if (!state)
            {
                btn.Checked = state;
                return;
            }
            btn.Checked = isChecked;
        }

        void resetAccessRights()
        {
            new List<RadioButton> { AddAccess, DeleteAccess, AmendAccess, AmendAdd, AmendRemove }
            .ForEach(btn => setRadioButtonState(btn, true)
            );
        }
        void setAccessTypes()
        {
            ApplicationType applicationType = EntitlementDetailType;
            AccountType accountType = AdhocAccountType;

            new List<RadioButton> { AddAccess, DeleteAccess, AmendAccess, AmendAdd, AmendRemove }.ForEach(btn =>
                  {
                      setRadioButtonState(btn, false);
                  });

            switch (accountType)
            {
                case AccountType.User:

                    bool canAmendChecked = true;
                    if (checkIfEndpointSystemUserByBirth(applicationType))
                    {
                        //if (SelfRadioButton.Checked)
                        //{
                        //    if (!IsUserFullTimeEmployee())
                        //    {
                        //        setRadioButtonState(AddAccess, IsAddAccessApplicable(), true);
                        //        setRadioButtonState(DeleteAccess, true, false);
                        //        canAmendChecked = !IsAddAccessApplicable();
                        //    }
                        //}
                        //if (OthersRadioButton.Checked)
                        //{
                        if (!IsUserFullTimeEmployee())
                        {
                            setRadioButtonState(AddAccess, IsAddAccessApplicable(), true);
                            setRadioButtonState(DeleteAccess, IsDeleteAccessApplicable());
                            canAmendChecked = !IsAddAccessApplicable();
                        }
                        //}
                    }
                    else
                    {
                        setRadioButtonState(AmendAccess, true);
                        setRadioButtonState(AddAccess, IsAddAccessApplicable(), true);
                        setRadioButtonState(DeleteAccess, IsDeleteAccessApplicable());
                        canAmendChecked = !IsAddAccessApplicable();
                    }

                    setRadioButtonState(AmendAccess, true, canAmendChecked);
                    break;
                case AccountType.Shared:
                    setRadioButtonState(AddAccess, IsAddAccessApplicable(), true);
                    setRadioButtonState(DeleteAccess, IsDeleteAccessApplicable());
                    setRadioButtonState(AmendAccess, true, !IsAddAccessApplicable());
                    break;
                case AccountType.Service:

                    setRadioButtonState(AddAccess, IsAddAccessApplicable(), true);
                    setRadioButtonState(DeleteAccess, IsDeleteAccessApplicable());
                    setRadioButtonState(AmendAccess, true, !IsAddAccessApplicable());
                    break;
                case AccountType.Secondary:

                    setRadioButtonState(AddAccess, IsAddAccessApplicable(), true);
                    setRadioButtonState(DeleteAccess, IsDeleteAccessApplicable());
                    setRadioButtonState(AmendAccess, true, !IsAddAccessApplicable());
                    break;

            }

            RequestTypeSelectionChanged(null, null);
        }

        bool IsAddAccessApplicable()
        {
            if (EntitlementDetailType == ApplicationType.SAP_EP || EntitlementDetailType == ApplicationType.CE || EntitlementDetailType == ApplicationType.IDMS) { return false; }
            if (MasterConfigHelper.SuccessFactorEndpointSystemIDs.Contains(this.SystemId) && this.IsUserFullTimeEmployee()) { return false; }
            return true;
        }

        bool IsDeleteAccessApplicable()
        {
            var notAllowedList = new List<ApplicationType> { ApplicationType.SAP_EP, ApplicationType.CE, ApplicationType.IDMS };
            if (this.EntitlementDetailType == ApplicationType.MicrosoftTeams)
            {
                var userData = DomainApiClient<Dto.User>.GetById(this.AdHocAccessRequest.RequesteeUserId);
                if (userData.EmployeeType == EmployeeType.Permanent)
                {
                    notAllowedList.Add(ApplicationType.MicrosoftTeams);
                }
            }
            if (MasterConfigHelper.SuccessFactorEndpointSystemIDs.Contains(this.SystemId) && this.IsUserFullTimeEmployee()) { return false; }
            if (notAllowedList.Contains(EntitlementDetailType)) { return false; }
            return true;

        }

        #endregion AccessRights
        protected void ddlEndPointSystemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnFAQ.Visible = EndpointSystemType == EndpointType.OneCard;
            if (ddlEndPointSystemType.SelectedIndex > 0)
            {
                ClearAccountTypes();
                BindEndPointSystem();

                //{
                //    if(this.EndpointSystemType == EndpointType.SkypeForBusiness)
                //    {
                //        this.RequestReasonDiv.Attributes.Add("class", "form-group required");
                //    }else
                //    {
                //        this.RequestReasonDiv.Attributes.Add("class", "form-group");
                //    }
                //    //divVPNType.Visible = false;
                //}
                //this.RequestTypeSelectionChanged(sender, e);
                //VisibleValidTo();

                //if (this.EndpointSystemType == EndpointSystemType.InhouseSystems)
                //{
                //    //    lblInfoTR.Visible = true;
                //    //    lblInfo.Text = MasterConfigHelper.OtherSystemSelectionMessage;
                //    //}
                //    //else
                //    //{
                //    //    lblInfoTR.Visible = false;
                //}
                onAccessRequestValuesChanged("EndpointSystemType");

            }

        }

        private void BindVPNType()
        {
            //divVPNType.Visible = true;
            //ddlVPNType.Items.Clear();
            ////Only Citrix
            //if (this.UserSearchUC.SelectedUser.EmployeeType == EmployeeType.Contractor || this.UserSearchUC.SelectedUser.EmployeeType == EmployeeType.Vendor)
            //{
            //    ddlVPNType.Items.Add(new ListItem(Enum.GetName(typeof(VPNType), VPNType.CITRIX), ((int)VPNType.CITRIX).ToString()));
            //    ddlVPNType.SelectedValue = ((int)VPNType.CITRIX).ToString();
            //    this.VPNType = VPNType.CITRIX;
            //}
            //else
            //{
            //    ddlVPNType.Items.Add(new ListItem(Enum.GetName(typeof(VPNType), VPNType.CITRIX), ((int)VPNType.CITRIX).ToString()));
            //    ddlVPNType.Items.Add(new ListItem(Enum.GetName(typeof(VPNType), VPNType.SSLVPNOnly), ((int)VPNType.SSLVPNOnly).ToString()));
            //    ddlVPNType.SelectedValue = ((int)VPNType.SSLVPNOnly).ToString();
            //    this.VPNType = VPNType.SSLVPNOnly;
            //}
        }

        bool canAmendRemoveEnable(ApplicationType applicationType)
        {
            var amendRemoveNotApplicableApplicationTypes = new[] { ApplicationType.NPAC, ApplicationType.SSIDS_GUI, ApplicationType.SSIDS_SMS };
            return !amendRemoveNotApplicableApplicationTypes.Contains(applicationType);
        }

        bool canAmendAddEnable(ApplicationType applicationType)
        {
            return true;
        }

        bool canAmendAddAutoCheck(ApplicationType applicationType)
        {
            return true;
        }
        protected void RequestTypeSelectionChanged(object sender, EventArgs e)
        {
            lblLicenseMsg.Text = "";
            if (AmendAccess.Checked == true)
            {
                AmendmentPanel.Visible = true;
                setRadioButtonState(AmendRemove, canAmendRemoveEnable(this.EntitlementDetailType));
                setRadioButtonState(AmendAdd, canAmendAddEnable(this.EntitlementDetailType), canAmendAddAutoCheck(this.EntitlementDetailType));
            }
            else
            {
                AmendAdd.Checked = false;
                AmendRemove.Checked = false;
                AmendmentPanel.Visible = false;
            }
            if (AddAccess.Checked && this.ddlEndpoint.SelectedIndex > 0 && EntitlementDetailType == ApplicationType.Office365)
            {
                lblLicenseMsg.Text = MasterConfigHelper.AdhocLicenseMsg;
            }



            //    AddAmndRadioButton.Visible = false;
            //    DeleteAmndRadioButton.Visible = false;
            //    lblEffDateValidation.Text = string.Empty;
            //    DatePickerValidFrom.MinDate = DateTime.Now;
            //}
            //if (DelAccRadioButton.Checked == true)
            //{
            //    lblLicenseMsg.Text = "";
            //    AmendmentPanel.Visible = false;
            //    AddAmndRadioButton.Visible = false;
            //    DeleteAmndRadioButton.Visible = false;
            //    lblEffDateValidation.Text = string.Empty;
            //}
            //else if (AmendmentRadioButton.Checked == true)
            //{
            //    lblLicenseMsg.Text = "";
            //    AmendmentPanel.Visible = true;
            //    AddAmndRadioButton.Visible = true;
            //    DeleteAmndRadioButton.Visible = true;
            //    lblEffDateValidation.Text = string.Empty;
            //}
            //VisibleValidTo();
            EnableValidTo();

            onAccessRequestValuesChanged("AdhocActionType");

        }


        private void BindEndPointSystem()
        {
            ddlEndpoint.Items.Clear();
            ClearAccountTypes();

            try
            {
                if (ddlEndPointSystemType.SelectedIndex > 0)
                {
                    int EndPointSystemTypeId = Convert.ToInt32(ddlEndPointSystemType.SelectedValue);

                    ddlEndpoint.Items.Clear();
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", EndPointSystemTypeId.ToString());
                    var endPoints = DomainApiClient<Dto.EndPoint>.GetByAction<List<Dto.EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetByEndpointSystemTypeId, parameter);
                    List<EndPoint> list = endPoints.Where(e => !e.IsTrustedSource).OrderBy(i => i.DisplayName).ToList();
                    if (this.EndpointSystemType == EndpointType.SAP)
                    {

                        list.RemoveAll(item => item.ApplicationType == ApplicationType.SAP_SRP);
                        list.RemoveAll(item => item.ApplicationType == ApplicationType.SAP_SUS);
                        DropDownHelper.DropDownListBind(ddlEndpoint, list, "DisplayName", "Id", null, true, false);
                    }
                    //else if (this.EndpointSystemType == EndpointType.SRM)
                    //{
                    //    ddlEndpoint.Items.Clear();
                    //    ddlEndpoint.Items.Add(new ListItem("--Select--", "0"));
                    //    ddlEndpoint.Items.Add(new ListItem() { Text = "SRP", Value = "SRP" });
                    //}
                    else
                    {
                        //list.Add(new EndPoint() { Id = 0, DisplayName = "--Select--" });
                        DropDownHelper.DropDownListBind(ddlEndpoint, list, "DisplayName", "Id", null, true, false);
                    }
                }
                else
                {
                    ddlEndpoint.Items.Clear();
                    ddlEndpoint.Items.Add(new ListItem("--Select--", "0"));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Values
        public EndpointType EndpointSystemType
        {
            get
            {
                return (EndpointType)Convert.ToInt16(string.IsNullOrEmpty(ddlEndPointSystemType.SelectedValue) ? "0" : ddlEndPointSystemType.SelectedValue);
            }
        }

        public EndPoint EndpointSystem
        {
            get
            {
                var id = Convert.ToInt32(string.IsNullOrEmpty(ddlEndpoint.SelectedValue) || ddlEndpoint.SelectedValue.Equals("--Select--") ? "0" : ddlEndpoint.SelectedValue);
                var endpoint = new EndPoint() { Id = id, DisplayName = ddlEndpoint.SelectedItem != null ? ddlEndpoint.SelectedItem.Text : "" };

                return endpoint;
            }
        }

        public AdHocActionType AdhocActionType
        {
            get
            {
                if (AddAccess.Checked)
                {
                    return AdHocActionType.NewAccount;
                }
                else if (DeleteAccess.Checked)
                {
                    return AdHocActionType.DeleteAccount;
                }
                else if (AmendAdd.Checked)
                {
                    return AdHocActionType.AmendmentAdd;
                }
                else if (AmendRemove.Checked)
                {
                    return AdHocActionType.AmendmentRemove;
                }
                else
                {
                    return AdHocActionType.None;
                }
            }
        }
        public ApplicationType EntitlementDetailType
        {
            get
            {
                if (this.EndpointSystemType == EndpointType.InhouseSystems)
                {
                    return ApplicationType.InhouseSystems;
                }
                //else if (this.EndpointSystemType == EndpointType.Miscellaneous)
                //{
                //    //return ApplicationType;
                //}
                else
                {
                    Dto.EndPoint endpoint = SelectedEndpoint;
                    if (endpoint != null)
                        return endpoint.ApplicationType;
                    else
                        return ApplicationType.None;
                }
            }
        }
        protected Dto.EndPoint SelectedEndpoint
        {
            get
            {
                if (ViewState["SelectedEndpoint"] != null)
                {
                    var endpoint = (Dto.EndPoint)ViewState["SelectedEndpoint"];
                    if (endpoint != null && endpoint.Id == this.SystemId)
                    {
                        return endpoint;
                    }

                }
                ViewState["SelectedEndpoint"] = GetSelectedEndpoint();
                return (EndPoint)ViewState["SelectedEndpoint"];
            }
            set
            {
                ViewState["SelectedEndpoint"] = value;
            }
        }


        EndPoint GetSelectedEndpoint()
        {
            if (this.SystemId == 0)
                return null;
            return DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(DomainApiAction.Endpoint.EndPoint.GetByIdIncludingEndPointSystemType,
                new Dictionary<string, string>() { { "id", this.SystemId.ToString() } });
        }

        public AccountType AdhocAccountType
        {
            get
            {

                return (AccountType)Convert.ToInt32(string.IsNullOrEmpty(ddlAccountType.SelectedValue) ? "0" : ddlAccountType.SelectedValue);

            }
        }
        public int SystemId
        {
            get
            {
                if (Convert.ToInt32(this.EndpointSystemType) > -1)
                    return Convert.ToInt16(string.IsNullOrEmpty(ddlEndpoint.SelectedValue) || ddlEndpoint.SelectedValue.Equals("--Select--") ? 0 : Convert.ToInt32(ddlEndpoint.SelectedValue));
                return 0;
            }
        }

        #endregion Values

        #region Events

        protected void ddlAccountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearValidTo();
            if (ddlAccountType.SelectedIndex > 0)
            {
                setAccessTypes();
                onAccessRequestValuesChanged("AccountType");


                AccountType type;

                if (ddlAccountType.SelectedItem.Text == "Shared Account")
                    type = AccountType.Shared;

                else if (ddlAccountType.SelectedItem.Text == "Service Account")
                    type = AccountType.Service;
                else if (ddlAccountType.SelectedItem.Text == "Secondary Account")
                    type = AccountType.Secondary;
                else
                    type = AccountType.User;
                if (isRemarksMandatory(type))
                {
                    SetMandatory(RequestReasonDiv, true);
                }
                EnableValidTo();
            }
        }
        protected void ddlEndpoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearValidTo();
            if (ddlEndpoint.SelectedIndex > 0)
            {
                validateBusinessLogicOnEndPointSystemChange();
                resetAccessRights();
            }

            if (ddlAccountType.Items.Count == 2 && ddlAccountType.SelectedValue == Convert.ToString((int)AccountType.User))
            {
                ddlAccountType_SelectedIndexChanged(null, null);
            }
            onAccessRequestValuesChanged("EndPoint");
        }
        private void ClearValidTo()
        {
            this.ValidToPicker.Clear();
            this.AdHocAccessRequest.ValidTo = null;
            SetMandatory(RequestReasonDiv, false);
        }
        void onAccessRequestValuesChanged(string ValueName)
        {
            Session["AccessRequestChanged"] = true;
            if (Session["CHANGEDPROPERTY"] == null)
            {
                Session["CHANGEDPROPERTY"] = new List<string> { ValueName };
                return;
            }
            List<string> valueNames = (List<string>)Session["CHANGEDPROPERTY"];
            if (!valueNames.Contains(ValueName))
            {
                valueNames.Add(ValueName);
                Session["CHANGEDPROPERTY"] = valueNames;
            }
        }


        #endregion Events

        protected void AmendRemove_CheckedChanged(object sender, EventArgs e)
        {
            EnableValidTo();
            onAccessRequestValuesChanged("AdhocActionType");
        }

        protected void AmendAdd_CheckedChanged(object sender, EventArgs e)
        {
            EnableValidTo();
            onAccessRequestValuesChanged("AdhocActionType");
        }

        protected void ValidFromPicker_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
        {
            var user = UserSearchUC.SelectedUser;

            if (this.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (user != null && !user.FullTimeEmployee && user.TerminationDate.HasValue && (AdhocAccountType == AccountType.User || AdhocAccountType == AccountType.Secondary))
                {
                    ValidToPicker.SelectedDate = user.TerminationDate;
                }
                else
                {
                    ValidToPicker.SelectedDate = DateTime.MaxValue;
                }
            }
            else if ((this.EndpointSystemType == EndpointType.SAP || this.EndpointSystemType == EndpointType.SAPHANA || this.EndpointSystemType == EndpointType.EAM_NOVA) && AdhocActionType == AdHocActionType.AmendmentAdd)
            {

                if (user != null && !user.FullTimeEmployee && user.TerminationDate.HasValue && AdhocAccountType != AccountType.Shared && AdhocAccountType != AccountType.Service)
                {
                    ValidToPicker.SelectedDate = user.TerminationDate;
                    ValidToPicker.MaxDate = user.TerminationDate.Value;
                }
                else
                {
                    ValidToPicker.MaxDate = DateTime.MaxValue;
                    ValidToPicker.SelectedDate = DateTime.MaxValue;
                }
            }
        }
    }
}