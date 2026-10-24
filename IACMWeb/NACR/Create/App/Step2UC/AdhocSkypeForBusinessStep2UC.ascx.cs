using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocSkypeForBusinessStep2UC : UIBaseStep2UC
    {
        private const string moveToOnline = "Move To Online";
        private const string moveToOnPrem = "Move To OnPrem";
        public Dictionary<string, string> SelectedMembers
        {
            get
            {
                if (ViewState["V_SELECTEDMEMBERS"] != null)
                {
                    return (Dictionary<string, string>)ViewState["V_SELECTEDMEMBERS"];
                }
                return null;
            }
            set
            {
                ViewState["V_SELECTEDMEMBERS"] = value;
            }
        }
        public string EndpointUserId
        {
            get
            {
                if (string.IsNullOrEmpty(hiddenEndpointUser.Value))
                {
                    return "0";
                }
                return hiddenEndpointUser.Value;
            }
            set
            {
                hiddenEndpointUser.Value = value;
            }
        }
        public string EndpointSystemId
        {
            get
            {
                if (ViewState["EndpointId"] != null)
                {
                    return ViewState["EndpointId"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ViewState["EndpointId"] = value;
            }
        }
        public string AdhocActionType
        {
            get
            {
                if (ViewState["AdhocActionType"] != null)
                {
                    return ViewState["AdhocActionType"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ViewState["AdhocActionType"] = value;
            }
        }
        
        private List<ResponseGroup> ResponseGroups
        {
            get
            {
                if (ViewState["SFBResponseGroups"] != null)
                {
                    return (List<ResponseGroup>)ViewState["SFBResponseGroups"];
                }
                return new List<ResponseGroup>();
            }
            set
            {
                ViewState["SFBResponseGroups"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void afterSave()
        {
            ClearFields();
            ClearDefaultValue();
            ResponseGroups = new List<ResponseGroup>();
        }
        protected override void InitializeStep()
        {
            ClearFields();
            if (AdhocRequest != null)
            {
                Session["AdHocSFBInfo"] = null;
                pnlResponseGroup.Visible = false;
                ClearDefaultValue();
                var action = AdhocRequest.AdhocActionType;
                this.AdhocActionType = action.ToString();
                this.EndpointSystemId = AdhocRequest.EndpointSystemId.ToString();
                LoadControl();
                if (action == AdHocActionType.AmendmentRemove)
                {
                    divMask.Visible = false;
                    lblMask.Text = "Mask";
                    var column = ResponseGroupGrid.Columns.FindByUniqueName("Mask");
                    if (column != null)
                    {
                        column.Display = false;
                    }
                }
                else
                {
                    lblMask.Text = "Mask";
                    var column = ResponseGroupGrid.Columns.FindByUniqueName("Mask");
                    if (column != null)
                    {
                        column.Display = true;
                    }
                }
                if (action != AdHocActionType.NewAccount)
                {
                    divEndpointUser.Visible = true;
                    
                    BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                    if(ddlEndpointUser.SelectedIndex > 0)
                    {
                        this.EndpointUserId = ddlEndpointUser.SelectedValue;

                    }else
                    {
                        this.EndpointUserId = "0";
                    }
                    this.ddlEndpointUser.Enabled = true;
                    
                    if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
                    {
                        BindAdhocRequest();
                    }
                    else
                    {
                        FillApplicationGrid();
                    }
                }
                else
                {
                    if(action == AdHocActionType.NewAccount)
                    {
                        lblInfoMessage.Visible = true;
                        lblInfoMessage.Text = MasterConfigHelper.SFBNewAccountInformationMessage;
                        this.divEntTypeRequFieldIndicator.Attributes.Add("class", "form-group");
                        newAccountMessage.Visible = true;
                    }
                    var requestDetails = AdhocRequest.GetEntitlementDetail() as AdhocSkypeForBusinessRequest;
                    if (requestDetails != null)
                    {
                        this.ResponseGroups = requestDetails.SkypeForBusinessResponseGroups;
                        FillApplicationGrid();
                    }
                }
            }
        }
        private void BindAdhocRequest()
        {
            var Entitlement = (AdhocSkypeForBusinessRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (ddlEndpointUser.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    ddlEndpointUser.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);

                ddlEntitlementType.SelectedValue = Convert.ToString((int)Entitlement.EntitlementType);
                ddlEntitlementType_SelectedIndexChanged(this.ddlEntitlementType, EventArgs.Empty);
                if(Entitlement.EntitlementType == EntitlementType.DCC && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    SetDCCAmendmentAddMessage();
                    txtDCCExtension.Text = Entitlement.Extension;
                }
                else if (Entitlement.EntitlementType == EntitlementType.DCC && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    SetDCCAmendmentRemoveMessage();
                }
                else if(Entitlement.EntitlementType == EntitlementType.Move)
                {
                    if(!(Entitlement.MoveToOnline.HasValue && Entitlement.MoveToOnline.Value))
                    {
                        rdoOnPrem.Checked = true;
                        divRegistrarPool.Visible = true;
                        if (!string.IsNullOrWhiteSpace(Entitlement.RegistrarPoolName))
                            this.ddlRegistrarPool.SelectedValue = Entitlement.RegistrarPoolName;
                    }
                }
                if (Entitlement.SkypeForBusinessResponseGroups != null)
                    ResponseGroups = Entitlement.SkypeForBusinessResponseGroups;
                FillApplicationGrid();
            }
        }
        void LoadControl()
        {
            if (AdhocRequest != null)
            {
               // var accountRecon = BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                if (AdhocRequest.AccountType == AccountType.User)
                {
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                       
                    }
                    LoadEntitlementType();
                }
                
            }
        }
        void LoadEntitlementType()
        {
            var excludeDCCDCCS = MasterConfigHelper.ExcludeMSTeamsDCCDCCSPolicy;
            var hasResponseGroup = false;
            var userObject = GetEndPointUser();
            var param = new Dictionary<string, string>();
            var resGrp = new ListItem(EntitlementType.ResponseGroup.ToString(), ((int)EntitlementType.ResponseGroup).ToString());
            ddlEntitlementType.Items.Clear();
            ddlEntitlementType.Items.Add(new ListItem("-- Select --", ((int)EntitlementType.None).ToString()));
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                
                divLineURI.Visible = true;
                lblLineURI.Visible = true;
                lblLineURI.Text = userObject?.LineURI;
            }
            else
            {
                ddlEntitlementType.Items.Add(resGrp);
            }
            
            var telephone = new ListItem(EntitlementType.Default.ToString(), ((int)EntitlementType.Default).ToString());
            
            if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && userObject != null)
            {
                
                var dcc = new ListItem(EntitlementType.DCC.ToString(), ((int)EntitlementType.DCC).ToString());
                //var dccs = new ListItem(EntitlementType.DCCS.ToString(), ((int)EntitlementType.DCCS).ToString());
                var idd = new ListItem(EntitlementType.IDD.ToString(), ((int)EntitlementType.IDD).ToString());
                if (userObject.UserType == EntitlementType.Default || userObject.UserType == EntitlementType.None)
                {
                    ddlEntitlementType.Items.Add(idd);
                    divWarningMessage.Visible = false;
                    telephone.Text = "Reserved Number";
                    ddlEntitlementType.Items.Add(telephone);
                    param.Add("userId", ddlEndpointUser.SelectedValue);
                    param.Add("endpointSystemId", this.EndpointSystemId);
                    hasResponseGroup = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.SkypeForBusiness.HasUserHoldAnyResponseGroup, param);
                    
                    if (hasResponseGroup == false)
                    {
                        if (excludeDCCDCCS == false)
                        {
                            ddlEntitlementType.Items.Add(dcc);
                            //ddlEntitlementType.Items.Add(dccs);
                        }
                    }

                }
               
                //else if(userObject.UserType == EntitlementType.IDD)
                //{
                //    ddlEntitlementType.Items.Add(dccs);
                //}
                //else if (userObject.UserType == EntitlementType.DCCS)
                //{
                //    ddlEntitlementType.Items.Add(idd);
                //}

                if(userObject.UserType == EntitlementType.IDD || userObject.UserType == EntitlementType.DCC)
                {
                    divWarningMessage.Visible = true;
                    var userHolding = string.Empty;
                    param = new Dictionary<string, string>();
                    param.Add("endpointUserId", ddlEndpointUser.SelectedValue);
                    param.Add("endpointSystemId", this.AdhocRequest.EndpointSystemId.ToString());
                    var entitlements = DomainApiClient<Dto.EntitlementReconcilation>.GetByAction<List<Dto.EntitlementReconcilation>>(DomainApiAction.Provisioning.EntitlementReconcilation.GetByUserEntitlementByExceptGroup, param);
                    if (entitlements.Count > 0)
                    {
                        userHolding =$"Reserved number can be requested only if no additional access has been applied before(Current Access:{string.Join(",", entitlements.Select(i =>(EntitlementType)i.EntitlementType))})";
                    }
                    try
                    {
                        ListItemCollection liCol = ddlEntitlementType.Items;
                        if (liCol != null && liCol.Count > 0)
                        {
                             foreach (var entitlement in entitlements)
                            {
                                if (entitlement.EntitlementType > 0)
                                {
                                    for (int i = 0; i < liCol.Count; i++)
                                    {
                                        ListItem li = liCol[i];
                                        if (li.Value == entitlement.EntitlementType.ToString())
                                            ddlEntitlementType.Items.Remove(li);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    
                    lblWarning.Text = userHolding;
                }

                if ((userObject.UserType == EntitlementType.IDD || userObject.UserType == EntitlementType.Default))
                {
                    ddlEntitlementType.Items.Add(resGrp);
                    //if((string.IsNullOrWhiteSpace(userObject.HostingProvider) || userObject.HostingProvider.Equals("SRV:", StringComparison.InvariantCultureIgnoreCase)))
                    //{
                    //    ddlEntitlementType.Items.Add(new ListItem(EntitlementType.Move.ToString(), ((int)EntitlementType.Move).ToString()));
                    //}
                }
            }
            else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                param = new Dictionary<string, string>();
                param.Add("userId", ddlEndpointUser.SelectedValue);
                param.Add("endpointSystemId", this.EndpointSystemId);
                hasResponseGroup = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.SkypeForBusiness.HasUserHoldAnyResponseGroup, param);

                if (userObject.UserType != EntitlementType.DCC)
                {
                    if (hasResponseGroup)
                    {
                        ddlEntitlementType.Items.Add(resGrp);
                    }
                }

                param = new Dictionary<string, string>();
                param.Add("endpointUserId", ddlEndpointUser.SelectedValue);
                param.Add("endpointSystemId", this.AdhocRequest.EndpointSystemId.ToString());
                var entitlements = DomainApiClient<Dto.EntitlementReconcilation>.GetByAction<List<Dto.EntitlementReconcilation>>(DomainApiAction.Provisioning.EntitlementReconcilation.GetByUserEntitlementByExceptGroup, param);
                if(entitlements.Count > 0)
                {
                    foreach (var entitlement in entitlements)
                    {
                        var type = (EntitlementType)entitlement.EntitlementType;
                        if(excludeDCCDCCS && (type == EntitlementType.DCC))
                        {
                            continue;
                        }
                        if (entitlement.EntitlementType != (int)EntitlementType.Default &&(type == EntitlementType.IDD || type == EntitlementType.DCC))
                        {
                            ddlEntitlementType.Items.Add(new ListItem(type.ToString(), ((int)type).ToString()));
                        }
                    }
                }
            }
            else if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                this.divEntitlementType.Visible = false;
            }
            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && ddlEntitlementType != null && ddlEntitlementType.Items.Count > 0)
            {
                this.divEntitlementType.Visible = true;
            }
        }
        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            if (ddlEndpointUser.SelectedIndex > 0)
            {
                EndpointUserId = ddlEndpointUser.SelectedValue;
            }
            else
            {
                ClearFields();
                EndpointUserId = "0";
                ShowAlertMessage(Infrastructure.AlertType.Warning,"Please select EndpointUser");
            }
        }
        Dto.AccountReconDto BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            Dto.AccountReconDto list = DomainApiClient<Dto.ADSUserInfo>.GetByAction<Dto.AccountReconDto>("GetADUsersByDIUserId", dict);
            Session["EndpointUserId"] = null;
            if (list != null && list.AccountReconDictionary != null)
            {
                ddlEndpointUser.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEndpointUser, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
                if (ddlEndpointUser.SelectedItem != null && ddlEndpointUser.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    Session["EndpointUserId"] = Convert.ToInt32(ddlEndpointUser.SelectedValue);
                }
            }
            return list;
        }
        void ClearFields()
        {
            Session["AdHocSFBInfo"] = null;
            this.SelectedMembers = null;
            ResponseGroupGrid.Visible = false;
            pnlResponseGroup.Visible = false;
            divNotes.Visible = false;
            ResponseGroupAutoCompleteBox.Entries.Clear();
        }
        private void ClearDefaultValue()
        {
            lblDCCWarningMessage.Text = "";
            pnlMoveOption.Visible = false;
            divRegistrarPool.Visible = false;
            rdoOnLine.Checked = false;
            rdoOnPrem.Checked = false;
            divLineURI.Visible = false;
            lblLineURI.Visible = false;
            pnlDCCExtension.Visible = false;
            txtDCCExtension.Text = "";
            divMask.Visible = false;
            ResponseGroups = null;
            FillApplicationGrid();
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                divNotes.Visible = false;
                divMask.Visible = false;
                chkMask.Checked = true;
            }
            if(this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                divLineURI.Visible = true;
                lblLineURI.Visible = true;
                divEndpointUser.Visible = true;
            }
            btnAddRequest.Visible = false;
            pnlResponseGroupSelection.Visible = false;
            pnlResponseGroup.Visible = false;
        }
        public override bool Validate()
        {
            var returnValue = true;
            if(this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                if (this.ddlEndpointUser.SelectedIndex <= 0)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, "Please select Endpoint User.");
                    return false;
                }
                if(this.AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                {
                    if (this.ddlEntitlementType.SelectedIndex <= 0)
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, "Please select Type.");
                        return false;
                    }
                }
                EntitlementType entitlementType = (EntitlementType)Enum.Parse(typeof(EntitlementType), ddlEntitlementType.SelectedItem.Value, true);
                if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && entitlementType == EntitlementType.Move)
                {
                    if(rdoOnPrem.Checked == false && rdoOnLine.Checked == false)
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, "Please select an Option OnLine");
                        return false;
                    }
                    var param = new Dictionary<string, string>();
                    param.Add("userId", EndpointUserId);
                    param.Add("endpointSystemId",this.AdhocRequest.EndpointSystemId.ToString());
                    var userInfo = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetById(Convert.ToInt64(EndpointUserId));
                    if(userInfo != null && (userInfo.UserType == EntitlementType.DCC || userInfo.UserType == EntitlementType.DCCS) && rdoOnLine.Checked)
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, "DCC/DCCS user cannot apply for Move(OnLine) option");
                        return false;
                    }
                    //if (rdoOnLine.Checked)
                    //{
                    //    param = new Dictionary<string, string>();
                    //    param.Add("userId", ddlEndpointUser.SelectedValue);
                    //    param.Add("endpointSystemId", this.EndpointSystemId);
                    //    var isResponseGroupExists = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.SkypeForBusiness.HasUserHoldAnyResponseGroup, param);
                    //    if (isResponseGroupExists && rdoOnLine.Checked)
                    //    {
                    //        ShowAlertMessage(Infrastructure.AlertType.Error, "User is part of ResponseGroup(s). Hence cannot raise an Move Online request.");
                    //        return false;
                    //    }
                    //}
                    //else
                    //{
                    //    if(rdoOnPrem.Checked && ddlRegistrarPool.SelectedIndex <= 0)
                    //    {
                    //        ShowAlertMessage(Infrastructure.AlertType.Error, "Please Select a RegistrarPool");
                    //        return false;
                    //    }
                    //}
                   
                }
                if (entitlementType == EntitlementType.ResponseGroup)
                {
                    if (this.ResponseGroups.Count == 0)
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, "Please Select at least one Response Group");
                        return false;
                    }
                }
                //else if(entitlementType == EntitlementType.DCC && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                //{
                //    if (string.IsNullOrWhiteSpace(txtDCCExtension.Text))
                //    {
                //        ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter valid DCC extension number");
                //        return false;
                //    }
                //}
                returnValue = ValidateRequest();
            }
            return returnValue;
        }
        protected bool ValidateRequest()
        {
            if (AdhocRequest == null)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "AdHocAccessRequest is null.");
                return false;
            }
            return true;
        }
       
        protected void chkSpecialNumber_CheckedChanged(object sender, EventArgs e)
        {

        }
        protected void FillApplicationGrid()
        {
            if (this.ResponseGroups == null)
            {
                ResponseGroups = new List<ResponseGroup>();
            }
            var visible = (ResponseGroups.Count > 0);
            ResponseGroupGrid.DataSource = this.ResponseGroups.AsEnumerable().Select(c => c);
            ResponseGroupGrid.DataBind();
            pnlResponseGroup.Visible = visible;
            ResponseGroupGrid.Visible = visible;
            if(this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                var column = ResponseGroupGrid.Columns.FindByUniqueName("Mask");
                if(column != null)
                {
                    column.HeaderText = "Remove Masking";
                }
            }  
        }
        protected void btnAddRequest_Click(object sender,EventArgs e)
        {
            AddResponseGroup();
        }
        private void AddResponseGroup()
        {
            if (ResponseGroupAutoCompleteBox.Entries.Count == 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Warning, "Please select ResponseGroup");
                return;
            }

            var responses = this.ResponseGroups;
            foreach (AutoCompleteBoxEntry Entries in ResponseGroupAutoCompleteBox.Entries)
            {
                var Id = Convert.ToInt64(Entries.Value);
                var group = DomainApiClient<Dto.SkypeForBusinessResponseGroupInfo>.GetById(Id);
                if (responses != null && responses.Count > 0 && responses.Any(i => i.Id == Id))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, $"ResponseGroup:{group.Name} already added. Please add another ResponseGroup.");
                    return;
                }
                
                var responseGroup = new ResponseGroup();
                responseGroup.Id = group.Id;
                responseGroup.Identity = group.Identity;
                responseGroup.Name = group.Name;
                responseGroup.Mask = chkMask.Checked;
                responses.Add(responseGroup);
            }
            this.ResponseGroups = responses;
            ResponseGroupAutoCompleteBox.Entries.Clear();
            divMask.Visible = false;
            chkMask.Checked = true;
            FillApplicationGrid();
        }
        protected override IEntitlementDetail GetEntitlementDetail()
        {
            var request = new AdhocSkypeForBusinessRequest();
            request = GetAdhocSkypeForBusinessRequest();
            return request;
        }
        private AdhocSkypeForBusinessRequest GetAdhocSkypeForBusinessRequest()
        {
            var request = new AdhocSkypeForBusinessRequest();
            if (AdhocRequest != null)
            {
                request.EndpointUserId = ddlEndpointUser.SelectedIndex > 0 ? Convert.ToInt32(ddlEndpointUser.SelectedItem.Value) : 0;

                if (ddlEndpointUser.SelectedItem != null)
                {
                    request.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                }

                EntitlementType entitlementType =(EntitlementType)Enum.Parse(typeof(EntitlementType), ddlEntitlementType.SelectedItem.Value, true);
                request.EntitlementType = entitlementType;
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    var user = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetById(request.EndpointUserId);
                    request.EntitlementType = user.UserType;
                }
                else if(AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    request.EntitlementType = EntitlementType.Default;
                }
                if (entitlementType == EntitlementType.ResponseGroup)
                {
                    request.SkypeForBusinessResponseGroups = this.ResponseGroups;
                }
                //else if(entitlementType == EntitlementType.Move)
                //{
                //    if(rdoOnLine.Checked)
                //    {
                //        request.MoveToOnline = true;
                //    }else if (rdoOnPrem.Checked)
                //    {
                //        request.MoveToOnline = false;
                //        request.RegistrarPoolName = ddlRegistrarPool.SelectedValue;
                //    }
                //}
                else if(entitlementType == EntitlementType.DCC)
                {
                    request.LineUri = string.Empty;
                    request.Extension = string.Empty;
                    lblLineURI.Text = string.Empty;
                }
                request.DisplayCurrentLineURI = lblLineURI.Text;
            }
            return request;
        }
        protected void ResponseGroupAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            string group = string.Empty;

            var NewItem = ResponseGroupAutoCompleteBox.Entries[ResponseGroupAutoCompleteBox.Entries.Count - 1];
            ResponseGroupAutoCompleteBox.Entries.Clear();
            ResponseGroupAutoCompleteBox.Entries.Add(NewItem);
            if(this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                var entitlementType = (EntitlementType)(Convert.ToInt32(ddlEntitlementType.SelectedValue));
                if (entitlementType == EntitlementType.ResponseGroup)
                {
                    
                    foreach (AutoCompleteBoxEntry Entries in ResponseGroupAutoCompleteBox.Entries)
                    {
                        divMask.Visible = false;
                        var Id = Convert.ToInt64(Entries.Value);
                        var param = new Dictionary<string, string>();
                        param.Add("endpointUserId", ddlEndpointUser.SelectedValue);
                        param.Add("endpointSystemId", this.EndpointSystemId);
                        param.Add("entitlement", Entries.Text);
                        var groupobj = DomainApiClient<Dto.EntitlementReconcilation>.GetByAction<Dto.EntitlementReconcilation>(DomainApiAction.Provisioning.EntitlementReconcilation.GetByUserEntitlementByType, param);
                        if (groupobj != null && !string.IsNullOrWhiteSpace(groupobj.Functions))
                        {
                            divMask.Visible = true;
                            chkMask.Checked = true;
                            chkMask.Enabled = false;
                        }
                        //else
                        //{
                        //    divMask.Visible = false;
                        //}
                        //divMask.Visible = false;
                        //chkMask.Checked = false;
                    }
                }
            }else
            {
                divMask.Visible = false;
                chkMask.Checked = true;
                foreach (AutoCompleteBoxEntry Entries in ResponseGroupAutoCompleteBox.Entries)
                {
                    var Id = Convert.ToInt64(Entries.Value);
                    var resGroup = DomainApiClient<Dto.SkypeForBusinessResponseGroupInfo>.GetById(Id);
                    if(resGroup != null)
                    {
                        var param = new Dictionary<string, string>();
                        param.Add("name", resGroup.Name);
                        param.Add("entitlementType", EntitlementType.ResponseGroup.ToString());
                        var resGroupMatrix = DomainApiClient<Dto.SkypeForBusinessProvisioningMatrix>.GetByAction<Dto.SkypeForBusinessProvisioningMatrix>(DomainApiAction.Matrix.SkypeForBusinessMatrix.GetSkypeForBusinessMatrixByName, param);
                        divMask.Visible = resGroupMatrix != null;
                    }
                }
            }
        }
        protected void ResponseGroupAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (ResponseGroupAutoCompleteBox.Entries.Count == 0)
            {

            }
        }
        private Dto.SkypeForBusinessUserInfo GetEndPointUser()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("userId", this.AdhocRequest.RequesteeUserId.ToString());
            parameters.Add("endpointSystemId", EndpointSystemId.ToString());
            return DomainApiClient<Dto.SkypeForBusinessUserInfo>.
                        GetByAction<Dto.SkypeForBusinessUserInfo>(DomainApiAction.Endpoint.SkypeForBusiness.GetSkypeForBusinessUserById, parameters);
        }
        protected void ddlEntitlementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ddlEntitlementType.SelectedIndex == 0)
            {
                if(this.AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, "Please select Type");
                    this.ResponseGroups = new List<ResponseGroup>();
                    ClearDefaultValue();
                    return;
                }
                
            }
            var fields = GetProvisioningFields();
            ClearDefaultValue();
            
            EntitlementType type = (EntitlementType)Enum.Parse(typeof(EntitlementType), ddlEntitlementType.SelectedItem.Value, true);
            var userObject = GetEndPointUser();
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                divEndpointUser.Visible = true;
                divLineURI.Visible = true;
                lblLineURI.Text = userObject?.LineURI;
                lblLineURI.Visible = true;
            }
            divReservedNumberWarning.Visible = false;

            string vp = string.Empty;
            string vrp = string.Empty;
            string dpp = string.Empty;
            string prefix = string.Empty;
            if (userObject != null)
            {
               
                if (((userObject.UserType == EntitlementType.DCC) && (type == EntitlementType.DCCS || type == EntitlementType.IDD)) ||
                    (userObject.UserType == EntitlementType.IDD && (type == EntitlementType.DCC || type == EntitlementType.DCCS)) ||
                    (userObject.UserType == EntitlementType.DCCS && (type == EntitlementType.DCC || type == EntitlementType.IDD)) ||
                    (type == EntitlementType.ResponseGroup && (userObject.UserType == EntitlementType.DCC || userObject.UserType == EntitlementType.DCCS))
                    )
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, $"{userObject.UserType} user cannot apply for {type}.");
                    return;
                }
            }
            switch (type)
            {
                case EntitlementType.ResponseGroup:
                    {
                        pnlResponseGroupSelection.Visible = true;
                        pnlResponseGroup.Visible = true;
                        btnAddRequest.Visible = true;
                    }
                    break;
                case EntitlementType.Default:
                    if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                    {
                        divReservedNumberWarning.Visible = true;
                    }
                    
                    break;
                case EntitlementType.DCC:
                    {
                        ShowDCCLineUriPrefix();
                    }
                    break;
                //case EntitlementType.DCCS:
                //    break;
                case EntitlementType.IDD:
                    break;
                //case EntitlementType.Move:
                //    {
                //        //var param = new Dictionary<string, string>();
                //        //param.Add("userId", ddlEndpointUser.SelectedValue);
                //        //param.Add("endpointSystemId", this.EndpointSystemId);
                //        //var usr = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetById(Convert.ToInt64(ddlEndpointUser.SelectedValue));
                //        //var isResponseGroupExists = DomainApiClient<Dto.SkypeForBusinessUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.SkypeForBusiness.HasUserHoldAnyResponseGroup, param);
                //        divRegistrarPool.Visible = false;
                //        pnlMoveOption.Visible = false;
                //        rdoOnPrem.Checked = false;
                //        if (string.IsNullOrWhiteSpace(userObject.HostingProvider) || userObject.HostingProvider.Equals("SRV:",StringComparison.InvariantCultureIgnoreCase))
                //        {
                //            rdoOnLine.Enabled = true;
                //            rdoOnLine.Checked = true;
                //            pnlMoveOption.Visible = true;
                //        }
                //        else
                //        {
                //            ShowAlertMessage(Infrastructure.AlertType.Warning, "Selected User is already in Onlince. Hence cannot raise Move Online request.");
                //        }
                //        //else
                //        //{
                //        //    rdoOnLine.Enabled = true;
                //        //    rdoOnLine.Checked = true;
                //        //    divRegistrarPool.Visible = false;
                //        //}
                        
                //        //LoadRegistrarPool(fields);
                //        break;
                //    }
                default:break;
            }        
            
        }

        private void SetDCCAmendmentRemoveMessage()
        {
            lblDCCWarningMessage.Text = MasterConfigHelper.AmendmentRemoveDCCMessage;
        }
        private void SetDCCAmendmentAddMessage()
        {
            lblDCCWarningMessage.Text = MasterConfigHelper.AmendmentAddDCCMessage;
        }
        private void ShowDCCLineUriPrefix()
        {
            if(this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                SetDCCAmendmentAddMessage();
                pnlDCCExtension.Visible = false;
                //var param = new Dictionary<string, string>();
                //param.Add("endpointSystemId", this.EndpointSystemId.ToString());
                //param.Add("fieldName", "DefaultDCC");
                //var field = DomainApiClient<Dto.NProvisioningField>.GetByAction<Dto.NProvisioningField>(DomainApiAction.Ruleset.NProvisioningField.GetByEndpointSystemIdAndFieldName, param);
                //if (field != null)
                //{
                //    lblLineUrIPrefix.Text = field.Value;
                //}
                txtDCCExtension.Visible = false;
            }else if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                SetDCCAmendmentRemoveMessage();
            }
        }
        private List<Dto.NProvisioningField> GetProvisioningFields()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointSystemTypeId",((int)DI.Chakra.Infrastructure.Enums.EndpointType.MicrosoftTeams).ToString());
            parameter.Add("endPointSystemId",this.AdhocRequest.EndpointSystemId.ToString());
            var listProvField = DomainApiClient<Dto.NProvisioningField>.GetByAction<List<Dto.NProvisioningField>>(DomainApiAction.Ruleset.NProvisioningField.GetByEndPointSystemIncludingValueDisplayName, parameter);
            return listProvField;
        }
        protected void ResponseGroupGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = ResponseGroups;
                var identity = Convert.ToInt64(((GridDataItem)e.Item).GetDataKeyValue("Id"));
                requests.Remove(requests.FirstOrDefault(c => c.Id == identity));
                this.ResponseGroups = requests;
                FillApplicationGrid();
            }
            else if (e.CommandName == "View")
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

        protected void Move_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoOnLine.Checked)
            {
                divRegistrarPool.Visible = false;
            }else
            {
                divRegistrarPool.Visible = true;
                ddlRegistrarPool.SelectedIndex = 0;
            }
        }
        private void LoadRegistrarPool(List<NProvisioningField> fields)
        {
            ddlRegistrarPool.Items.Clear();
            ddlRegistrarPool.Items.Add(new ListItem($"--- Select Registrar Pool ---","0"));
            var pools = fields.Where(i => i.FieldName.Equals("Registrarpool", StringComparison.InvariantCultureIgnoreCase) ||
                                          i.FieldName.Equals("RegistrarPool2", StringComparison.InvariantCultureIgnoreCase));
            foreach (var pool in pools) 
            {
                ddlRegistrarPool.Items.Add(new ListItem($"{pool.FieldName} ({pool.Value})", pool.Value));
            }
            
        }
    }
}