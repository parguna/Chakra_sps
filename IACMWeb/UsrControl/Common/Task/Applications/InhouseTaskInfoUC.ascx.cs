using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common.Task.Applications
{
    public partial class InhouseTaskInfoUC : UIBaseUC
    {
        #region Members

        private SessionStatusImpl _sessionStatus = null;
        private const string V_ACCOUNTDETAIL = "ACCOUNTDETAIL";
        private const string V_USERPASWWORD = "PASSWORD";
        private const string V_REQUEST = "V_REQUEST";
        private const string V_MASTERUSER = "V_MASTERUSER";
        #endregion

        #region Properties

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


        public Dto.User MasterUser
        {
            get
            {
                if (ViewState[V_MASTERUSER] == null)
                {
                    ViewState[V_MASTERUSER] = DomainApiClient<Dto.User>.GetById(Task.AccessRequest.UserId);
                }
                return (User)ViewState[V_MASTERUSER];
            }
            set
            {
                ViewState[V_MASTERUSER] = value;
            }
        }

        public List<KeyValuePair<string, string>> EntitlementRequest
        {
            get
            {
                if (ViewState[V_REQUEST] != null)
                {
                    return (List<KeyValuePair<string, string>>)ViewState[V_REQUEST];
                }
                return new List<KeyValuePair<string, string>>();
            }
            set
            {
                ViewState[V_REQUEST] = value;
            }
        }

        public EndPoint SelectedEndpoint
        {
            get { return Task.AccessRequest.AdHocAccessRequest.EndPoint; }
        }
        public string UserPassword
        {
            get
            {
                if (ViewState[V_USERPASWWORD] == null)
                    return txtINHPassword.Text.Trim();
                return (string)ViewState[V_USERPASWWORD];
            }
            set
            {
                ViewState[V_USERPASWWORD] = value;
            }
        }
        public AccountReconcilation AccountReconcilation
        {
            get
            {
                if (ViewState[V_ACCOUNTDETAIL] == null)
                    return null;
                return (AccountReconcilation)ViewState[V_ACCOUNTDETAIL];
            }
            set
            {
                ViewState[V_ACCOUNTDETAIL] = value;
            }
        }
        public long EndPointUserId
        {
            get
            {
                if (ViewState["EndPointUserId"] != null)
                {
                    return (long)ViewState["EndPointUserId"];
                }
                return 0;
            }
            set
            {
                ViewState["EndPointUserId"] = value;
            }
        }

        public AdHocActionType ActionType
        {
            get
            {
                if (null == ViewState["ActionType"])
                {
                    return AdHocActionType.None;
                }
                return (AdHocActionType)ViewState["ActionType"];
            }
            set
            {
                ViewState["ActionType"] = value;
            }
        }

        private bool IsEditMode
        {
            get
            {
                if (ViewState["IsEditMode"] != null)
                {
                    return Convert.ToBoolean(ViewState["IsEditMode"]);
                }
                return false;
            }
            set
            {
                ViewState["IsEditMode"] = value;
            }
        }

        private LockScenario LockScenario
        {
            get
            {
                return (LockScenario)Convert.ToInt32(ddlLockScenario.SelectedItem.Value);
            }
        }

        public long UserID
        {
            get
            {
                if (ViewState["ID"] != null)
                {
                    return Convert.ToInt64(ViewState["ID"]);
                }
                return 0;
            }
            set
            {
                ViewState["ID"] = value;
            }
        }

        public bool IsOffBoardRequest
        {
            get
            {
                if (ViewState["IsOffBoardRequest"] != null)
                {
                    return Convert.ToBoolean(ViewState["IsOffBoardRequest"]);
                }
                return false;
            }
            set
            {
                ViewState["IsOffBoardRequest"] = value;
            }
        }

        public AccessRequestSource RequestSource
        {
            get
            {
                if (ViewState["RequestSource"] != null)
                {
                    return (AccessRequestSource)(ViewState["RequestSource"]);
                }
                return AccessRequestSource.None;
            }
            set
            {
                ViewState["RequestSource"] = value;
            }
        }

        public string Username
        {
            get
            {
                return txtUserName.Text.Trim();
            }
        }

        public string Password
        {
            get
            {
                return txtINHPassword.Text;
            }
        }

        public string DisplayName
        {
            get
            {
               return txtDisplayName.Text.Trim();
            }
        }

        #endregion

        #region Methods
        public void InitializeForm()
        {
            if (Task.IsActionOnlyRequiredTask)
            {
                Session["InhouseSystemEndpointId"] = Task.AccessRequest.AdHocAccessRequest.EndPoint.Id;
                ActionType = Task.AccessRequest.AdHocAccessRequest.AdhocActionType;
                lblADAccessSystems.Text = MasterConfigHelper.InhouseSystemsToShowInTaskDetailForAD;
                //if (Task.AccessRequest.AdHocAccessRequest.AdhocActionType != AdHocActionType.None)
                //{
                //    if (Task.AccessRequest.AdHocAccessRequest != null)
                //        BindInhouseUserInfo();
                //}
                if (Task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    ContentPanel.Visible = true;
                    AccountOwnerPanel.Visible = true;
                    InitMyTaskDetails();
                    BindInhouseUserInfo();
                    SetNewAccount(Task);
                }

                if (Task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove && Task.AccessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.InhouseSystems)
                {
                    var entitlementDetail = (AdHocINHRequest)Task.AccessRequest.AdHocAccessRequest.GetEntitlementDetail();
                    Session["endpointUserId"] = entitlementDetail.EndpointUserId;
                    //RolesAutoCompleteBox.WebServiceSettings.Method = "GetEntitlementInHouseExisting";
                }

                if (Task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd && Task.AccessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.InhouseSystems)
                {
                    var entitlementDetail = (AdHocINHRequest)Task.AccessRequest.AdHocAccessRequest.GetEntitlementDetail();
                    Session["EndpointUserId"] = entitlementDetail.EndpointUserId;
                    //RolesAutoCompleteBox.WebServiceSettings.Method = "GetAllPermissionsExceptExisting";
                }

                //if (Task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                //{
                //    SetDeleteAccount(Task);
                //}
                //if (Task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                //{
                //    SetAmendmentAdd(Task);
                //}
                //if (Task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                //{
                //    SetAmendmentRemove(Task);
                //}
                if (Task.IsCompleted)
                {
                    ContentPanel.Enabled = false;
                }
            }
            else
            {
                ContentPanel.Visible = false;
                AccountOwnerPanel.Visible = false;
                Notifications.Visible = false;
            }
        }

        InhouseUserInfo GetInhouseUserByEmployeeId(string employeeId)
        {
            var param = new Dictionary<string, string>();
            param.Add("employeeId", employeeId);
            param.Add("endpointId", SelectedEndpoint.Id.ToString());

            var inhouseUser = DomainApiClient<Dto.InhouseUserInfo>.GetByAction<Dto.InhouseUserInfo>("GetByEmployeeId", param);
            return inhouseUser;
        }
        List<InhouseUserInfo> GetAllAccountsByEmployeeId(string employeeId)
        {
            var param = new Dictionary<string, string>();
            param.Add("employeeId", employeeId);
            param.Add("endpointId", SelectedEndpoint.Id.ToString());

            var inhouseUser = DomainApiClient<Dto.InhouseUserInfo>.GetByAction<List<Dto.InhouseUserInfo>>("GetAllAccountsByEmployeeId", param);
            return inhouseUser;
        }
        void BindInhouseUserInfo()
        {
            User user = MasterUser;
            if (user == null)
                return;

            var account = GetAccount(user.Id);
            this.UserSearchUC.SelectedUser = user;
            lblApplicationType.Text = SelectedEndpoint.ApplicationType.GetDescription();
            lblEndpoint.Text = SelectedEndpoint.DisplayName;
            txtIdentificationNo.Text = user.EmployeeId;
            txtEmail.Text = user.EmailId;
            txtFirstName.Text = user.FirstName;
            txtLastName.Text = user.LastName;
            txtDisplayName.Text = user.DisplayName;

            //if (account != null && Task.AccessRequest.AccountType == AccountType.User)
            //{
            //    txtUserName.Text = account.AccountName;
            //    DatePickerAccCreated.SelectedDate = account.AccountCreatedDate;
            //    IsLockedCheckBox.Checked = account.IsLocked;
            //    IsDeletedCheckBox.Checked = account.IsDeleted;
            //    DatePickerIsDeleted.SelectedDate = account.DeletedDate;
            //    ddlLockScenario.SelectedValue = Convert.ToInt32(account.LockScenario).ToString();
            //}
            if (Task.AccessRequest.AccountType != AccountType.User)
            {
                var adHocADRequest = (AdHocINHRequest)Task.AccessRequest.AdHocAccessRequest.GetEntitlementDetail();
                if (adHocADRequest != null)
                {
                    if (adHocADRequest.AdHocINHSharedAccount != null)
                    {

                        txtUserName.Text = adHocADRequest.AdHocINHSharedAccount.AccountName;
                        txtDisplayName.Text = adHocADRequest.AdHocINHSharedAccount.DisplayName;
                    }
                    else if(Task.AccessRequest.RequestSource == AccessRequestSource.UndoEvent)
                    {
                        txtUserName.Text = adHocADRequest.DisplayAccountName;
                        txtDisplayName.Text = adHocADRequest.DisplayAccountName;
                    }
                }
            }
        }

        private void SetNewAccount(DI.Chakra.Dto.Task Task)
        {
            DatePickerAccCreated.Enabled = false;
            UserSearchUC.DisableMyAutocompleteBox = ddlLockScenario.Enabled = TypeofAccounts.Enabled = false;
            //TextBox[] txtboxs = { txtIdentificationNo, txtEmail, txtUserName, txtINHPassword, txtFirstName, txtLastName, txtDisplayName, txtRemarks1, txtRemarks2, txtRemarks3, txtRemarks4, txtRemarks5 };
            TextBox[] txtboxs = { txtIdentificationNo, txtEmail };
            TextboxControlsEnabled(txtboxs, false);
            CheckBox[] CheckBoxs = { IsLockedCheckBox, IsDisabledCheckBox, IsDeletedCheckBox };
            CheckBoxControlsEnabled(CheckBoxs, false);
            RadDatePicker[] RadDateTimePickers = { DatePickerLockedDate, DatePickerIsDisabled, DatePickerIsDeleted };
            RadDatePickerControlsEnabled(RadDateTimePickers, false);
            UserSearchUC.Disabled = true;
        }

        private void TextboxControlsEnabled(TextBox[] items, bool enabled)
        {
            foreach (TextBox item in items)
                item.Enabled = enabled;
        }

        public void CheckBoxControlsEnabled(CheckBox[] items, bool enabled)
        {
            foreach (CheckBox item in items)
                item.Enabled = enabled;
        }
        public void RadDatePickerControlsEnabled(RadDatePicker[] items, bool enabled)
        {
            foreach (RadDatePicker item in items)
                item.Enabled = enabled;
        }

        private void SetDeleteAccount(DI.Chakra.Dto.Task Task)
        {
            if (!Task.IsCompleted)
            {
                ContentPanel.Enabled = true;
                UserSearchUC.DisableMyAutocompleteBox = false;
                TextBox[] txtboxs = { txtIdentificationNo, txtEmail, txtUserName, txtINHPassword, txtFirstName, txtLastName, txtDisplayName, txtRemarks1, txtRemarks2, txtRemarks3, txtRemarks4, txtRemarks5 };
                TextboxControlsEnabled(txtboxs, false);
                CheckBox[] CheckBoxs = { IsLockedCheckBox, IsDisabledCheckBox, IsDeletedCheckBox };
                CheckBoxControlsEnabled(CheckBoxs, false);
                RadDatePicker[] RadDateTimePickersEnable = { DatePickerAccCreated, DatePickerLockedDate, DatePickerIsDisabled };
                RadDatePickerControlsEnabled(RadDateTimePickersEnable, false);
                RadDatePicker[] RadDateTimePickersDisable = { DatePickerIsDeleted };
                RadDatePickerControlsEnabled(RadDateTimePickersDisable, true);
                ddlLockScenario.Enabled = false;
                TypeofAccounts.Enabled = false;
            }
        }

        private void SetAmendmentRemove(DI.Chakra.Dto.Task Task)
        {
            if (!Task.IsCompleted)
            {
                ContentPanel.Enabled = true;
                UserSearchUC.DisableMyAutocompleteBox = false;
                TextBox[] txtboxs = { txtIdentificationNo, txtEmail, txtUserName, txtINHPassword, txtFirstName, txtLastName, txtDisplayName, txtRemarks1, txtRemarks2, txtRemarks3, txtRemarks4, txtRemarks5 };
                TextboxControlsEnabled(txtboxs, false);
                CheckBox[] CheckBoxs = { IsLockedCheckBox, IsDisabledCheckBox, IsDeletedCheckBox };
                CheckBoxControlsEnabled(CheckBoxs, false);
                RadDatePicker[] RadDateTimePickersEnable = { DatePickerLockedDate, DatePickerIsDisabled, DatePickerIsDeleted };
                RadDatePickerControlsEnabled(RadDateTimePickersEnable, true);
                RadDatePicker[] RadDateTimePickersDisable = { DatePickerAccCreated };
                RadDatePickerControlsEnabled(RadDateTimePickersDisable, false);
                ddlLockScenario.Enabled = false;
                TypeofAccounts.Enabled = false;
            }
        }
        private void SetAmendmentAdd(DI.Chakra.Dto.Task Task)
        {
            if (!Task.IsCompleted)
            {
                ContentPanel.Enabled = true;
                UserSearchUC.DisableMyAutocompleteBox = false;
                TextBox[] txtboxs = { txtIdentificationNo, txtEmail, txtUserName, txtINHPassword, txtFirstName, txtLastName, txtDisplayName, txtRemarks1, txtRemarks2, txtRemarks3, txtRemarks4, txtRemarks5 };
                TextboxControlsEnabled(txtboxs, false);
                CheckBox[] CheckBoxs = { IsLockedCheckBox, IsDisabledCheckBox, IsDeletedCheckBox };
                CheckBoxControlsEnabled(CheckBoxs, false);
                RadDatePicker[] RadDateTimePickersEnable = { DatePickerLockedDate, DatePickerIsDisabled, DatePickerIsDeleted };
                RadDatePickerControlsEnabled(RadDateTimePickersEnable, true);
                RadDatePicker[] RadDateTimePickersDisable = { DatePickerAccCreated };
                RadDatePickerControlsEnabled(RadDateTimePickersDisable, false);
                ddlLockScenario.Enabled = false;
                TypeofAccounts.Enabled = false;
            }
            else
                ContentPanel.Enabled = false;
        }

        public AdHocINHRequest GetEntitlementDetail()
        {
            string strEntitlements = string.Empty;

            //if (ActionType == AdHocActionType.NewAccount && RequestSource != AccessRequestSource.UndoEvent && !Validate()) return null;

            if (ActionType != AdHocActionType.DeleteAccount)
            {
                if (SelectedEndpoint.IsFunctionLevel)
                    strEntitlements = string.Join("|", EntitlementRequest.Select(p => p.Key + Constants.SPLITTER_FUCNTIONS + p.Value));
                else
                    strEntitlements = string.Join(",", EntitlementRequest.Select(p => p.Key));
            }

            AdHocINHRequest entitlementDetail = new AdHocINHRequest
            {
                EntitlementType = EntitlementType.Permission,
                EndpointUserId = EndPointUserId,
                Entitlement = strEntitlements,
                IsOffBoardRequest = IsOffBoardRequest
            };
            return entitlementDetail;
        }

        public string GetEntitlementString()
        {
            string strEntitlements = string.Empty;

            if (ActionType != AdHocActionType.DeleteAccount)
            {
                if (SelectedEndpoint.IsFunctionLevel)
                    strEntitlements = string.Join("|", EntitlementRequest.Select(p => p.Key + Constants.SPLITTER_FUCNTIONS + p.Value));
                else
                    strEntitlements = string.Join(",", EntitlementRequest.Select(p => p.Key));
            }

            return strEntitlements;
        }

        InhouseUserInfo GetInhouseUser()
        {
            var param = new Dictionary<string, string>();
            param.Add("endPointSystemId", SelectedEndpoint.Id.ToString());
            param.Add("userName", txtUserName.Text);
            InhouseUserInfo inhUser = DomainApiClient<Dto.InhouseUserInfo>.GetByAction<Dto.InhouseUserInfo>("GetByUserName", param);
            return inhUser;
        }
        public bool CreateINHUser()
        {
            try
            {
                var user = MasterUser;
                InhouseUserInfo inhUser = GetInhouseUser();
                ////Accountname exists for the different employee
                if (inhUser != null && inhUser.EmployeeId != user.EmployeeId)
                {
                    ShowAlertMessage(AlertType.Error, "Inhouse user already exists, please select a different username");
                    return false;
                }
                //Find all the accounts of an employee
                var inhUsers = GetAllAccountsByEmployeeId(user.EmployeeId);

                if(inhUsers != null && inhUsers.Count > 0)
                {
                    ///User must have only one User Account
                    //if (inhUsers != null && inhUsers.Exists(inhUsr => inhUsr.IsDeleted == false && inhUsr.AccountType == AccountType.User) && Task.AccessRequest.AccountType == AccountType.User)
                    //{
                    //    ShowAlertMessage(AlertType.Error, $"Inhouse user have an already have User Account : {inhUser.AccountName}");
                    //    return false;
                    //}
                    //Accountname exists for same employee
                    inhUser = inhUsers.FirstOrDefault(i => i.AccountName != null && i.AccountName.Equals(txtUserName.Text, StringComparison.InvariantCultureIgnoreCase));
                    if (inhUser != null && inhUser.IsDeleted == false && inhUser.AccountType != Task.AccessRequest.AccountType)
                    {
                        ShowAlertMessage(AlertType.Error, $"Same User account already exists for different account type:{inhUser.AccountType}, please select a different username");
                        return false;
                    }
                }

                //Account name doesnt exists
                if (inhUser == null)
                {
                    inhUser = new InhouseUserInfo();
                }

                inhUser.EndPointSystemId = SelectedEndpoint.Id;
                inhUser.InhouseSystemCode = SelectedEndpoint.ApplicationCode;
                inhUser.Email = txtEmail.Text.Trim();
                //Fixed this issue because AmendmentAdd/Remove/Delete failed because endpoint userid  getiing zero.
                inhUser.EmployeeId = user.EmployeeId;
                inhUser.NRIC = user.EmployeeId;
                inhUser.UserName = txtUserName.Text.Trim();
                inhUser.DisplayName = txtDisplayName.Text.Trim();
                inhUser.FirstName = txtFirstName.Text.Trim();
                inhUser.LastName = txtLastName.Text.Trim();
                inhUser.AccountType = Task.AccessRequest.AccountType;
                inhUser.CreatedBy = new SessionStatusImpl().User.DisplayName;
                inhUser.CreatedDate = DateTime.Now;
                inhUser.IsLocked = false;
                inhUser.Password = RSAHelper.EncryptData(txtINHPassword.Text);
                inhUser.AccountCreatedDate = DateTime.Now;
                inhUser.IsDisabled = false;
                inhUser.IsDeleted = false;
                inhUser.DeletedDate = null;
                inhUser.DisabledDate = null;
                var savedUserId = DomainApiClient<InhouseUserInfo>.SaveAndReturnId(inhUser);
                EndPointUserId = savedUserId;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected void AddPermissionButton_Click(object sender, EventArgs e)
        {
            if (Task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                return;

            if (SelectedEndpoint.IsFunctionLevel && !SelectedEndpoint.CanAutomate)
            {
                AddRequestForFunctions();
            }
            else
            {
                AddRequestForPermissionOnly();
            }
        }

        private bool EntitlementExists(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                foreach (KeyValuePair<string, string> req in EntitlementRequest)
                {
                    if (req.Key == key && req.Value == value)
                        return true;
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> req in EntitlementRequest)
                {
                    if (req.Key == key && req.Value.Split(',').Contains(value))
                        return true;
                }
            }

            return false;
        }

        void AddRequestForPermissionOnly()
        {
            //if (RolesAutoCompleteBox.Entries.Count > 0)
            //{
            //    var requests = new List<KeyValuePair<string, string>>();
            //    string newPermission = RolesAutoCompleteBox.Entries[0].Text.Replace(";", "").Trim();
            //    string newFunctions = string.Empty;
            //    requests = this.EntitlementRequest;
            //    if (!EntitlementExists(newPermission, newFunctions))
            //    {
            //        requests.Add(new KeyValuePair<string, string>(newPermission, newFunctions));
            //        this.EntitlementRequest = requests;
            //        FillRequestGrid();
            //        RolesAutoCompleteBox.Entries.Clear();
            //    }
            //    else
            //    {
            //        MasterPage.ShowAlert(AlertType.Error, "The permission is already in the request list.");
            //    }
            //}
            //else
            //{
            //    MasterPage.ShowAlert(AlertType.Error, "Please select the permission.");
            //}
        }
        private void FillRequestGrid()
        {
            if (this.EntitlementRequest == null)
            {
                EntitlementRequest = new List<KeyValuePair<string, string>>();
            }
            RequestGrid.DataSource = this.EntitlementRequest;
            divGrid.Visible = this.EntitlementRequest.Count == 0 ? false : true;
            RequestGrid.DataBind();
        }
        protected void RequestGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = this.EntitlementRequest;
                requests.RemoveAt(e.Item.ItemIndex);
                this.EntitlementRequest = requests;
                FillRequestGrid();
            }
        }
        void AddRequestForFunctions()
        {
            //if (RolesAutoCompleteBox.Entries.Count > 0 && ddlFunctions.CheckedItems.Count > 0)
            //{
            //    var requests = new List<KeyValuePair<string, string>>();
            //    string newPermission = RolesAutoCompleteBox.Entries[0].Text.Replace(";", "").Trim();
            //    List<string> lstFunctions = new List<string>();
            //    bool exists = false;
            //    foreach (var function in ddlFunctions.CheckedItems)
            //    {
            //        exists = EntitlementExists(newPermission, function.Text);
            //        if (exists)
            //        {
            //            break;
            //        }
            //        lstFunctions.Add(function.Text);
            //    }

            //    if (!exists)
            //    {
            //        string newFunctions = string.Join(",", lstFunctions);
            //        requests = this.EntitlementRequest;
            //        requests.Add(new KeyValuePair<string, string>(newPermission, newFunctions));
            //        this.EntitlementRequest = requests;
            //        FillRequestGrid();
            //        RolesAutoCompleteBox.Entries.Clear();
            //        ddlFunctions.Items.Clear();
            //    }
            //    else
            //    {
            //        MasterPage.ShowAlert(AlertType.Error, "The permission and function are already in the request list.");
            //    }
            //}
            //else
            //{
            //    MasterPage.ShowAlert(AlertType.Error, "Please select the permission and function.");
            //}
        }

        private void RebindPermissionGrid()
        {
            EntitlementRequest.Clear();
            var adhocRequest = Task.AccessRequest.AdHocAccessRequest;
            var requests = new List<KeyValuePair<string, string>>();
            if (adhocRequest != null && !string.IsNullOrWhiteSpace(adhocRequest.Entitlement))
            {
                if (SelectedEndpoint.IsFunctionLevel)
                {
                    foreach (var item in adhocRequest.Entitlement.Split('|'))
                    {
                        var ent = item.Split(new string[] { Constants.SPLITTER_FUCNTIONS }, StringSplitOptions.RemoveEmptyEntries);
                        if (ent.Count() > 1)
                            requests.Add(new KeyValuePair<string, string>(ent[0], ent[1]));
                        else
                            requests.Add(new KeyValuePair<string, string>(ent[0], string.Empty));
                    }
                }
                else
                {
                    foreach (var item in adhocRequest.Entitlement.Split(','))
                    {
                        requests.Add(new KeyValuePair<string, string>(item, string.Empty));
                    }
                }

                EntitlementRequest = requests;
            }
            FillRequestGrid();
        }

        bool IsAdhocRequestNull()
        {
            return Task.AccessRequest != null && Task.AccessRequest.AdHocAccessRequest != null;
        }

        protected void RolesAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            //if (RolesAutoCompleteBox.Entries.Count > 1)
            //{
            //    RolesAutoCompleteBox.Entries.RemoveAt(1);
            //    return;
            //}

            //if (!SelectedEndpoint.IsFunctionLevel)
            //    return;

            //if (RolesAutoCompleteBox.Entries.Count == 1 && IsAdhocRequestNull())
            //{
            //    Dictionary<string, string> parameter = new Dictionary<string, string>();
            //    parameter.Add("endpointId", Convert.ToString(Task.AccessRequest.AdHocAccessRequest.EndpointSystemId.Value));
            //    parameter.Add("permission", RolesAutoCompleteBox.Entries[0].Text);
            //    parameter.Add("endpointUserId", Convert.ToString(EndPointUserId));
            //    parameter.Add("type", Convert.ToInt32(Task.AccessRequest.AdHocAccessRequest.AdhocActionType).ToString());
            //    var functions = DomainApiClient<InhouseFunction>.
            //        GetByAction<List<InhouseFunction>>(DomainApiAction.Endpoint.InhouseSystems.GetInhouseFunctionActive, parameter);
            //    ddlFunctions.DataSource = functions;
            //    ddlFunctions.DataTextField = "FunctionName";
            //    ddlFunctions.DataValueField = "Id";
            //    ddlFunctions.DataBind();
            //    foreach (RadComboBoxItem itm in ddlFunctions.Items)
            //    {
            //        itm.Checked = true;
            //    }
            //}
            //else
            //{
            //    ddlFunctions.Items.Clear();
            //}
        }

        protected void RolesAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            //if (RolesAutoCompleteBox.Entries.Count == 0)
            //{
            //    ddlFunctions.Items.Clear();
            //}
        }
        void ShowFunctionColumn(bool hide)
        {
            RequestGrid.MasterTableView.GetColumn("Function").Display = hide;
            RequestGrid.MasterTableView.Columns.FindByUniqueName("Function").Visible = hide;
        }
        private void InitMyTaskDetails()
        {
            BindAccountType();
        }

        Dto.AccountReconcilation GetAccount(long userId)
        {
            var param = new Dictionary<string, string>
                    {
                        { "userId", userId.ToString()},
                        { "endpointSystemId", SelectedEndpoint.Id.ToString()},
                        { "accountType", AccountType.User.ToString()}
                    };

            List<AccountReconcilation> userAccounts = DomainApiClient<Dto.AccountReconcilation>.
                                                            GetByAction<List<Dto.AccountReconcilation>>(
                                                                            DomainApiAction.Provisioning.AccountReconcilation.GetInactiveAccounts, param);
            return userAccounts.FirstOrDefault();
        }

        public string Validate()
        {
            if (Task.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount && Task.AccessRequest.RequestSource != AccessRequestSource.UndoEvent)
            {
                string specialCharacter = "!@#$%^&*()+=[]\\\';,/{}|\":<>?";

                if (UserSearchUC.SelectedUser == null)
                {
                    return "select user";
                }

                string MandatoryMessage = string.Empty;
                TextBox[] MandatoryFields = { txtDisplayName, txtUserName, txtINHPassword };
                MandatoryMessage = MandatoryValidationmessage(MandatoryFields);

                if (!string.IsNullOrEmpty(MandatoryMessage))
                {
                    return MandatoryMessage;
                }

                for (int i = 0; i < specialCharacter.Length; i++)
                {
                    if (txtUserName.Text.Contains(specialCharacter[i].ToString()))
                    {
                        return "Special characters not allowed in User Name";
                    }
                }

                if (SelectedEndpoint == null)
                {
                    return string.Format("Please Select {0}.", T("App.Common.Endpoint"));
                }
                var user = MasterUser;
                InhouseUserInfo inhUser = GetInhouseUser();
                ////Accountname exists for the different employee
                if (inhUser != null && inhUser.EmployeeId != user.EmployeeId && inhUser.IsDeleted == false)
                {
                    return $"Inhouse userName already exists for EmployeeId:{inhUser.EmployeeId}, please select a different username";
                }
                //Find all the accounts of an employee
                var inhUsers = GetAllAccountsByEmployeeId(user.EmployeeId);

                if (inhUsers != null && inhUsers.Count > 0)
                {
                    ///User must have only one User Account
                    //if (inhUsers != null && inhUsers.Exists(inhUsr => inhUsr.IsDeleted == false && inhUsr.AccountType == AccountType.User) && Task.AccessRequest.AccountType == AccountType.User)
                    //{
                    //    return $"Inhouse user have an already have User Account : {inhUser.AccountName}";
                    //}
                    //Accountname exists for same employee
                    inhUser = inhUsers.FirstOrDefault(i => i.AccountName != null && i.AccountName.Equals(txtUserName.Text, StringComparison.InvariantCultureIgnoreCase));
                    if (inhUser != null && inhUser.IsDeleted == false && inhUser.AccountType != Task.AccessRequest.AccountType)
                    {
                        return $"Same User account already exists for different account type:{inhUser.AccountType}, please select a different username";
                    }
                }
            }

            return string.Empty;
        }

        string MandatoryValidationmessage(TextBox[] items)
        {
            string message = string.Empty;
            foreach (TextBox item in items)
            {
                switch (item.ID)
                {
                    case "txtUserName":
                        message = String.IsNullOrEmpty(txtUserName.Text) ? message + "User Name," : message;
                        break;
                    case "txtINHPassword":
                        message = String.IsNullOrEmpty(txtINHPassword.Text) ? message + "Password," : message;
                        break;
                    case "txtDisplayName":
                        message = String.IsNullOrEmpty(txtDisplayName.Text) ? message + "Display Name," : message;
                        break;
                }
            }
            return !String.IsNullOrEmpty(message) ? message.TrimEnd(',') + " should not be empty" : String.Empty;
        }
        private string EntiltementString(object Control)
        {
            string entiltment = "";
            RadAutoCompleteBox control = (RadAutoCompleteBox)Control;
            foreach (AutoCompleteBoxEntry selectedItem in control.Entries)
            {
                entiltment = entiltment == "" ? selectedItem.Value : entiltment + "," + selectedItem.Value;
            }
            return entiltment;
        }

        bool DoesExist(string name, long endPointSystemId)
        {
            Dto.InhouseUserInfo group = new Dto.InhouseUserInfo();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointSystemId", endPointSystemId.ToString());
            parameter.Add("userName", txtUserName.Text.Trim());
            return DomainApiClient<Dto.InhouseUserInfo>.GetByAction<bool>
                (DomainApiAction.Endpoint.InhouseSystems.InhouseUserInfo.DoesExists, parameter);
        }

        private bool SendEmailForAccountProvisioning(InhouseUserInfo user)
        {
            try
            {
                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("endpointUserId", user.Id.ToString());
                para.Add("endpointSystemId", user.EndPointSystemId.ToString());
                AccountReconcilation useracc = DomainApiClient<Dto.AccountReconcilation>.GetByAction<AccountReconcilation>
                    (DomainApiAction.Provisioning.AccountReconcilation.GetByUser, para);
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", useracc.UserId.ToString());
                User m_user = DomainApiClient<Dto.User>.GetByAction<User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

                Dictionary<string, string> eparameter = new Dictionary<string, string>();
                eparameter.Add("templateName", Constants.CREATE_ACCOUNT_NOTIFICATION_Inhouse_TEMPLATE.ToString());
                MailTemplate template = DomainApiClient<Dto.MailTemplate>.GetByAction<MailTemplate>
                    (DomainApiAction.Email.MailTemplate.GetByTemplateName, eparameter);

                if (template != null && m_user != null)
                {
                    EmailInfo emailInfo = new EmailInfo();
                    StringBuilder subject = new StringBuilder(template.MailSubject);

                    subject.Replace("@@USERDISPLAYNAME@@", m_user.DisplayName);
                    subject.Replace("@@USERNAME@@", user.AccountName);

                    StringBuilder body = new StringBuilder(template.MailContent);
                    body.Replace("@@RECEIVER@@", m_user.DisplayName);
                    body.Replace("@@USERNAME@@", user.UserName);
                    body.Replace("@@PASSWORD@@", user.Password);
                    body.Replace("@@PERNRNO@@", m_user.EmployeeId);
                    body.Replace("@@DATE@@", DateTime.Now.ToString());
                    //body.Replace("@@APPLICATIONNAME@@", ddlApplicaionName.SelectedItem.Text);
                    body.Replace("@@APPLICATIONNAMEFULL@@", string.Empty);

                    body.Replace("@@RES1@@", string.Empty);
                    body.Replace("@@RES2@@", string.Empty);
                    emailInfo.Body = body.ToString();
                    emailInfo.Subject = subject.ToString();
                    emailInfo.To = string.Format("{0}", user.Email);
                    emailInfo.Cc = string.Format("{0}", MasterConfigHelper.AccountCreatedNotificationTO);
                    emailInfo.From = template.MailFrom;
                    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                    //DomainApiClient<Dto.EmailInfo>.Save(emailInfo);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool SendEmailAfterAdhocAmendment(InhouseUserInfo user, AdHocAccessRequest adHocAccessRequest)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();
                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("endpointUserId", user.Id.ToString());
                para.Add("endpointSystemId", user.EndPointSystemId.ToString());
                AccountReconcilation useracc = DomainApiClient<Dto.AccountReconcilation>.GetByAction<AccountReconcilation>
                    (DomainApiAction.Provisioning.AccountReconcilation.GetByUser, para);
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", useracc.UserId.ToString());
                User m_user = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                Dictionary<string, string> eparameter = new Dictionary<string, string>();
                eparameter.Add("templateName", Constants.AdhocAmendment.ToString());
                MailTemplate template = DomainApiClient<Dto.MailTemplate>.GetByAction<MailTemplate>(DomainApiAction.Email.MailTemplate.GetByTemplateName, eparameter);
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                DI.Chakra.Dto.AccessRequest accessRequest = new DI.Chakra.Dto.AccessRequest();
                if (adHocAccessRequest.Id > 0)
                {
                    Dictionary<string, string> aparameter = new Dictionary<string, string>();
                    aparameter.Add("adhocAccessRequestId", adHocAccessRequest.Id.ToString());
                    accessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>
                        (DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, aparameter);
                }
                //entitlement = entitlement.Replace(",", "<br/>");
                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                //subject.Replace("@@applicationName@@", ddlApplicaionName.SelectedItem.Text);
                subject.Replace("@@userLogonName@@", user.UserName);
                subject.Replace("@@SYSTEM@@", adHocAccessRequest.EndpointSystemType.ToString());
                subject.Replace("@@REQUESTTYPE@@", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AdhocActionType));
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@USERID@@", string.IsNullOrEmpty(user.UserName) ? m_user.EmployeeId : user.UserName);

                // Body
                body.Replace("@@USER@@", m_user.DisplayName);
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                //body.Replace("@@applicationName@@", ddlApplicaionName.SelectedItem.Text);
                body.Replace("@@REQUESTTYPE@@", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AdhocActionType));
                body.Replace("@@EMPLOYEEID@@", string.IsNullOrEmpty(user.UserName) ? m_user.EmployeeId : user.UserName);
                body.Replace("@@userLogonName@@", user.UserName);
                string requestDetail = adHocAccessRequest.Entitlement;
                body.Replace("@@accessName@@", requestDetail);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Format("{0}", user.Email);
                emailInfo.Cc = string.Format("{0}", MasterConfigHelper.AccountCreatedNotificationTO);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                DomainApiClient<Dto.EmailInfo>.Save(emailInfo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void BindLockScenario()
        {
            var itemValues = System.Enum.GetValues(typeof(LockScenario));
            Dictionary<int, string> dic = new Dictionary<int, string>();
            List<LockScenario> list = new List<LockScenario>();
            foreach (var str in itemValues)
            {
                int value = Convert.ToInt32(str);
                ListItem lstitem = new ListItem(str.ToString(), value.ToString());
                ddlLockScenario.Items.Add(lstitem);
            }
        }

        private void BindAccountType()
        {
            TypeofAccounts.DataValueField = "Id";
            TypeofAccounts.DataTextField = "AccountTypeDesc";
            TypeofAccounts.DataSource = DomainApiClient<Dto.AccountCategory>.GetByAction<List<AccountCategory>>
                    (DomainApiAction.MetaData.AccountCategory.GetByIsActive, null);
            TypeofAccounts.DataBind();
            TypeofAccounts.SelectedItem.Text = UtilityMethods.GetEnumAttribute<Description>(Task.AccessRequest.AdHocAccessRequest.AccountType);
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Page.IsPostBack)
            //{
            //    txtINHPassword.Attributes.Add("value", this.UserPassword);
            //    txtINHPassword.Text = this.UserPassword;
            //}

            if (!this.Visible)
                return;

            if (!Page.IsPostBack)
            {
                BindLockScenario();
                _sessionStatus = new SessionStatusImpl();
                BindAccountType();
                DatePickerAccCreated.SelectedDate = DateTime.Now;
                var userId = Task.AccessRequest.UserId;
                Session["endpointSystemId"] = SelectedEndpoint.Id;
                //BindUserInfo(userId);
                if (Task.AccessRequest.AdHocAccessRequest != null && ActionType != AdHocActionType.DeleteAccount)
                {
                    divEntitlements.Visible = true;
                    RebindPermissionGrid();
                    BindFunctions();
                    if (Task.IsCompleted)
                    {
                        //AddPermissionButton.Enabled = false;
                        RequestGrid.Enabled = false;
                        //RolesAutoCompleteBox.Enabled = false;
                        //ddlFunctions.Enabled = false;
                    }
                }
            }
        }

        void BindFunctions()
        {
            if (SelectedEndpoint.IsFunctionLevel && !SelectedEndpoint.CanAutomate)
            {
                //divFunctions.Visible = true;
                ShowFunctionColumn(true);
            }
            else
            {
                //divFunctions.Visible = false;
                ShowFunctionColumn(false);
            }
        }
        protected void IsLockedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLockedCheckBox.Checked)
            {
                DatePickerLockedDate.SelectedDate = DateTime.Now;
                ddlLockScenario.Enabled = true;
            }
            else
            {
                DatePickerLockedDate.Clear();
                ddlLockScenario.Enabled = false;
            }
        }

        protected void IsDisabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (IsDisabledCheckBox.Checked)
                DatePickerIsDisabled.SelectedDate = DateTime.Now;
            else
                DatePickerIsDisabled.Clear();
        }

        protected void IsDeletedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (IsDeletedCheckBox.Checked)
                DatePickerIsDeleted.SelectedDate = DateTime.Now;
            else
                DatePickerIsDeleted.Clear();
        }

        #endregion
    }
}