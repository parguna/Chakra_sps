using System;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.Account.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Members
        private const string V_ID = "ENTITY_ID";
        private const string V_ACCOUNTDETAIL = "ACCOUNTDETAIL";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
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
                SetPage();
            }
        }

        public Dto.AccountReconcilation AccountReconcilation
        {
            get
            {
                if (ViewState[V_ACCOUNTDETAIL] == null)
                    return null;
                return (DI.Chakra.Dto.AccountReconcilation)ViewState[V_ACCOUNTDETAIL];
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
        #endregion

        #region Methods
        private void SetPage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", this.ContextObjectId.Value.ToString());
            AccountReconcilation = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>
                (DomainApiAction.Provisioning.AccountReconcilation.GetByIdIncludingEndPoint, parameter);

            AccountName.Text = AccountReconcilation.AccountName;
            DisplayName.Text = AccountReconcilation.FullName;
            if (AccountReconcilation.UserId.HasValue)
            {
                parameter.Clear();
                parameter.Add("id", AccountReconcilation.UserId.Value.ToString());
                MapGlobalUser.Checked = true;
                UserSearchUC.SelectedUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
            }
            else
            {
                UnmapGlobalUser.Checked = true;
                UserSearchUC.Disabled = true;
            }
            TypeofAccounts.SelectedValue = Convert.ToString((byte)AccountReconcilation.AccountType);
            if (AccountReconcilation.EndPoint.EndPointSystemType.EndpointSystemTypeName != EndpointType.SAP)
            {
                AccessListUC.Visible = true;
                AccessListUC.EntityId = EntityId;
                AccessListUC.ApplicationType = AccountReconcilation.EndPoint.EndPointSystemType.EndpointSystemTypeName;
            }
            else
            {
                SAPAccessListUC.Visible = true;
                SAPAccessListUC.EntityId = EntityId;
                SAPAccessListUC.ApplicationType = AccountReconcilation.EndPoint.EndPointSystemType.EndpointSystemTypeName;
            }
            ownerOffboard();

            int offboardingAction = (int)AccountReconcilation.OffboardingAccountAction;
            if (offboardingAction != 0)
            {
                ddlOwnerOffboard.SelectedIndex = offboardingAction;
            }

            PrimaryAccountManagement();
        }

        private void BindAccountType()
        {
            TypeofAccounts.DataValueField = "Id";
            TypeofAccounts.DataTextField = "AccountTypeDesc";
            TypeofAccounts.DataSource = DomainApiClient<Dto.AccountCategory>.GetByAction<List<AccountCategory>>
                (DomainApiAction.MetaData.AccountCategory.GetByIsActive, null);
            TypeofAccounts.DataBind();
        }

        private void BindOffboardingAccountAction()
        {
            ddlOwnerOffboard.DataValueField = "Id";
            ddlOwnerOffboard.DataTextField = "Desc";
            ddlOwnerOffboard.DataSource = DomainApiClient<Dto.OffboardingAccountAction>.GetAll();
            ddlOwnerOffboard.DataBind();
        }

        private void ownerOffboard()
        {
            ddlOwnerOffboard.SelectedIndex = 0;
            ddlOwnerOffboard.Enabled = true;
            if (TypeofAccounts.SelectedValue == string.Empty)
                return;

            if (Convert.ToByte(TypeofAccounts.SelectedValue) == (byte)AccountType.Shared ||
           Convert.ToByte(TypeofAccounts.SelectedValue) == (byte)AccountType.Service)
            {
                ddlOwnerOffboard.SelectedIndex = 1;
            }
            else
            {
                ddlOwnerOffboard.SelectedIndex = 2;
            }
        }

        private void PrimaryAccountManagement()
        {
            chkPrimaryAccount.Enabled = false;

            if (TypeofAccounts.SelectedValue == string.Empty)
                return;

            if (AccountReconcilation.EndPoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.ActiveDirectory &&
                Convert.ToByte(TypeofAccounts.SelectedValue) == (byte)AccountType.User)
            {
                chkPrimaryAccount.Enabled = true;
            }
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindAccountType();
                BindOffboardingAccountAction();

                if (ContextObjectId != null)
                {
                    EntityId = Convert.ToInt64(ContextObjectId);
                }
            }

            if (UIMode == UIMode.View)
                ContentWrapper.Enabled = false;
            this.MasterPage.ActionClicked += MasterPage_ActionClicked;
        }

        void MasterPage_ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
                Save();
        }
        private void Save()
        {
            bool userHasChanged = false;

            AccountReconcilation.AccountType = (AccountType)Convert.ToInt32(TypeofAccounts.SelectedValue);


            if (MapGlobalUser.Checked)
            {
                if (UserSearchUC.SelectedUser != null)
                {
                    // if the already existing user and currently selected user are the same, 
                    // set the flag to indicate that the user has changed
                    userHasChanged = (AccountReconcilation.UserId != UserSearchUC.SelectedUser.Id);
                    if(AccountReconcilation.AccountType == AccountType.User)
                    {
                        AccountReconcilation.FullName = UserSearchUC.SelectedUser.DisplayName;
                    }
                    AccountReconcilation.UserId = UserSearchUC.SelectedUser.Id;
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("endpointSystemId", AccountReconcilation.EndpointSystemId.ToString());
                    parameter.Add("reconcilationId", AccountReconcilation.Id.ToString());
                    var entitlementReconciles = DomainApiClient<EntitlementReconcilation>.GetByAction<List<EntitlementReconcilation>>
                                (DomainApiAction.Provisioning.EntitlementReconcilation.GetByReconcilationId, parameter);
                    foreach (var item in entitlementReconciles)
                    {
                        item.UserId = UserSearchUC.SelectedUser.Id;
                        DomainApiClient<Dto.EntitlementReconcilation>.Save(item);
                    }
                }
                else
                {
                    ShowAlertMessage(AlertType.Error, "Please select a user to map to.");
                    return;
                }
            }
            else
            {
                AccountReconcilation.UserId = null;
            }

            // if no user has been mapped change MappingType to None instead of Manual
            if (AccountReconcilation.UserId == null)
            {
                AccountReconcilation.MappingType = MappingType.None;
            }
            // if the user mapping has changed
            else if (userHasChanged)
            {
                AccountReconcilation.MappingType = MappingType.Manual;
            }

            AccountReconcilation.OffboardingAccountAction = (OffboardingAccountActionType)Enum.Parse(typeof(OffboardingAccountActionType),
                ddlOwnerOffboard.SelectedValue);

            if (AccountReconcilation.UserId != null && AccountReconcilation.AccountType == AccountType.User && AccountReconcilation.EndPoint != null && AccountReconcilation.EndPoint.ApplicationType == ApplicationType.ADSingaporePowerLocal)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", Convert.ToString(AccountReconcilation.UserId));
                parameter.Add("sAMAccountName", AccountReconcilation.AccountName.ToString());
                parameter.Add("endpointsystemId", AccountReconcilation.EndpointSystemId.ToString());
                DomainApiClient<Dto.User>.GetByAction<object>(DomainApiAction.User.UpdateUserADAccount, parameter);
            }


            DomainApiClient<Dto.AccountReconcilation>.Save(AccountReconcilation);


            DomainApiClient<AccountReconcilation>.GetByAction<object>
                (DomainApiAction.Provisioning.AccountReconcilation.ProcessAccountMapping, new Dictionary<string, string> { { "AccountReconcilationId", AccountReconcilation.Id.ToString() } });


            Dictionary<string, string> entparameter = new Dictionary<string, string>();
            entparameter.Add("accountReconcilationId", AccountReconcilation.Id.ToString());
            EntitlementReconcilation entitlementReconcile = DomainApiClient<EntitlementReconcilation>.GetByAction<EntitlementReconcilation>
                (DomainApiAction.Provisioning.EntitlementReconcilation.GetByAccountReconcilationId, entparameter);
            if (entitlementReconcile != null)
            {
                entitlementReconcile.UserId = AccountReconcilation.UserId;
                DomainApiClient<Dto.EntitlementReconcilation>.Save(entitlementReconcile);
            }
            this.ContextObjectId = null;
            Response.Redirect("List.aspx");
        }

        protected void Map_CheckedChanged(object sender, EventArgs e)
        {
            UserSearchUC.Disabled = UnmapGlobalUser.Checked;
        }

        protected void TypeofAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            ownerOffboard();
            PrimaryAccountManagement();
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("List.aspx");
        }
        #endregion
    }
}