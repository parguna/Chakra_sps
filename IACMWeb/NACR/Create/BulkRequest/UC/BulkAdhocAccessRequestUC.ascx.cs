using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Web.NACR.Create.App.Step2UC;
using Telerik.Web.UI;
using DI.Chakra.Web.NACR.Create.Validation;

namespace DI.Chakra.Web.NACR.Create.BulkRequest
{

    public partial class BulkAdhocAccessRequestUC : UIBaseUC
    {
        Dto.BulkRequest mAdHocAccessRequest;
        public delegate void SaveHandler(Dto.BulkRequest adhocrequest);
        public event SaveHandler SaveClicked;
        private static int _UserId = -1;
        public delegate void BackHandler();
        public event BackHandler BackClicked;
        private const string V_REQUEST = "V_REQUEST";
        private const string V_SUPERVISOR = "V_SUPERVISOR";
        private const string V_BulkRequestId = "V_BulkRequestId";

        public bool IsFulltimeEmployee
        {
            get
            {
                if (string.IsNullOrWhiteSpace(hiddenIsFullTimeEmployee.Value))
                {
                    return true;
                }
                return Convert.ToBoolean(hiddenIsFullTimeEmployee.Value);
            }
            set
            {
                hiddenIsFullTimeEmployee.Value = value.ToString();
            }
        }

        public string RosEmpid
        {
            get
            {
                return hiddenRosEmpid.Value;
            }
            set
            {
                hiddenRosEmpid.Value = value.ToString();
            }
        }

        public Dto.BulkRequest BulkAdHocAccessRequest
        {
            get
            {
                mAdHocAccessRequest = this.GetBulkRequest();
                return mAdHocAccessRequest;
            }
        }

        private Dto.BulkRequest ExistingBulkRequest
        {
            get
            {
                if (ViewState[V_BulkRequestId] != null)
                {
                    return (Dto.BulkRequest)ViewState[V_BulkRequestId];
                }
                return null;
            }
            set
            {
                ViewState[V_BulkRequestId] = value;
            }
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

        public AdHocActionType AdhocActionType
        {
            get
            {
                if (NewAccRadioButton.Checked)
                {
                    return AdHocActionType.NewAccount;
                }
                else if (DelAccRadioButton.Checked)
                {
                    return AdHocActionType.DeleteAccount;
                }
                else if (AddAmndRadioButton.Checked)
                {
                    return AdHocActionType.AmendmentAdd;
                }
                else if (DeleteAmndRadioButton.Checked)
                {
                    return AdHocActionType.AmendmentRemove;
                }
                else
                {
                    return AdHocActionType.None;
                }
            }
        }

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

        public EndpointType EndpointSystemType
        {
            get
            {
                return (EndpointType)Convert.ToInt16(ddlEndPointSystemType.SelectedValue);
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

        public static int UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                _UserId = value;
            }
        }

        static EndPoint _endPoint;

        EndPoint GetEndPoint(long id)
        {
            if (_endPoint == null || _endPoint.Id != id)
            {
                _endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(
                                DomainApiAction.Endpoint.EndPoint.GetByIdIncludingEndPointSystemType,
                                new Dictionary<string, string>() { { "id", id.ToString() } });
            }
            return _endPoint;
        }

        public ApplicationType EntitlementDetailType
        {
            get
            {
                if (this.EndpointSystemType == EndpointType.InhouseSystems)
                {
                    return ApplicationType.InhouseSystems;
                }
                else if (this.EndpointSystemType == EndpointType.SRM)
                {
                    return ApplicationType.SRM;
                }
                else
                {

                    Dto.EndPoint endpoint = GetEndPoint(this.SystemId);
                    if (endpoint != null)
                        return endpoint.ApplicationType;
                    else
                        return ApplicationType.None;
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            BindEndPointSystemType();
            DatePickerValidFrom.SelectedDate = DateTime.Now;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                WebControl.DisabledCssClass = null;
                SupervisorAutoCompleteBox.Disabled = true;
            }
        }

        public void Initialize()
        {
            if (Request.QueryString["BulkRequestId"] != null)
            {

                return;
            }
            if (!Page.IsPostBack)
            {
                lblLicenseMsg.Text = "";
                Session["BulkAdhocrequestObj"] = null;
                IsFulltimeEmployee = true;
                BindEndPointSystemType();
                if (DatePickerValidFrom.MinDate >= DateTime.Now)
                {
                    DatePickerValidFrom.SelectedDate = DatePickerValidFrom.MinDate;
                }
            }
            serverDateTime.Value = DateTime.Today.ToString("yyyy-MM-dd");
        }

        protected void UserSelectionChanged(object sender, EventArgs e)
        {
            lblLicenseMsg.Text = "";
            ddlEndPointSystemType.SelectedIndex = 0;
            ddlEndpoint.Items.Clear();
            RadioButton userselection = (RadioButton)sender;
            UserNameAutoCompleteBox.Entries.Clear();
            SupervisorAutoCompleteBox.Clear();

            this.RequestedUser.Clear();
            UserRequestGrid.Rebind();
            IsFulltimeEmployee = EmployeeField.Checked;
            BindEndPointSystemType();

        }

        protected void RequestTypeSelectionChanged(object sender, EventArgs e)
        {

            if (NewAccRadioButton.Checked == true)
            {
                if (ddlEndpoint.SelectedIndex > 0 && this.EntitlementDetailType == ApplicationType.Office365)
                {
                    lblLicenseMsg.Text = MasterConfigHelper.AdhocLicenseMsg;
                }
                AmendmentPanel.Visible = false;
                AddAmndRadioButton.Visible = false;
                DeleteAmndRadioButton.Visible = false;
                DatePickerValidFrom.MinDate = DateTime.Now;

            }
            if (DelAccRadioButton.Checked == true)
            {
                lblLicenseMsg.Text = "";
                AmendmentPanel.Visible = false;
                AddAmndRadioButton.Visible = false;
                DeleteAmndRadioButton.Visible = false;

            }
            else if (AmendmentRadioButton.Checked == true)
            {
                lblLicenseMsg.Text = "";
                AmendmentPanel.Visible = true;
                AddAmndRadioButton.Visible = true;
                DeleteAmndRadioButton.Visible = true;

            }
            var amendRemoveNotApplicableApplicationTypes = new[] { ApplicationType.HRMS_FLINS, ApplicationType.NPAC, ApplicationType.SSIDS_GUI, ApplicationType.SSIDS_SMS, ApplicationType.SRM };
            DeleteAmndRadioButton.Enabled = !amendRemoveNotApplicableApplicationTypes.Contains(this.EntitlementDetailType);
        }

        private void BindEndPointSystemType()
        {
            ddlEndPointSystemType.Items.Clear();
            ddlEndPointSystemType.Items.Insert(0, new ListItem("--Select--", "-1"));

            var endpointSystemTypes = Enum.GetValues(typeof(EndpointType));
            var endpointSystemTypelist = endpointSystemTypes.OfType<EndpointType>().Where(t => getApplicableEndpointTypes().Contains(t)).OrderBy(i => i.GetDescription().ToString()).ToList();

            foreach (EndpointType item in endpointSystemTypelist)
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                ListItem listItem = new ListItem(description, ((int)item).ToString());
                ddlEndPointSystemType.Items.Add(listItem);
            }
        }
        /// <summary>
        /// EndpointType VPN,SFB,o365 removed after discussion(10-02-2020)
        /// </summary>
        /// <returns></returns>
        EndpointType[] getApplicableEndpointTypes()
        {
            return new EndpointType[] {
                EndpointType.ActiveDirectory,
                EndpointType.SAP,
                //EndpointType.SkypeForBusiness,
                //EndpointType.VPN,
                EndpointType.SSIDS,
                EndpointType.SRM,
                EndpointType.SAPEnterprisePortal,
                EndpointType.InhouseSystems,
                EndpointType.IDMS,
                EndpointType.GEMS,
                //EndpointType.Office365,
                 EndpointType.OneCard,
                 EndpointType.SAPHANA,
            };
        }

        List<EndPoint> GetEndPointsByType(long EndPointSystemTypeId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", EndPointSystemTypeId.ToString());
            List<EndPoint> list = DomainApiClient<Dto.EndPoint>.GetByAction<List<Dto.EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetByEndpointSystemTypeId, parameter);
            if (list != null && list.Count > 0)
            {
                list = list.OrderBy(i => i.DisplayName).ToList();
            }
            return list;
        }

        private void BindEndPointSystem()
        {
            ddlEndpoint.Items.Clear();
            try
            {
                if (ddlEndPointSystemType.SelectedIndex > 0)
                {
                    long EndPointSystemTypeId = Convert.ToInt32(ddlEndPointSystemType.SelectedValue);

                    ddlEndpoint.Items.Clear();

                    if (this.EndpointSystemType == EndpointType.SAP)
                    {
                        List<EndPoint> list = GetEndPointsByType(EndPointSystemTypeId);
                        list = list.Where(e => !e.IsTrustedSource).ToList();
                        list.RemoveAll(item => item.ApplicationType == ApplicationType.SAP_SRP);
                        list.RemoveAll(item => item.ApplicationType == ApplicationType.SAP_SUS);
                        DropDownHelper.DropDownListBind(ddlEndpoint, list, "DisplayName", "Id", null, true, false);
                    }
                    else if (this.EndpointSystemType == EndpointType.ActiveDirectory)
                    {
                        List<EndPoint> list = GetEndPointsByType(EndPointSystemTypeId);
                        list.RemoveAll(item => item.ApplicationType != ApplicationType.ADSingaporePowerLocal);
                        DropDownHelper.DropDownListBind(ddlEndpoint, list, "DisplayName", "Id", null, true, false);
                    }
                    else
                    {
                        List<EndPoint> list = GetEndPointsByType(EndPointSystemTypeId);
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

        User GetUserById(long id)
        {
            return DomainApiClient<Dto.User>.GetById(id);
        }

        public void BindAdhocRequest(Dto.BulkRequest Request)
        {
            ExistingBulkRequest = Request;
            employeetypetr.Disabled = true;
            this.DatePickerValidFrom.SelectedDate = Request.ActivationDate;
            EmployeeField.Checked = Request.IsEmployee;
            IsFulltimeEmployee = EmployeeField.Checked;
            NonEmployeeField.Checked = !Request.IsEmployee;
            UserNameAutoCompleteBox.Entries.Clear();
            RequestedUser.Clear();
            Request.BulkRequestDetails.ForEach(l =>
            {
                RequestedUser = RequestedUser;
                if (!(RequestedUser.Select(lq => lq.Id).Contains(l.UserId)))
                {
                    User user = GetUserById(l.UserId);
                    if (user != null)
                        RequestedUser.Add(user);
                }
            });
            UserRequestGrid.Rebind();

            SupervisorAutoCompleteBox.Clear();
            SupervisorAutoCompleteBox.Attributes["selectedsupervisor"] = Convert.ToString(Request.SelectedSupervisor);
            if (Request.ActualSupervisor != null)
                SupervisorAutoCompleteBox.Attributes["actualsupervisor"] = Convert.ToString(Request.ActualSupervisor);
            SupervisorAutoCompleteBox.Attributes["supervisorchanged"] = Convert.ToString(Request.SupervisorChanged);

            if (Request.SelectedSupervisor != null)
            {
                if (SupervisorAutoCompleteBox.SelectedUser == null || SupervisorAutoCompleteBox.SelectedUser.Id != Request.SelectedSupervisor.Value)
                {
                    SupervisorAutoCompleteBox.SelectedUser = DomainApiClient<Dto.User>.GetById(Request.SelectedSupervisor.Value);
                }
            }

            ddlEndPointSystemType.SelectedValue = Convert.ToString(Convert.ToInt16(Request.EndpointType));
            ddlEndPointSystemType_SelectedIndexChanged(null, null);
            ddlEndpoint.SelectedValue = Convert.ToString(Request.EndpointSystemId);
            ddlEndpoint_SelectedIndexChanged(null, null);
            NewAccRadioButton.Checked = Request.AdhocActionType == AdHocActionType.NewAccount;
            AmendmentRadioButton.Checked = (Request.AdhocActionType == AdHocActionType.AmendmentAdd || Request.AdhocActionType == AdHocActionType.AmendmentRemove);
            DelAccRadioButton.Checked = Request.AdhocActionType == AdHocActionType.DeleteAccount;
            RequestTypeSelectionChanged(null, null);
            AddAmndRadioButton.Checked = Request.AdhocActionType == AdHocActionType.AmendmentAdd;
            DeleteAmndRadioButton.Checked = Request.AdhocActionType == AdHocActionType.AmendmentRemove;
            txtComments.Text = Request.Description;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        public bool Validate()
        {
            if (this.DatePickerValidFrom.IsEmpty)
            {
                ShowAlertMessage(AlertType.Warning, "Valid From should not be empty.");
                return false;
            }
            else if (RequestedUser.Count <= 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please select at least one user");
                return false;
            }
            if (SupervisorAutoCompleteBox.SelectedUser == null)
            {
                ShowAlertMessage(AlertType.Warning, "Please select a Supervisor.");
                return false;
            }
            if (ddlEndPointSystemType.SelectedValue == null || ddlEndPointSystemType.SelectedValue == "-1" || EndpointSystemType == EndpointType.None)
            {
                ShowAlertMessage(AlertType.Warning, "Please select Application Type.");
                return false;
            }
            EndPoint endpnt = GetEndPoint(this.SystemId);

            if (ApplicationType.SRM != this.EntitlementDetailType && endpnt == null)
            {
                ShowAlertMessage(AlertType.Warning, "Please select the Application.");
                return false;
            }
            if (EntitlementDetailType == ApplicationType.MicrosoftTeams && string.IsNullOrWhiteSpace(txtComments.Text))
            {
                ShowAlertMessage(AlertType.Warning, "Please enter the Comments.");
                return false;
            }
            try
            {
                foreach (var usr in RequestedUser)
                {
                    var criteria = new Dto.Presentation.CanProceedSecondStepCriteria
                    {
                        RequestTypeId = Convert.ToInt64(RequestType.Provisioning),
                        EndpointSystemId = this.SystemId,
                        AccountType = AccountType.User,
                        ActionType = NewAccRadioButton.Checked ? AdHocActionType.NewAccount : (DelAccRadioButton.Checked ? AdHocActionType.DeleteAccount : (AddAmndRadioButton.Checked ? AdHocActionType.AmendmentAdd : AdHocActionType.AmendmentRemove)),
                        RequestDate = DateTime.Today,
                        EndPointSystemType = EndpointSystemType,
                        ApplicationType = EntitlementDetailType,
                        UserType = usr.EmployeeType,
                        UserId = 0
                    };
                    var canProceed = DomainApiClient<Object>.GetByAction<BooleanStatus>("WorkflowController", DomainApiAction.Workflow.WorkflowConfiguration.CanProceedToSecondStepBulkRequest, null, criteria);
                    if (canProceed.CanProceed == false)
                    {
                        ShowAlertMessage(AlertType.Warning, canProceed.Message);
                        return false;
                    }
                }
                
            }
            catch (Exception)
            {
            }
            return true;
        }

        public Dto.BulkRequest ValidateBulkRequest(out bool isValid)
        {
            isValid = true;
            Dto.BulkRequest bulkRequest = BulkAdHocAccessRequest;

            var bulkRequestParameter = new BulkRequestParameter()
            {
                AccountType = AccountType.User,
                EndpointSystemId = bulkRequest.EndpointSystemId,
                EndpointSystemTypeId = bulkRequest.EndpointType,
                keyValuePairs = new Dictionary<string, string>(),
                ApplicationType = this.EntitlementDetailType
            };
            ValidationUser returnValue = null;
            if (NewAccRadioButton.Checked)
            {
                bulkRequestParameter.ActionType = Infrastructure.Enums.ActionType.NewAccount;
            }
            else if (DelAccRadioButton.Checked)
            {
                bulkRequestParameter.ActionType = Infrastructure.Enums.ActionType.DeleteAccount;
            }
            else if (AmendmentPanel.Visible && AddAmndRadioButton.Checked)
            {
                bulkRequestParameter.ActionType = Infrastructure.Enums.ActionType.AmendmentAdd;
            }
            else if (AmendmentPanel.Visible && DeleteAmndRadioButton.Checked)
            {
                bulkRequestParameter.ActionType = Infrastructure.Enums.ActionType.AmendmentRemove;
            }
            returnValue = BulkAdhocAccessRequestUserValidation.ValidateUsers(bulkRequestParameter, RequestedUser);

            if (returnValue != null)
            {
                isValid = false;
                string msg = BulkAdhocAccessRequestUserValidation.GetErrormessage(returnValue.ErrorType);
                if (!string.IsNullOrEmpty(msg))
                {
                    if (returnValue.DisplayName.IndexOf(',') > -1)
                    {
                        msg = msg.Replace("does not hold", "do not hold");
                    }
                    this.ShowAlertMessage(AlertType.Error, $"{returnValue.DisplayName} {msg}.");
                }
            }


            RequestedUser.ForEach(l =>
            {
                bulkRequest.BulkRequestDetails.Add(new BulkRequestDetail()
                {
                    BulkRequestId = bulkRequest.Id,
                    Designation = l.SectionName,
                    EmailId = l.EmailId,
                    EmployeeId = l.EmployeeId,
                    OneKey = l.OneKeyTokenId,
                    OrgBranch = l.OrgBranch,
                    OrgCompany = l.OrgCompany,
                    OrgDepartment = l.OrgDepartment,
                    OrgDivision = l.OrgDivision,
                    OrgSection = l.OrgSection,
                    OrgUnit = l.Reserved8,
                    UserId = l.Id,
                    UserName = l.UserName,
                    Name = l.DisplayName
                });
            });


            return bulkRequest;

        }

        private void clearfields()
        {
            ddlEndpoint.ClearSelection();
            ddlEndPointSystemType.ClearSelection();
            txtComments.Text = string.Empty;
            RequestedUser.Clear();
            UserRequestGrid.Rebind();
        }

        Dto.BulkRequest GetBulkRequest()
        {
            Dto.BulkRequest adhocrequest = null;

            if (Session["BulkAdhocrequestObj"] != null)
                adhocrequest = (Dto.BulkRequest)Session["BulkAdhocrequestObj"];
            else
                adhocrequest = new Dto.BulkRequest();

            if (ExistingBulkRequest != null)
            {
                adhocrequest.EntitlementDetail = ExistingBulkRequest.EntitlementDetail;
            }
            adhocrequest.ActivationDate = this.DatePickerValidFrom.SelectedDate;
            adhocrequest.AdhocActionType = this.AdhocActionType;
            adhocrequest.RequestedBy = new SessionStatusImpl().UserId;
            adhocrequest.RequestStatus = AccessRequestStatus.Submitted;
            adhocrequest.ProgressStatus = AccessRequestProgress.Submitted;
            adhocrequest.RequestSource = AccessRequestSource.AdhocRequest;
            if (adhocrequest.BulkRequestDetails == null)
                adhocrequest.BulkRequestDetails = new List<BulkRequestDetail>();
            adhocrequest.BulkRequestDetails.Clear();

            adhocrequest.Description = this.txtComments.Text;
            adhocrequest.EndpointSystemId = this.SystemId;
            adhocrequest.EndpointType = this.EndpointSystemType;
            adhocrequest.ApplicationType = this.EntitlementDetailType;
            adhocrequest.IsRequestedToRemove = (AdhocActionType == AdHocActionType.AmendmentRemove || AdhocActionType == AdHocActionType.DeleteAccount);
            adhocrequest.IsEmployee = EmployeeField.Checked;
            adhocrequest.ActualSupervisor = ActualSupervisor != null ? (long?)ActualSupervisor.Id : null;
            adhocrequest.SelectedSupervisor = SupervisorAutoCompleteBox.SelectedUser != null ? SupervisorAutoCompleteBox.SelectedUser.Id : 0;
            if (!string.IsNullOrEmpty(SupervisorAutoCompleteBox.Attributes["actualsupervisor"]))
                adhocrequest.ActualSupervisor = Convert.ToInt32(SupervisorAutoCompleteBox.Attributes["actualsupervisor"]);
            adhocrequest.SupervisorChanged = Convert.ToBoolean(SupervisorAutoCompleteBox.Attributes["supervisorchanged"]);
            return adhocrequest;
        }

        protected void ddlEndpoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEndpoint.SelectedIndex > 0)
            {

                if (this.EntitlementDetailType == ApplicationType.MicrosoftTeams)
                {
                    SetMandatory(RequestReasonDiv, true);
                }
                else
                {
                    SetMandatory(RequestReasonDiv, false);
                }
                bool disableAction = (this.EntitlementDetailType == ApplicationType.MicrosoftTeams || this.EntitlementDetailType == ApplicationType.SAP_EP || this.EntitlementDetailType == ApplicationType.CE || this.EntitlementDetailType == ApplicationType.VPN);
                if (disableAction)
                {
                    this.NewAccRadioButton.Enabled = false;
                    this.DelAccRadioButton.Enabled = false;
                    if (this.EntitlementDetailType == ApplicationType.VPN)
                    {
                        this.DelAccRadioButton.Enabled = true;
                    }
                    this.NewAccRadioButton.Checked = false;
                    this.AmendmentRadioButton.Checked = true;
                    this.AddAmndRadioButton.Checked = true;
                    this.DeleteAmndRadioButton.Checked = false;
                    this.AmendmentPanel.Visible = true;
                    this.AddAmndRadioButton.Visible = true;
                    this.DeleteAmndRadioButton.Visible = true;
                }
                else
                {
                    if (this.EntitlementDetailType != ApplicationType.VPN && this.EntitlementDetailType != ApplicationType.Office365 && this.EndpointSystemType != EndpointType.ActiveDirectory
                            && this.EntitlementDetailType != ApplicationType.HRMS_FLINS && this.EntitlementDetailType != ApplicationType.IDMS)
                    {
                        this.NewAccRadioButton.Enabled = true;
                        this.DelAccRadioButton.Enabled = true;
                        this.AmendmentRadioButton.Checked = false;
                        this.NewAccRadioButton.Checked = true;
                        this.AmendmentPanel.Visible = false;
                        this.AddAmndRadioButton.Visible = false;
                        this.DeleteAmndRadioButton.Visible = false;
                        this.AddAmndRadioButton.Checked = true;
                        this.DeleteAmndRadioButton.Checked = false;

                        if (this.EntitlementDetailType == ApplicationType.OneCard && EmployeeField.Checked)
                        {
                            NewAccRadioButton.Checked = false;
                            DelAccRadioButton.Checked = false;
                            NewAccRadioButton.Enabled = false;
                            DelAccRadioButton.Enabled = false;
                            this.AmendmentRadioButton.Checked = true;
                            this.RequestTypeSelectionChanged(sender, e);

                        }
                    }

                    else
                    {
                        if (this.EntitlementDetailType == ApplicationType.Office365 || this.EndpointSystemType == EndpointType.ActiveDirectory
                            || this.EntitlementDetailType == ApplicationType.HRMS_FLINS || this.EntitlementDetailType == ApplicationType.IDMS)
                        {
                            if (EmployeeField.Checked)
                            {
                                if (this.EntitlementDetailType == ApplicationType.SAPLocal)
                                {
                                    this.NewAccRadioButton.Enabled = true;
                                    this.DelAccRadioButton.Enabled = true;
                                    this.AmendmentRadioButton.Checked = false;
                                    this.NewAccRadioButton.Checked = true;
                                    this.AmendmentPanel.Visible = false;
                                    this.AddAmndRadioButton.Visible = false;
                                    this.DeleteAmndRadioButton.Visible = false;
                                    this.AddAmndRadioButton.Checked = true;
                                    this.DeleteAmndRadioButton.Checked = false;
                                    this.RequestTypeSelectionChanged(sender, e);

                                }
                                else
                                {
                                    this.NewAccRadioButton.Checked = false;
                                    this.NewAccRadioButton.Enabled = false;
                                    this.AmendmentRadioButton.Checked = true;
                                    this.AddAmndRadioButton.Checked = true;
                                    this.DeleteAmndRadioButton.Checked = false;
                                    this.AmendmentPanel.Visible = true;
                                    this.AddAmndRadioButton.Visible = true;
                                    this.DeleteAmndRadioButton.Visible = true;
                                    this.DelAccRadioButton.Checked = false;
                                    this.DelAccRadioButton.Enabled = false;
                                    this.RequestTypeSelectionChanged(sender, e);
                                }
                            }
                            else
                            {
                                if (this.EntitlementDetailType == ApplicationType.Office365)
                                {
                                    lblLicenseMsg.Text = DBConfigHelper.MasterConfigHelper.AdhocLicenseMsg;
                                }
                                this.NewAccRadioButton.Checked = true;
                                this.NewAccRadioButton.Enabled = true;
                                this.AmendmentRadioButton.Checked = false;
                                this.DeleteAmndRadioButton.Checked = false;
                                this.AmendmentPanel.Visible = false;
                                this.AddAmndRadioButton.Visible = false;
                                this.AddAmndRadioButton.Checked = true;
                                this.DelAccRadioButton.Checked = false;
                                this.DelAccRadioButton.Enabled = true;


                                if (this.EntitlementDetailType == ApplicationType.IDMS)
                                {
                                    this.NewAccRadioButton.Checked = false;
                                    this.NewAccRadioButton.Enabled = false;
                                    this.AmendmentRadioButton.Checked = true;
                                }

                                this.RequestTypeSelectionChanged(sender, e);
                            }
                        }


                    }
                }
            }
            else
            {
                this.NewAccRadioButton.Enabled = true;
                this.DelAccRadioButton.Enabled = true;
                this.AmendmentRadioButton.Checked = false;
                this.NewAccRadioButton.Checked = true;
            }
        }

        protected void ddlEndPointSystemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEndPointSystemType.SelectedIndex > 0)
            {
                lblLicenseMsg.Text = "";
                BindEndPointSystem();
                this.RequestTypeSelectionChanged(sender, e);
            }

        }

        protected void UserNameAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            int userID = Convert.ToInt32(this.UserNameAutoCompleteBox.Entries[0].Value);

            var ActualSupervisor = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetROByUser,
                                                                                    new Dictionary<string, string>{
                                                                                        { "id", userID.ToString() } });
            if (ActualSupervisor != null)
            {
                if (SupervisorAutoCompleteBox.SelectedUser == null)
                {
                    SupervisorAutoCompleteBox.SelectedUser = ActualSupervisor;
                    SupervisorAutoCompleteBox.Attributes["actualsupervisor"] = Convert.ToString(ActualSupervisor.Id);
                }
            }
            else
                SupervisorAutoCompleteBox.Attributes["actualsupervisor"] = "";
            if (!string.IsNullOrEmpty(e.Entry.Text))
            {
                BindValidFromTo();
            }

        }

        private void BindValidFromTo()
        {
            DatePickerValidFrom.Clear();
            if (this.UserNameAutoCompleteBox.Entries.Count > 0)
            {
                if (UserNameAutoCompleteBox.Entries.Count == 2)
                {
                    UserNameAutoCompleteBox.Entries.Remove(UserNameAutoCompleteBox.Entries[1]);
                }
                User user = GetUserById(Convert.ToInt32(UserNameAutoCompleteBox.Entries[0].Value));
                if (user.TerminationDate != null)
                {
                    DatePickerValidFrom.MaxDate = (DateTime)user.TerminationDate;
                }
                DatePickerValidFrom.SelectedDate = DateTime.Now;
            }
        }

        protected void SupervisorAutoCompleteBox_EntryAdded(object sender)
        {

        }

        protected void UserNameAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (RequestedUser.Count <= 0)
                SupervisorAutoCompleteBox.Clear();
        }

        protected void AddRequestButton_Click(object sender, EventArgs e)
        {
            if (UserNameAutoCompleteBox.Entries.Count != 0)
            {
                if (SupervisorAutoCompleteBox.SelectedUser == null)
                {
                    ShowAlertMessage(AlertType.Error, "User does not hold supervisor. Please select the supervisor.");
                    return;
                }
                List<User> requests = new List<User>();

                requests = this.RequestedUser;
                List<long> listofOldEntiltement = this.RequestedUser.Select(lq => lq.Id).ToList();
                if (!listofOldEntiltement.Contains(Convert.ToInt32(UserNameAutoCompleteBox.Entries[0].Value)))
                {
                    var user = GetUserById(Convert.ToInt32(UserNameAutoCompleteBox.Entries[0].Value));
                    requests.Add(user);
                    SupervisorAutoCompleteBox.Attributes["selectedsupervisor"] = Convert.ToString(SupervisorAutoCompleteBox.SelectedUser);
                    SupervisorAutoCompleteBox.Attributes["supervisorchanged"] = SupervisorAutoCompleteBox.Attributes["selectedsupervisor"] == SupervisorAutoCompleteBox.Attributes["actualsupervisor"] ? Convert.ToString(false) : Convert.ToString(true);
                    this.RequestedUser = requests;
                    UserRequestGrid.Rebind();
                    UserNameAutoCompleteBox.Entries.Clear();
                    if (RequestedUser.Count <= 0)
                        SupervisorAutoCompleteBox.Clear();

                }
                else
                {
                    ShowAlertMessage(AlertType.Error, "The User is already in the requested list.");
                }
            }
            else
            {
                ShowAlertMessage(AlertType.Error, "Please select the user.");
            }
        }

        protected void UserRequestGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = this.RequestedUser;
                requests.RemoveAt(e.Item.ItemIndex);
                this.RequestedUser = requests;
                UserRequestGrid.Rebind();
            }
            else if (e.CommandName == "OrgInfo")
            {
                User user = new User();
                user = GetUserById(Convert.ToInt32(e.CommandArgument));
                if (user != null && user != null)
                {
                    txtCompany.Text = user.OrgCompany;
                    txtDepartment.Text = user.OrgDepartment;
                    txtBranch.Text = user.OrgBranch;
                    txtSection.Text = user.OrgSection;
                    lblUnitName.Text = user.OrgUnit;
                    if (!string.IsNullOrEmpty(user.CostCenter))
                    {
                        trCostCenter.Visible = true;
                        txtCostCenter.Text = user.CostCenter;
                    }
                    else
                    {
                        trCostCenter.Visible = false;
                    }
                    OrgInfoModalPopupExtender.VisibleOnPageLoad = true;
                }
            }
        }

        protected void UserRequestGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (this.RequestedUser == null)
            {
                RequestedUser = new List<User>();
            }
            UserRequestGrid.DataSource = this.RequestedUser;
            if (this.RequestedUser.Count > 0)
                RosEmpid = RequestedUser[0].ROEmployeeId;
            else
            {
                SupervisorAutoCompleteBox.Clear();
                RosEmpid = "";
            }
            trGrdid.Visible = this.RequestedUser.Count == 0 ? false : true;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            OrgInfoModalPopupExtender.VisibleOnPageLoad = false;
        }
    }

    public class ValidationUser
    {
        public string DisplayName { get; set; }
        public int ErrorType { get; set; }
    }
}