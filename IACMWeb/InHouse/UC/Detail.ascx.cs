using System;
using System.Linq;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using Application;

namespace DI.Chakra.Web.InHouse.UC
{
    public partial class Detail : UIBaseUC
    {
        #region Members

        private const string V_INHUSER = "INHUSER";
        private const string V_EntityID = "V_EntityID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_ACCOUNTDETAIL = "ACCOUNTDETAIL";
        public string txtPassword = "txtPassword";
        private const string V_SELECTEDPERMISSIONS = "SELECTEDPERMISSIONS";

        #endregion

        #region Properties

        public string TxtPassword
        {
            get
            {
                if (ViewState[txtPassword] == null)
                    return string.Empty;
                return (string)ViewState[txtPassword];
            }
            set
            {
                ViewState[txtPassword] = value;
            }
        }

        public long EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return -1;
                return (long)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
                SetPage();
            }
        }

        public InhouseUserInfo INHUser
        {
            get
            {
                if (ViewState[V_INHUSER] == null)
                    return new InhouseUserInfo();
                return (InhouseUserInfo)ViewState[V_INHUSER];
            }
            set
            {
                ViewState[V_INHUSER] = value;
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

        public Dictionary<long, string> SelectedPermissions
        {
            get
            {
                if (ViewState[V_SELECTEDPERMISSIONS] != null)
                {
                    return (Dictionary<long, string>)ViewState[V_SELECTEDPERMISSIONS];
                }
                return new Dictionary<long, string>();
            }
            set
            {
                ViewState[V_SELECTEDPERMISSIONS] = value;
            }
        }

        #endregion

        #region Methods
        private void SetPage()
        {
            BindLockScenario();
            BindAccountType();
            EndpointUC.Initialize();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", EntityId.ToString());
            AccountReconcilation = DomainApiClient<Dto.AccountReconcilation>.GetByAction<AccountReconcilation>
                (DomainApiAction.Provisioning.AccountReconcilation.GetByIdIncludingEndPoint, parameter);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", (AccountReconcilation != null ? AccountReconcilation.EndpointUserId : 0).ToString());
            List<InhouseUserInfo> list = DomainApiClient<Dto.InhouseUserInfo>.GetByAction<List<InhouseUserInfo>>
                (DomainApiAction.Endpoint.InhouseSystems.InhouseUserInfo.GetByIdIncludeEndPoint, param);

            if (list.Count == 0)
            {
                INHUser = new InhouseUserInfo();
                return;
            }
            txtINHPassword.Enabled = false;
            INHUser = list[0];
            EndpointUC.SelectedEndpoint = INHUser.EndPoint;
            txtIdentificationNo.Text = INHUser.NRIC;
            Session["InhouseSystemEndpointId"] = INHUser.EndPointSystemId;
            txtEmail.Text = INHUser.Email;
            txtUserName.Text = INHUser.UserName;
            txtFirstName.Text = INHUser.FirstName;
            txtLastName.Text = INHUser.LastName;
            txtDisplayName.Text = INHUser.DisplayName;
            TxtPassword = RSAHelper.DecryptData(INHUser.Password);
            txtINHPassword.Attributes.Add("value", TxtPassword);            
            TypeofAccounts.SelectedValue = Convert.ToString((byte)AccountReconcilation.AccountType);

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("userId", INHUser.Id.ToString());

            var permissions = INHUser.Permissions;
            if (permissions != null && permissions.Count > 0)
            {
                foreach (var permission in permissions)
                {
                    AddPermission(permission.Id, permission.Name);
                }
            }

            Dictionary<string, string> param1 = new Dictionary<string, string>();
            param1.Add("endpointUserId", INHUser.Id.ToString());
            param1.Add("endpointSystemId", INHUser.EndPointSystemId.ToString());
            AccountReconcilation useracc = DomainApiClient<Dto.AccountReconcilation>.GetByAction<AccountReconcilation>
                (DomainApiAction.Provisioning.AccountReconcilation.GetByUser, param1);
            User accountowner = useracc.UserId !=null ? DomainApiClient<Dto.User>.GetById((long)useracc.UserId) :null;
            if (accountowner != null)
                this.UserSearchUC.SelectedUser = accountowner;
            IsLockedCheckBox.Checked = useracc.IsLocked;
            IsDisabledCheckBox.Checked = INHUser.IsDisabled;
            IsDeletedCheckBox.Checked = INHUser.IsDeleted;
            ddlLockScenario.SelectedValue = Convert.ToString(Convert.ToInt32(useracc.LockScenario));
            txtRemarks1.Text = INHUser.Remarks1;
            txtRemarks2.Text = INHUser.Remarks2;
            txtRemarks3.Text = INHUser.Remarks3;
            txtRemarks4.Text = INHUser.Remarks4;
            txtRemarks5.Text = INHUser.Remarks5;
        }

        public void ShowAlertMessage(DI.Chakra.Infrastructure.AlertType AlertType, string ErrorMessage)
        {
            MasterPage.ShowAlert(AlertType, ErrorMessage);
        }

        private bool Validate()
        {
            bool flag;
            flag = true;
            if (EndpointUC.SelectedEndpoint == null)
            {
                this.MasterPage.ShowAlert(AlertType.Error, string.Format("Please Select {0}.", T("App.Common.Endpoint")));
                flag = false;
            }

            string MandatoryMessage = string.Empty;
            if (UIMode == UIMode.Add)
            {
                TextBox[] MandatoryFields = { txtDisplayName, txtUserName, txtINHPassword };
                MandatoryMessage = MandatoryValidationmessage(MandatoryFields);
            }
            if (UIMode == UIMode.Edit)
            {
                TextBox[] MandatoryFields = { txtDisplayName, txtUserName };
                MandatoryMessage = MandatoryValidationmessage(MandatoryFields);
            }
            if (!String.IsNullOrEmpty(MandatoryMessage))
            {
                this.MasterPage.ShowAlert(AlertType.Warning, MandatoryMessage);
                flag = false;
            }
            if ((UIMode == UIMode.Add) && (DoesExist(txtUserName.Text.Trim())))
            {
                MasterPage.ShowAlert(AlertType.Error, "User Name Already Exists for this EndPointSystem");
                flag = false;
            }
            else if (UIMode == UIMode.Edit)
            {
                var dbInhUser = DomainApiClient<Dto.InhouseUserInfo>.GetById(EntityId);
                if (dbInhUser != null)
                {
                    if ((dbInhUser.UserName != txtUserName.Text.Trim()) && (dbInhUser.EndPointSystemId != INHUser.EndPointSystemId))
                    {
                        if (DoesExist(txtUserName.Text.Trim()))
                        {
                            MasterPage.ShowAlert(AlertType.Error, "User Name Already Exists for this EndPointSystem");
                            flag = false;
                        }
                    }
                }
            }

            return flag;
        }

        private void Save()
        {
            try
            {
                if (Validate())
                {
                    long userId = 0;
                    string Permission = string.Empty;

                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("endpointUserId", INHUser.Id.ToString());
                    param.Add("endpointSystemId", INHUser.EndPointSystemId.ToString());
                    AccountReconcilation accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<AccountReconcilation>
                        (DomainApiAction.Provisioning.AccountReconcilation.GetByUser, param);
                    INHUser.EndPointSystemId = EndpointUC.SelectedEndpoint.Id;
                    INHUser.NRIC = txtIdentificationNo.Text.Trim();
                    INHUser.InhouseSystemCode = EndpointUC.SelectedEndpoint.ApplicationCode.Trim();
                    INHUser.Email = txtEmail.Text.Trim();
                    INHUser.UserName = txtUserName.Text.Trim();
                    INHUser.AccountName = txtUserName.Text.Trim();
                    INHUser.DisplayName = txtDisplayName.Text.Trim();
                    INHUser.FirstName = txtFirstName.Text.Trim();
                    INHUser.LastName = txtLastName.Text.Trim();
                    INHUser.Remarks1 = txtRemarks1.Text.Trim();
                    INHUser.Remarks2 = txtRemarks2.Text.Trim();
                    INHUser.Remarks3 = txtRemarks3.Text.Trim();
                    INHUser.Remarks4 = txtRemarks4.Text.Trim();
                    INHUser.Remarks5 = txtRemarks5.Text.Trim();

                    if (accountRecon != null)
                    {
                        accountRecon.AccountType = (AccountType)Convert.ToInt32(TypeofAccounts.SelectedValue);
                        accountRecon.LockScenario = (LockScenario)Convert.ToInt32(ddlLockScenario.SelectedItem.Value);
                        accountRecon.IsLocked = IsLockedCheckBox.Checked;
                        if (accountRecon.IsLocked)
                            INHUser.LockedDate = DateTime.Now;
                    }
                    if (INHUser.Id == 0)
                    {
                        INHUser.Password = String.IsNullOrEmpty(txtINHPassword.Text.Trim()) ? TxtPassword : RSAHelper.EncryptData(txtINHPassword.Text.Trim());
                        INHUser.AccountCreatedDate = DateTime.Now;
                    }

                    INHUser.IsDisabled = IsDisabledCheckBox.Checked;
                    if (INHUser.IsDisabled)
                        INHUser.DisabledDate = DateTime.Now;
                    INHUser.IsDeleted = IsDeletedCheckBox.Checked;
                    if (INHUser.IsDisabled)
                        INHUser.DeletedDate = DateTime.Now;
                    if (UserSearchUC.SelectedUser != null)
                        userId = UserSearchUC.SelectedUser.Id;

                    List<InhousePermission> permissionList = new List<InhousePermission>(); 
                    if (this.SelectedPermissions.Count > 0)
                    {
                        foreach (var permission in SelectedPermissions)
                        {                      
                            InhousePermission objPer = DomainApiClient<Dto.InhousePermission>.GetById(permission.Key);
                            permissionList.Add(objPer);
                        }
                    }

                    INHUser.Permissions = permissionList;
                    DomainApiClient<Dto.InhouseUserInfo>.Save(INHUser);
                    EndPoint endpointobj = DomainApiClient<EndPoint>.GetById(INHUser.EndPointSystemId);
                    param.Clear();
                    param.Add("userName", txtUserName.Text);
                    param.Add("inhousesystemcode", EndpointUC.SelectedEndpoint.ApplicationCode);
                    List<InhouseUserInfo> users = DomainApiClient<Dto.InhouseUserInfo>.GetByAction<List<InhouseUserInfo>>
                        ("GetByUserName", param);
                    if (users.Count > 0)
                        INHUser.Id = users[0].Id;

                    param.Clear();
                    param.Add("isNewlyAdded", (UIMode == UIMode.Add ? true : false).ToString());
                    param.Add("accountTypeId", TypeofAccounts.SelectedValue.ToString());
                    param.Add("mappingType", userId > 0 ? MappingType.Manual.ToString() : MappingType.None.ToString());
                    param.Add("userId", userId.ToString());
                    param.Add("fromAccessRequest", false.ToString());
                    param.Add("accessRequestId", "0");
                    DomainApiClient<Dto.AccountReconcilation>.GetByAction<Object>
                        (DomainApiAction.Provisioning.AccountReconcilation.AddOrUpdateAccountReconcilation, param, INHUser);

                    param.Clear();
                    param.Add("endPoint", EndpointUC.SelectedEndpoint.ToString());
                    DomainApiClient<Dto.InhouseUserInfo>.GetByAction<Object>
                        (DomainApiAction.Endpoint.InhouseSystems.InhouseUserInfo.MapUser, param, EndpointUC.SelectedEndpoint);

                    if ((UIMode == UIMode.Add))
                    {
                        param.Clear();
                        param.Add("inHouseUserId", INHUser.Id.ToString());
                        DomainApiClient<Dto.InhouseUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.InhouseSystems.InhouseUserInfo.EmailCompose, param);
                        MasterPage.ShowAlert(AlertType.Success, "Record Saved Successfully");
                    }
                    if (((UIMode == UIMode.Edit)))
                    {
                        param.Clear();
                        param.Add("inHouseUserId", INHUser.Id.ToString());
                        DomainApiClient<Dto.InhouseUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.InhouseSystems.InhouseUserInfo.EmailCompose, param);
                        MasterPage.ShowAlert(AlertType.Success, "Record Updated Successfully");
                    }
                    Response.Redirect("~/Inhouse/List.aspx");
                }
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "Error while saving");
            }
        }

        private bool DoesExist(string name)
        {
            Dto.InhouseUserInfo group = new Dto.InhouseUserInfo();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointSystemId", Session["InhouseSystemEndpointId"].ToString());
            parameter.Add("userName", txtUserName.Text.Trim());
            return DomainApiClient<Dto.InhouseUserInfo>.GetByAction<bool>
                (DomainApiAction.Endpoint.InhouseSystems.InhouseUserInfo.DoesExists, parameter);
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

        private void BindAccountType()
        {
            TypeofAccounts.DataValueField = "Id";
            TypeofAccounts.DataTextField = "AccountTypeDesc";
            TypeofAccounts.DataSource = DomainApiClient<Dto.AccountCategory>.GetByAction <List<AccountCategory>>
                    (DomainApiAction.MetaData.AccountCategory.GetByIsActive, null);
            TypeofAccounts.DataBind();
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
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            this.txtINHPassword.Attributes.Add("value", this.txtINHPassword.Text);
            if (!IsPostBack)
            {
                EntityId =  ContextObjectId != null ? Convert.ToInt64(ContextObjectId) : 0;
                EndpointUC.SelectedApplication= new EndPointSystemType { EndpointSystemTypeName = EndpointType.InhouseSystems };

                if (EndpointUC.SelectedApplication==null)
                {
                    EndpointUC.DisabledApplicationType = true;
                    ShowAlertMessage(AlertType.Error, "Please configure the Inhouse EndpointSystem Type");
                    return;
                }

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", EndpointUC.SelectedApplication.Id.ToString());
                var Endpoint = DomainApiClient<Dto.EndPointSystemType>.GetByAction <EndPointSystemType>
                    (DomainApiAction.Endpoint.EndPointSystemType.GetByIdIncludingEndpoints, parameter);

                if (Endpoint == null || Endpoint.EndPoints.Count == 0)
                {
                    ShowAlertMessage(AlertType.Error, "First Create InHouse Endpoint");
                    ContentWrapper.Enabled = false;
                }
                else
                {
                    EndpointUC.DisabledApplicationType = true;                    
                    if (UIMode == UIMode.View)
                        ContentWrapper.Enabled = false;
                }
                ddlLockScenario.Enabled = false;
                if((UIMode==UIMode.Edit)||(UIMode == UIMode.View))
                {
                    divPassword.Attributes.Add("class", divPassword.Attributes["class"].ToString().
                        Replace("form-group required", "form-group"));
                }
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Inouse Application - Save");
            }
        }

        protected void AddPermissionButton_Click(object sender, EventArgs e)
        {
            foreach (AutoCompleteBoxEntry item in InhousePermissionAutoCompleteBox.Entries)
            {
                bool loadEpHierrchy = false;
                var funId = Convert.ToInt64(item.Value);
                var fns = this.SelectedPermissions;
                if (!fns.ContainsKey(funId))
                {
                    loadEpHierrchy = true;
                }

                AddPermission(funId, item.Text);
            }
            this.RebindPermissionGrid();
        }

        private void RebindPermissionGrid()
        {
            this.Permission_FormGrid.DataSource = this.SelectedPermissions;
            this.Permission_FormGrid.Rebind();
            this.InhousePermissionAutoCompleteBox.Entries.Clear();
            Permission_FormGrid.Enabled = true;
            if (Permission_FormGrid.Items.Count > 0)
            {
                Permission_FormGrid.Visible = true;
            }
            else
            {
                Permission_FormGrid.Visible = false;
            }
        }

        protected void Permission_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long epId = (long)Convert.ChangeType(item.GetDataKeyValue("Key"), typeof(long));
                var fns = this.SelectedPermissions;
                fns.Remove(epId);
                this.SelectedPermissions = fns;
                this.RebindPermissionGrid();
            }
        }

        protected void Permission_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                if ((UIMode == UIMode.View) | (UIMode == UIMode.Edit))
                {
                    //(item["DeleteColumn"].Controls[0] as ImageButton).Enabled = false;
                }
                else
                {
                    //(item["DeleteColumn"].Controls[0] as ImageButton).Enabled = true;
                }
            }
        }

        protected void Permission_FormGrid_PreRender(object sender, EventArgs e)
        {
            foreach (GridColumn col in Permission_FormGrid.Columns)
            {
                if (col.UniqueName == "FunctionName")
                    col.HeaderText = T("App.Common.INHPermission");
            }
            this.Permission_FormGrid.DataSource = this.SelectedPermissions;
            Permission_FormGrid.Rebind();
        }

        private void AddPermission(long funId, string hostName)
        {
            var fns = this.SelectedPermissions;
            if (!fns.ContainsKey(funId))
            {
                fns.Add(funId, hostName);

            }
            this.SelectedPermissions = fns;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~//Inhouse//List.aspx");
        }

        protected void EndpointUC_EndpointChanged(EndPoint EndPoint)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endpointSystemId", EndPoint.Id.ToString());
            EndpointConfiguration epConfig = DomainApiClient<EndpointConfiguration>.GetByAction<EndpointConfiguration>(DomainApiAction.Endpoint.EndPoint.Configuration.EndpointConfiguration.GetByEndpointSystemId, parameter);
            var config = epConfig.ConfigurationDetails.Where(lq => lq.KeyId == Convert.ToInt64(EndpointKey.CanRequestMultipleRoles)).Select(select => select.Value).FirstOrDefault();
            bool IsMultipeRole = config != null ? Convert.ToBoolean(config) : false;

            InhousePermissionAutoCompleteBox.TextSettings.SelectionMode = IsMultipeRole ? RadAutoCompleteSelectionMode.Multiple
                : RadAutoCompleteSelectionMode.Single;
            Session["InhouseSystemEndpointId"] = EndPoint.Id;
            InhousePermissionAutoCompleteBox.Entries.Clear();
        }
        #endregion

        protected void IsLockedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLockedCheckBox.Checked)
                ddlLockScenario.Enabled = true;
            else
                ddlLockScenario.Enabled = false;
        }

        private string MandatoryValidationmessage(TextBox[] items)
        {
            string message = string.Empty;
            foreach(TextBox item in items)
            {
                switch(item.ID)
                {
                    case "txtUserName":
                        message = String.IsNullOrEmpty(txtUserName.Text) ?message+"User Name,  " : message ;
                        break;
                    case "txtINHPassword":
                        message= String.IsNullOrEmpty(txtINHPassword.Text)&& String.IsNullOrEmpty(TxtPassword) ?  message + "Password, ": message;
                        break;
                    case "txtDisplayName":
                        message = String.IsNullOrEmpty(txtDisplayName.Text) ?  message + "Display Name, ": message;
                        break;
                }
            }
            return !String.IsNullOrEmpty(message) ? message + " should not be empty" : String.Empty;
        }
    }
}