
namespace DI.Chakra.Web.Account
{
    public partial class Edit : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        //#region Members
        //private const string V_ID = "ENTITY_ID";
        //private const string V_APPLICATIONTYPE = "APPLICATIONTYPE";
        //private const string V_ACCOUNTDETAIL = "ACCOUNTDETAIL";
        //private UserFacade UserFacade = new UserFacade();
        //private UserAccountFacade UserAccountFacade = new UserAccountFacade();
        //private MasterFacade masterFacade = new MasterFacade();
        //#endregion

        //#region Properties
        //public int EntityId
        //{
        //    get
        //    {
        //        if (ViewState[V_ID] == null)
        //            return -1;
        //        return (int)ViewState[V_ID];
        //    }
        //    set
        //    {
        //        ViewState[V_ID] = value;
        //    }
        //}
        //public EndpointType ApplicationType
        //{
        //    get
        //    {
        //        if (ViewState[V_APPLICATIONTYPE] == null)
        //            return EndpointType.None;
        //        return (EndpointType)ViewState[V_APPLICATIONTYPE];
        //    }
        //    set
        //    {
        //        ViewState[V_APPLICATIONTYPE] = value;
        //        SetPage();
        //    }
        //}
        //public DI.Chakra.Model.AccountDetail AccountDetail
        //{
        //    get
        //    {
        //        if (ViewState[V_ACCOUNTDETAIL] == null)
        //            return null;
        //        return (DI.Chakra.Model.AccountDetail)ViewState[V_ACCOUNTDETAIL];
        //    }
        //    set
        //    {
        //        ViewState[V_ACCOUNTDETAIL] = value;
        //    }
        //}
        //#endregion

        //#region Methods
        //private void SetPage()
        //{
        //    AccountDetail = UserAccountFacade.GetAccountDetailByEndPointUserID(EntityId, ApplicationType);
        //    IList<UserAccount> UserAccount = UserAccountFacade.GetUserAccountDetailByApplication(AccountDetail.EndpointSystemId);
        //    AccountName.Text = UserAccount[0].UserName;
        //    DisplayName.Text = UserAccount[0].FullName;
        //    if (AccountDetail.IsMappedAccount)
        //    {
        //        MapGlobalUser.Checked = true;
        //        if (AccountDetail.DIUserID.HasValue)
        //            UserSearchUC.SelectedUser = UserFacade.GetUser(AccountDetail.DIUserID.Value);
        //    }
        //    else
        //    {
        //        UnmapGlobalUser.Checked = true;
        //        UserSearchUC.Disabled = UnmapGlobalUser.Checked;
        //    }

        //    TypeofAccount.SelectedValue = UtilityMethods.GetEnumAttribute<Description>(AccountDetail.AccountType);

        //    if (ApplicationType != EndpointType.SAP)
        //    {
        //        AccessListUC.Visible = true;
        //        AccessListUC.EntityId = EntityId;
        //        AccessListUC.ApplicationType = ApplicationType;
        //    }
        //    else
        //    {
        //        SAPAccessListUC.Visible = true;
        //        SAPAccessListUC.EntityId = EntityId;
        //        SAPAccessListUC.ApplicationType = ApplicationType;
        //    }
        //    ownerOffboard();
        //    PrimaryAccountManagement();
        //}
        //private void BindAccountType()
        //{
        //    TypeofAccount.DataValueField = "AccountTypeId";
        //    TypeofAccount.DataTextField = "AccountTypeDesc";
        //    TypeofAccount.DataSource = masterFacade.GetAccountCategories();
        //    TypeofAccount.DataBind();
        //}
        //private void ownerOffboard()
        //{
        //    ddlOwnerOffboard.SelectedIndex = 0;
        //    ddlOwnerOffboard.Enabled = false;


        //    if (TypeofAccount.SelectedValue == string.Empty)
        //        return;

        //        if (Convert.ToByte(TypeofAccount.SelectedValue) == (byte)AccountType.Shared ||
        //       Convert.ToByte(TypeofAccount.SelectedValue) == (byte)AccountType.Service)
        //    {
        //        ddlOwnerOffboard.Enabled = true;
        //        ddlOwnerOffboard.SelectedIndex = 1;
        //    }
        //}

        //private void PrimaryAccountManagement()
        //{
        //    chkPrimaryAccount.Enabled = false;

        //    if (TypeofAccount.SelectedValue == string.Empty)
        //        return;

        //    if (ApplicationType == EndpointType.ActiveDirectory &&
        //        Convert.ToByte(TypeofAccount.SelectedValue) == (byte)AccountType.User)
        //    {
        //        chkPrimaryAccount.Enabled = true;
        //    }

           
        //}
        //#endregion

        //#region Events
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        BindAccountType();
        //    }
        //    if (!IsPostBack && ContextObjectId != null)
        //    {
        //        EntityId = Convert.ToInt32(ContextObjectId);
        //        ApplicationType = this.EndpointType;

        //    }

        //    this.MasterPage.ActionClicked += MasterPage_ActionClicked;
        //}

        //void MasterPage_ActionClicked(object sender, AccessActionEventArgs e)
        //{
        //    if(e.ActionType == ACLAction.Save)
        //        Save();
        //}
        //private void Save()
        //{
        //    AccountDetail.IsPrimaryAccount = chkPrimaryAccount.Checked;
        //    AccountDetail.UnMappedMapTo = ddlOwnerOffboard.SelectedValue=="1" ? "HOD" : "";

        //    if (MapGlobalUser.Checked)
        //    {
        //        AccountDetail.DIUserID = UserSearchUC.SelectedUser.UserId;
        //    }
        //    else
        //    {
        //        AccountDetail.DIUserID = null;
        //    }

        //    //AccountDetail.AccountType = (byte)TypeofAccount.SelectedValue;

        //    UserAccountFacade.UpdateAccountDetail(AccountDetail, ApplicationType);
        //    Response.Redirect("List.aspx");
        //}

        //protected void Map_CheckedChanged(object sender, EventArgs e)
        //{
        //    UserSearchUC.Disabled = UnmapGlobalUser.Checked;
        //}
        //protected void TypeofAccount_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ownerOffboard();
        //    PrimaryAccountManagement();
        //}
        //protected void Cancel_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect("List.aspx");
        //}
        //#endregion

    }
}