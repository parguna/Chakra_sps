using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocSapSrmStep2UC : UIBaseStep2UC
    {
        private const string ARSRMRoles = "ARSRMRoles";
        public List<AdHocSAPSRMRequestDetail> SRMRoles
        {
            get
            {
                if(ViewState[ARSRMRoles] == null)
                {
                    ViewState[ARSRMRoles] = new List<AdHocSAPSRMRequestDetail>();
                }
                return (List<AdHocSAPSRMRequestDetail>)ViewState[ARSRMRoles];
            }
            set
            {
                ViewState[ARSRMRoles] = value;
            }
        }
        public string SRMUserName
        {
            get
            {
                return this.hdnUserName.Value;
            }
            set
            {
                this.hdnUserName.Value = value;
            }
        }
        public string SRMAccountType
        {
            get
            {
                return this.hdnSRMAccountType.Value;
            }
            set
            {
                this.hdnSRMAccountType.Value = value;
            }
        }
        public string SRMActionType
        {
            get
            {
                return this.hdnHSRMActionType.Value;
            }
            set
            {
                this.hdnHSRMActionType.Value = value;
            }
        }
        public string SRMUserId
        {
            get
            {
                return this.hdnSRMUserId.Value;
            }
            set
            {
                this.hdnSRMUserId.Value = value;
            }
        }
        public string SRMEndpointId
        {
            get
            {
                return this.hdnEndpointId.Value;
            }
            set
            {
                this.hdnEndpointId.Value = value;
            }
        }
        protected override void afterSave()
        {
            base.afterSave();
            SRMRoles = null;
            AccontNameTxt.Text = string.Empty;
            BusinessRoleAutoCompleteBox.Entries.Clear();
            ddlPOrg.Items.Clear();
            PGrpComboBox.Items.Clear();
            
            if (this.AdhocRequest != null)
            {
                if(this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    ddlFlinsUsers.SelectedIndex = -1;
                }
                if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                {
                    ddlEndpointUser.SelectedIndex = -1;
                }
                SRMObjGrid.DataSource = null;
                SRMObjGrid.DataBind();
                SRMObjGrid.Visible = false;
            }
        }
        public string SelectedBusinessRole
        {
            get
            {
                if (BusinessRoleAutoCompleteBox.Entries.Count > 0)
                    return BusinessRoleAutoCompleteBox.Entries[0].Value;
                else
                {
                    return string.Empty;
                }
            }
        }
        public string SelectedPorg
        {


            get
            {
                if (ddlPOrg.SelectedIndex > 0)
                {
                    return this.ddlPOrg.SelectedItem.Value;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public List<string> SelectedPGrps
        {
            get
            {
                List<string> selecteditems = new List<string>();

                foreach (var checkeditem in PGrpComboBox.CheckedItems)
                {
                    selecteditems.Add(checkeditem.Text);
                }
                return selecteditems;
            }

        }
        public string AccountName
        {
            get
            {
                if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount && ddlEndpointUser.SelectedIndex>0)
                {
                    return ddlEndpointUser.SelectedItem.Text;
                }


                if ((this.AdhocRequest.AccountType == AccountType.Service || this.AdhocRequest.AccountType == AccountType.Shared || this.AdhocRequest.AccountType == AccountType.Secondary)
                       && (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
                {
                    return this.AccontNameTxt.Text;
                }

                if (ddlFlinsUsers.SelectedIndex > 0)
                    return ddlFlinsUsers.SelectedItem.Text;
                return string.Empty;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void ClearFields()
        {
            SRMRoles = null;
            AccontNameTxt.Text = string.Empty;
            BusinessRoleAutoCompleteBox.Entries.Clear();
            ddlPOrg.Items.Clear();
            PGrpComboBox.Items.Clear();
            SRMObjGrid.Visible = false;
            btnAdd.Enabled = true;
        }

        protected override void InitializeStep()
        {
            SRMAccountType = string.Empty; 
            SRMUserId = "0";
            SRMActionType = string.Empty;
            SRMUserName = string.Empty;
            SRMEndpointId = "0";
            divMessage.Visible = false;
            trSecurityQstns.Visible = false;
            ClearFields();
            BusinessRoleAutoCompleteBox.Enabled = true;
            if (AdhocRequest != null)
            {
                SpecialAccountWarning.ShowWarningMessage(AdhocRequest);
                trflinsUsers.Visible = AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType == AccountType.User;
                divMessage.Visible = trflinsUsers.Visible;
                btnAdd.Visible = !(AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount);
                SRMUserId = AdhocRequest.RequesteeUserId.ToString();
                SRMActionType = ((int)AdhocRequest.AdhocActionType).ToString();
                SRMAccountType = ((int)AdhocRequest.AccountType).ToString();
                SRMEndpointId = AdhocRequest.EndpointSystemId.ToString();
                
                LoadControl();
                if (trflinsUsers.Visible)
                {
                    BindFlinsusers(this.AdhocRequest);
                }
            }

            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                this.chkboxRetainMainRole.Visible = true;
                this.chkboxRetainMainRole.Checked = false;
                SRMObjGrid.Columns.FindByUniqueName("RetainMainRole").Visible = true;
                SRMObjGrid.Columns.FindByUniqueName("RetainMainRole").Display = true;
                chkboxRetainMainRole_CheckedChanged(null, null);                
            }
        }

        void BindFlinsusers(AdHocAccessRequest request)
        {
            var dict = new Dictionary<string, string>();
            EndPoint endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationTypeNonTrusted, new Dictionary<string, string> { { "applicationType", ((int) ApplicationType.HRMS_FLINS).ToString() } });
            dict.Add("userid", request.RequesteeUserId.ToString());
            dict.Add("endpointsystemid", endPoint.Id.ToString());
            dict.Add("accountType", Convert.ToString((int)request.AccountType));
            AccountReconDto list = DomainApiClient<Dto.ADSUserInfo>.GetByAction<AccountReconDto>("GetFLINSAccountByDIUserId", dict);

            if (list != null && list.AccountReconDictionary != null)
            {
                ddlFlinsUsers.Items.Clear();
                DropDownHelper.DropDownListBind(ddlFlinsUsers, list.AccountReconDictionary, "Value", "Key", null, true, null, false);
                if (list.AccountReconDictionary.Count() == 1)
                {
                    ddlFlinsUsers.SelectedIndex = 1;
                }
            }
            ddlFlinsUsers_SelectedIndexChanged(this, null);
        }

        //void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        //{ 
        //    var dict = new Dictionary<string, string>();
        //    dict.Add("requesteeuserId", userId.ToString());
        //    dict.Add("acctype", Convert.ToString((int)accountType));
        //    var users = new List<string>();
        //    if (accountType == AccountType.Shared)
        //    {
        //        users = DomainApiClient<Dto.SAPSRMBusinessRoleMapping>.GetByAction<List<string>>(DomainApiAction.Endpoint.SRM.SAPSRMBusinessRoleMappings.GetSAPSRMEndpointUsers, dict);
        //    }
        //    else
        //    {
        //        users = DomainApiClient<Dto.SAPSRMBusinessRoleMapping>.GetByAction<List<string>>(DomainApiAction.Endpoint.SRM.SAPSRMBusinessRoleMappings.GetSAPSRMPgrpEndpointUsers, dict);
        //    }

        //    if (users != null && users.Count > 0)
        //    {
        //        users = users.Distinct().ToList();
        //        ddlEndpointUser.DataSource = users;
        //        ddlEndpointUser.DataValueField = "UserName";
        //        ddlEndpointUser.DataTextField = "UserName";
        //        ddlEndpointUser.DataBind();
        //        if (users.Count() == 1)
        //        {
        //            this.ddlEndpointUser.Items.Insert(0, "--Select EndPoint User--");
        //        }
        //        ddlEndpointUser_SelectedIndexChanged(this, null);
        //    }
        //}

        private void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            AccountReconDto list = DomainApiClient<Dto.ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);

            if (list != null && list.AccountReconDictionary != null)
            {
                ddlEndpointUser.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEndpointUser, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
                Session["EndpointUserId"] = null;
                if (ddlEndpointUser.SelectedItem != null && ddlEndpointUser.SelectedIndex > 0)
                {
                    Session["EndpointUserId"] = Convert.ToInt32(ddlEndpointUser.SelectedValue);
                }
                ddlEndpointUser_SelectedIndexChanged(this, null);
            }
        }
        void LoadControl()
        {

            if (AdhocRequest != null)
            {
                chkboxRetainMainRole.Visible = false;
                if (AdhocRequest.AccountType == AccountType.User)
                {
                    if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                    {
                        trEndPointUser.Visible = true;
                        SharedPanelFLINSBI.Visible = false;
                        TRBuisnessRole.Visible = true;
                        TRporg.Visible = true;
                        TRpgrp.Visible = true;
                        BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        trEndPointUser.Visible = true;
                        SharedPanelFLINSBI.Visible = false;
                        TRBuisnessRole.Visible = false;
                        TRporg.Visible = false;
                        TRpgrp.Visible = false;
                        BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);

                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        trflinsUsers.Visible = true;
                        trEndPointUser.Visible = false;
                        SharedPanelFLINSBI.Visible = false;
                        TRBuisnessRole.Visible = true;
                        TRporg.Visible = true;
                        TRpgrp.Visible = true;
                        // BindBuisnessRoles();
                    }
                }
                else if (AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary)
                {
                    if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                    {
                        trEndPointUser.Visible = true;
                        SharedPanelFLINSBI.Visible = false;
                        TRBuisnessRole.Visible = true;
                        TRporg.Visible = true;
                        TRpgrp.Visible = true;
                        BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        trEndPointUser.Visible = true;
                        SharedPanelFLINSBI.Visible = false;
                        TRBuisnessRole.Visible = false;
                        TRporg.Visible = false;
                        TRpgrp.Visible = false;
                        BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);

                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        if (AdhocRequest.AccountType == AccountType.Service)
                        {
                            trSecurityQstns.Visible = true;
                        }
                        trEndPointUser.Visible = false;
                        SharedPanelFLINSBI.Visible = true;
                        TRBuisnessRole.Visible = true;
                        TRporg.Visible = true;
                        TRpgrp.Visible = true;
                        trflinsUsers.Visible = false;
                        BindSAPUserType(AdhocRequest.AccountType);
                        //BindBuisnessRoles();
                    }
                }
                if (AdhocRequest.GetEntitlementDetail()!=null || ( Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
                {
                    BindAdhocRequest();
                }
            }
        }
        void BindAdhocRequest()
        {
            var Entitlement = (AdHocSAPSRMRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (ddlEndpointUser.Visible)
                {
                    if (ddlEndpointUser.Items.FindByValue(Entitlement.AccountName == "" ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.AccountName)) != null)
                        ddlEndpointUser.SelectedValue = Entitlement.AccountName == "" ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.AccountName);
                    ddlEndpointUser_SelectedIndexChanged(null, null);
                }
                if (Entitlement.AdHocSAPSRMRequestDetails != null && Entitlement.AdHocSAPSRMRequestDetails.Count() > 0)
                {
                    SRMRoles = Entitlement.AdHocSAPSRMRequestDetails;
                    SRMObjGrid.Visible = true;
                    SRMObjGrid.DataSource = Entitlement.AdHocSAPSRMRequestDetails;
                    if (this.AdhocRequest.AdhocActionType != AdHocActionType.AmendmentRemove)
                    {
                        SRMObjGrid.MasterTableView.GetColumn("RetainMainRole").Display = false;
                    }
                    SRMObjGrid.DataBind();
                }

                
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType != AccountType.User)
                {
                    AccontNameTxt.Text = Entitlement.AccountName;
                }

            }
        }

        void clearValues()
        {
            BusinessRoleAutoCompleteBox.Entries.Clear();
            ddlPOrg.Items.Clear();
            PGrpComboBox.Items.Clear();
        }
        
        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            SRMRoles = null;
            BusinessRoleAutoCompleteBox.Entries.Clear();
            ddlPOrg.Items.Clear();
            PGrpComboBox.Items.Clear();
            if (ddlEndpointUser.SelectedIndex > 0)
            {
                SRMUserName = ddlEndpointUser.SelectedItem.Text;
                // BindBuisnessRoles();
                BusinessRoleAutoCompleteBox.Enabled = true;
            }
            else
            {
                SRMUserName = string.Empty;
                BusinessRoleAutoCompleteBox.Enabled = false;
                ddlPOrg.Items.Clear();
                PGrpComboBox.Items.Clear();
            }
            SRMObjGrid.DataSource = SRMRoles;
            SRMObjGrid.DataBind();

        }
        protected void ddlPOrg_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPOrg.SelectedIndex > 0)
            {
                BindPgrps();
            }
            else
            {
                //btnAdd.Enabled = false;
                PGrpComboBox.Items.Clear();
            }

        }

        void BindPgrps()
        {

            Dictionary<string, string> dataSource = DomainApiClient<Dto.SAPSRMBusinessRoleMapping>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SRM.SAPSRMBusinessRoleMappings.GetSAPSRMPgrp,
                new Dictionary<string, string>
                {
                    {"BuisnessRole",SelectedBusinessRole },
                    {"adhocActionType",Convert.ToString((int)this.AdhocRequest.AdhocActionType) },
                    {"accType", Convert.ToString((int)this.AdhocRequest.AccountType) },
                    {"userId",this.AdhocRequest.RequesteeUserId.ToString() },
                    {"userName",SRMUserName },
                    {"POrg",SelectedPorg }
                });

            if (dataSource != null && dataSource.Count > 0)
            {
                this.PGrpComboBox.Enabled = true;
                dataSource = dataSource.OrderBy(item => item.Key).ToDictionary(item => item.Key, value => value.Value);
                this.PGrpComboBox.DataSource = dataSource;
                this.PGrpComboBox.DataTextField = "Key";
                this.PGrpComboBox.DataValueField = "Value";
                this.PGrpComboBox.DataBind();
            }
            else
            {
                this.PGrpComboBox.Enabled = false;
                //btnAdd.Enabled = true;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            // BusinessRoleAutoCompleteBox.Entries.Clear();

            if (string.IsNullOrWhiteSpace(this.SelectedBusinessRole))
            {
                ShowAlertMessage(Infrastructure.AlertType.Warning, "Please select Business Role.");
                return;
            }
            if ((this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
            {

                if (ddlPOrg.Items.Count > 0 && string.IsNullOrWhiteSpace(SelectedPorg))
                {
                    ShowAlertMessage(AlertType.Error, "Purchasing Org is mandatory");
                    return;
                }
                

                if (ddlPOrg.Items.Count > 0 && string.IsNullOrWhiteSpace(SelectedPorg))
                {
                    ShowAlertMessage(AlertType.Error, "Please select Purchasing Org");
                    return;
                }
                else
                {
                    if (ddlPOrg.Items.Count > 0 && !string.IsNullOrWhiteSpace(SelectedPorg))
                    {
                        if(PGrpComboBox.Items.Count > 0 && PGrpComboBox.CheckedItems.Count == 0)
                        {
                            ShowAlertMessage(AlertType.Error, "Please select Purchasing Group");
                            return;
                        }
                    }
                }
            }
            bool validate = true;
            if (AdhocRequest.AdhocActionType != AdHocActionType.AmendmentRemove)
            {
                validate = ADValidation();
            }
            
            if (AlreadyAdded() && validate)
            {
                if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    if (chkboxRetainMainRole.Visible && !chkboxRetainMainRole.Checked)
                    {
                        if (SRMRoles.Count > 0)
                        {
                            var seleteditems = SRMRoles;
                            seleteditems.RemoveAll(item => item.Entitlement == this.SelectedBusinessRole);
                            SRMRoles = seleteditems;
                        }
                    }
                    else if (chkboxRetainMainRole.Visible && chkboxRetainMainRole.Checked)
                    {
                        if (SRMRoles.Count > 0)
                        {
                            var seleteditems = SRMRoles;
                            seleteditems.RemoveAll(item => item.Entitlement == this.SelectedBusinessRole && item.RetainMainRole == false);
                            SRMRoles = seleteditems;
                        }
                    }
                    if(chkboxRetainMainRole.Visible && chkboxRetainMainRole.Checked)
                    {
                        
                        if (ddlPOrg.SelectedValue == "0")
                        {
                            ShowAlertMessage(Infrastructure.AlertType.Warning, "Please select Purchase Org.");
                            return;
                        }

                        bool anyOneShouldBeUnChecked = false;
                        foreach (RadComboBoxItem item in PGrpComboBox.Items)
                        {
                            if (item != null && item.Checked)
                            {
                                anyOneShouldBeUnChecked = true;
                                break;
                            }
                        }
                        if (anyOneShouldBeUnChecked == false)
                        {
                            ShowAlertMessage(Infrastructure.AlertType.Warning, "Please select Purchase Group.");
                            return;
                        }

                        var userCurrentRoles = DomainApiClient<Dto.SAPSRMBusinessRoleMapping>.GetByAction<List<Dto.PresentationUserBusinessRole>>(DomainApiAction.Endpoint.SRM.SAPSRMBusinessRoleMappings.GetSAPSRMUserRoles,
                                                new Dictionary<string, string> {   {"adhocActionType",Convert.ToString((int)this.AdhocRequest.AdhocActionType) },
                                                        { "accType" ,Convert.ToString((int)this.AdhocRequest.AccountType)},
                                                        { "Userid", this.AdhocRequest.RequesteeUserId.ToString() },
                                                         { "UserName", this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount ?                ddlFlinsUsers.SelectedItem.Text : ddlEndpointUser.SelectedItem.Text }
                                                         ,{ "EndpointId", this.AdhocRequest.EndpointSystemId.ToString()}
                                                    });
                        var selectedBRole = userCurrentRoles.Where(i =>  i.Value.ToUpper() == SelectedBusinessRole.ToUpper()).FirstOrDefault();
                        var morethanOnePorg = selectedBRole.POrgs.Where(i => i.Value.ToUpper() != SelectedPorg.ToUpper()).ToList();
                        var srmFilteredRoles = SRMRoles.Where(i => i.Entitlement.ToUpper() == this.SelectedBusinessRole.ToUpper()).ToList();
                        if (srmFilteredRoles.Count > 0)
                        {
                            if (morethanOnePorg.Count > 0)
                            {
                                var roles = new List<AdHocSAPSRMRequestDetail>();
                                var srmrole = new AdHocSAPSRMRequestDetail();
                                srmrole.Entitlement = this.SelectedBusinessRole;
                                srmrole.PurchaseOrg = this.SelectedPorg;
                                srmrole.RetainMainRole = chkboxRetainMainRole.Visible && chkboxRetainMainRole.Checked;
                                srmrole.PurchaseGroup = string.Join(",", this.SelectedPGrps);
                                srmrole.EntitlementType = EntitlementType.Role;
                                roles.Add(srmrole);
                                var allrole = roles.Union(srmFilteredRoles).ToList();
                                bool isAllowed = false;
                                var notInthList = selectedBRole.POrgs.Where(i=> !srmFilteredRoles.Exists(j=> j.PurchaseOrg.ToUpper() == i.Value.ToUpper() )).ToList();
                                foreach (var srmRole in allrole)
                                {
                                    var pOrgsWithGroups = selectedBRole.POrgs.FirstOrDefault(i => i.Value.ToUpper() == srmRole.PurchaseOrg.ToUpper());
                                    var pGrps = srmRole.PurchaseGroup.Split(new char[] { ',' }).ToList();
                                    if (!pOrgsWithGroups.PGroups.All(i => pGrps.Exists(j => j.ToUpper() == i.Value.ToUpper())))
                                    {
                                        isAllowed = true;
                                        break;
                                    }
                                }
                                if (isAllowed == false)
                                {
                                    if (notInthList.Count == 0)
                                    {
                                        ShowAlertMessage(Infrastructure.AlertType.Warning, "" +
                                            "At least one Purchase Group is required for your existing business role. Please deselect any one of them.");
                                        return;
                                    }
                                    else
                                    {
                                        if (notInthList.Count == 1 && PGrpComboBox.Items.All(i => i.Checked))
                                        {
                                            ShowAlertMessage(Infrastructure.AlertType.Warning, "At least one Purchase Group is required for your existing business role. Please deselect any one of them.");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (morethanOnePorg.Count > 0)
                            {
                                //Allowed
                            }
                            else
                            {
                                if (PGrpComboBox.Items.All(i => i.Checked))
                                {
                                    ShowAlertMessage(Infrastructure.AlertType.Warning, "At least one Purchase Group is required for your existing business role. Please deselect any one of them.");
                                    return;
                                }
                            }
                        }
                        
                    }
                }
                List<AdHocSAPSRMRequestDetail> _gridObjlst = new List<AdHocSAPSRMRequestDetail>();
                AdHocSAPSRMRequestDetail obj = new AdHocSAPSRMRequestDetail();
                obj.Entitlement = this.SelectedBusinessRole;
                obj.PurchaseOrg = this.SelectedPorg;
                obj.RetainMainRole = chkboxRetainMainRole.Visible && chkboxRetainMainRole.Checked;
                obj.PurchaseGroup = string.Join(",", this.SelectedPGrps);
                obj.EntitlementType = EntitlementType.Role;                
                _gridObjlst.Add(obj);

                if (_gridObjlst.Count > 0)
                {
                    var seleteditems = new List<AdHocSAPSRMRequestDetail>();
                    if (SRMRoles.Count > 0)
                    {
                        seleteditems = SRMRoles;
                    }

                    seleteditems = seleteditems.Count > 0 ? seleteditems.Union(_gridObjlst).ToList() : _gridObjlst;
                    SRMRoles = seleteditems;
                }

                if (SRMRoles.Count > 0)
                {
                    SRMObjGrid.Visible = true;
                    SRMObjGrid.DataSource = SRMRoles;
                    if (this.AdhocRequest.AdhocActionType != AdHocActionType.AmendmentRemove)
                    {
                        SRMObjGrid.MasterTableView.GetColumn("RetainMainRole").Display = false;
                    }
                    SRMObjGrid.DataBind();
                    //if (ddlPOrg.Items.Count > 0)
                    //    ddlPOrg.SelectedIndex = 0;
                    //PGrpComboBox.Items.Clear();
                    //BusinessRoleAutoCompleteBox.Entries.Clear();
                    //btnAdd.Enabled = false;
                }
            }
            clearValues();
        }
        bool AlreadyAdded()
        {
            var seleteditems = new List<AdHocSAPSRMRequestDetail>();
            if (SRMRoles.Count > 0)
            {
                seleteditems = SRMRoles;

                if (seleteditems.Count() > 0)
                {
                    if (seleteditems.Any(item => item.Entitlement == this.SelectedBusinessRole && item.PurchaseOrg == this.SelectedPorg))
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Warning, "The selected Business Role & Porg combination is already added.");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public bool ValidateAccountName()
        {
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {

                if (string.IsNullOrWhiteSpace(this.AccountName))
                {
                    return false;
                }

                else
                {
                    return true;
                }

            }
            return true;

        }

        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdHocSAPSRMRequest srmRequest = new AdHocSAPSRMRequest();
            srmRequest.AdHocSAPSRMSharedAccount = new AdHocSAPSharedAccount();
            if (AdhocRequest != null)
            {
               
                if (AdhocRequest.AccountType == AccountType.User)
                {
                    if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        srmRequest.AccountName = ddlFlinsUsers.SelectedItem.Text;
                    }
                    else if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                    {
                        srmRequest.EndpointUserId = Convert.ToInt64(ddlEndpointUser.SelectedValue);
                        if (ddlEndpointUser.SelectedItem != null)
                        {
                            srmRequest.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                            srmRequest.AccountName = ddlEndpointUser.SelectedItem.Text;
                        }
                    }
                    srmRequest.AdHocSAPSRMSharedAccount = null;
                    var list = SRMRoles;
                    srmRequest.AdHocSAPSRMRequestDetails = list;

                }
                else if (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Secondary)
                {
                    if (ValidateAccountName())
                    {
                        if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                        {
                            srmRequest.EndpointUserId = Convert.ToInt64(ddlEndpointUser.SelectedValue);
                            srmRequest.AccountName = ddlEndpointUser.SelectedItem.Text;
                            srmRequest.AdHocSAPSRMSharedAccount.AccountName = ddlEndpointUser.SelectedItem.Text;
                        }
                        else
                        {
                            srmRequest.AccountName = this.AccountName;
                            srmRequest.AdHocSAPSRMSharedAccount.AccountName = this.AccountName;
                            if (SAPUserTypeDropDown.SelectedIndex > 0)
                            {
                                srmRequest.AdHocSAPSRMSharedAccount.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                            }
                            srmRequest.AdHocSAPSRMSharedAccount.Description = txtDiscription.Text;
                        }
                        var list = SRMRoles;
                        srmRequest.AdHocSAPSRMRequestDetails = list;
                    }
                }
            }
            return srmRequest;
        }

        void BindSAPUserType(AccountType accountType)
        {
            try
            {
                SAPUserTypeDropDown.Items.Clear();
                SAPUserTypeDropDown.Items.Insert(0, new ListItem("-- Select SAP UserType --", "-1"));

                foreach (SAPUserType item in Enum.GetValues(typeof(SAPUserType)))
                {
                    if (item == SAPUserType.Reference || (accountType == AccountType.Shared && item == SAPUserType.Communication))
                        continue;

                    var description = UtilityMethods.GetEnumAttribute<Description>(item);
                    ListItem listItem = new ListItem(description, ((byte)item).ToString());
                    if (accountType == AccountType.Service && (((SAPUserType)item) == SAPUserType.Service || ((SAPUserType)item) == SAPUserType.System))
                    {
                        SAPUserTypeDropDown.Items.Add(listItem);
                    }
                    if ((accountType == AccountType.Shared || accountType == AccountType.Secondary) && item == SAPUserType.Dialog)
                    {
                        SAPUserTypeDropDown.Items.Add(listItem);
                        SAPUserTypeDropDown.SelectedIndex = 1;
                        //SAPUserTypeDropDown.Enabled = false;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public override bool Validate()
        {
            if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (AdhocRequest.AccountType == AccountType.User)
                {
                    if (ddlFlinsUsers.SelectedIndex == 0)
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, "Please select User Name.");
                        return false;
                    }
                }

                if (AdhocRequest.AccountType != AccountType.User)
                {
                    if (SAPUserTypeDropDown.SelectedIndex <= 0)
                    {
                        ShowAlertMessage(AlertType.Error, "Please Select SAP UserType.");
                        return false;
                    }

                    if (string.IsNullOrEmpty(AccontNameTxt.Text))
                    {
                        ShowAlertMessage(AlertType.Error, "Please Select Account Name.");
                        return false;
                    } 
                }
            }
            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                if (SRMRoles.Count == 0)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error,DI.Chakra.Infrastructure.Constants.addToGridWarning);
                    return false;
                }
            }
            if (AdhocRequest.AccountType == AccountType.Service && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (string.IsNullOrEmpty(AdhocRequest.SecurityQuestions))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter the Service account compliance check.");

                    return false;
                }
            }
           
            if (SRMRoles.Count > 0)
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType != AccountType.User)
                {
                   if (!string.IsNullOrWhiteSpace(this.AccountName))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;

            }
            else
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    if (ddlEndpointUser.SelectedIndex > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
        }
        private bool ADValidation()
        {
            if (!string.IsNullOrWhiteSpace(this.SelectedBusinessRole))
            {
                if (string.IsNullOrWhiteSpace(this.AccountName))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter account name.");
                    return false;
                }

                var AdGroupsExixst = DomainApiClient<Dto.SAPSRMMainRoleMatrix>.GetByAction<bool>(DomainApiAction.Endpoint.SRM.SAPSRMMainRoleMatrix.IsAdGroupsExist,
                new Dictionary<string, string> {
                        { "RoleName", this.SelectedBusinessRole },
                        {"actionType", ((int)AdhocRequest.AdhocActionType).ToString() },
                        {"accountType", ((int)this.AdhocRequest.AccountType).ToString() },
                        {"DIUserID",AdhocRequest.RequesteeUserId.ToString()  },
                        {"AccountName",this.AccountName  }
                });
                if (!AdGroupsExixst)
                {


                    ShowAlertMessage(Infrastructure.AlertType.Error, "AD account is required for selected role.");
                    return false;
                }

                return true;
            }
            return false;
        }

        protected void PGrpComboBox_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (PGrpComboBox.CheckedItems.Count > 0)
            {
                btnAdd.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = false;
            }
        }
        protected void SRMObjGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var Entitlement = (string)((GridDataItem)e.Item).GetDataKeyValue("Entitlement");
                var PurchaseOrg = (string)((GridDataItem)e.Item).GetDataKeyValue("PurchaseOrg");
                var PurchaseGroup = (string)((GridDataItem)e.Item).GetDataKeyValue("PurchaseGroup");
                List<AdHocSAPSRMRequestDetail> _sessionObj = SRMRoles;
                _sessionObj.RemoveAll(item => item.Entitlement == Entitlement && item.PurchaseOrg == PurchaseOrg && item.PurchaseGroup == PurchaseGroup);
                SRMRoles = _sessionObj;
                if (_sessionObj.Count > 0)
                {
                    SRMObjGrid.DataSource = SRMRoles;
                    SRMObjGrid.DataBind();
                }
                else
                {
                    SRMRoles = null;
                    SRMObjGrid.Visible = false;
                    //btnAdd.Enabled = false;
                }
                clearValues();
            }
            else if (e.CommandName == "Details")
            {
                var Entitlement = (string)((GridDataItem)e.Item).GetDataKeyValue("Entitlement");
                var userCurrentRoles = DomainApiClient<Dto.SAPSRMBusinessRoleMapping>.GetByAction<List<Dto.PresentationUserBusinessRole>>(DomainApiAction.Endpoint.SRM.SAPSRMBusinessRoleMappings.GetSAPSRMUserRoles,
                                                new Dictionary<string, string> {   {"adhocActionType",Convert.ToString((int)this.AdhocRequest.AdhocActionType) },
                                                        { "accType" ,Convert.ToString((int)this.AdhocRequest.AccountType)},
                                                        { "Userid", this.AdhocRequest.RequesteeUserId.ToString() },
                                                        { "UserName", this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount ? ddlFlinsUsers.SelectedItem.Text : ddlEndpointUser.SelectedItem.Text },
                                                        { "businessRole", Entitlement},{ "EndpointId", this.AdhocRequest.EndpointSystemId.ToString()}
                                                    });
                var selectedBRole = userCurrentRoles.Where(i => i.Value.ToUpper() == Entitlement.ToUpper()).FirstOrDefault();
                StringBuilder content = new StringBuilder();
                if(selectedBRole !=null && selectedBRole != null)
                {
                    content.AppendLine($"<b>Business Role:</b> {selectedBRole.RoleName} <br/>");
                    foreach (var POrg in selectedBRole.POrgs)
                    {
                        content.AppendLine($"<br/> <b>Purchase Org:</b> {POrg.Value} ");
                        content.AppendLine($"<br/> <b>Purchase Group(s):</b>");
                        if(POrg!=null)
                        {
                            content.Append($" {string.Join(",", POrg?.PGroups.Select(i=>i.PGrpName))} ");
                        }
                        content.Append($"<br/>");
                    }
                }
                lblInformation.Text = content.ToString();
                ShowWindow();
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
            if(this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                SRMObjGrid.Columns.FindByUniqueName("Details").Visible = true;
            }
        }
        protected void BusinessRoleAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

            if (BusinessRoleAutoCompleteBox.Entries.Count == 0)
            {
                ddlPOrg.Items.Clear();
                PGrpComboBox.Items.Clear();
                if(this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    chkboxRetainMainRole.Checked = false;
                }
            }
        }
        protected void BusinessRoleAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (BusinessRoleAutoCompleteBox.Entries.Count > 0)
            {
                if (BusinessRoleAutoCompleteBox.Entries.Count > 1)
                {
                    BusinessRoleAutoCompleteBox.Entries.Remove(BusinessRoleAutoCompleteBox.Entries[1]);
                } 
            }

            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                BindPorgs();
                if (ddlPOrg.Items.Count == 2)
                {
                    chkboxRetainMainRole.Checked = false;
                }
            }
        }
        protected void chkboxRetainMainRole_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkboxRetainMainRole.Checked)
            {
                ddlPOrg.Items.Clear();
                PGrpComboBox.Items.Clear();
                ddlPOrg.Enabled = false;
                PGrpComboBox.Enabled = false;
                btnAdd.Enabled = true;
                
            }
            if (chkboxRetainMainRole.Checked)
            {
                if (string.IsNullOrWhiteSpace(this.SelectedBusinessRole))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, "Please select Business Role.");
                    chkboxRetainMainRole.Checked = false;
                    return;
                }
                ddlPOrg.Enabled = true;
                PGrpComboBox.Enabled = true;
                //btnAdd.Enabled = false;
                BindPorgs();
            }
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                TRporg.Attributes.Remove("class");
                TRpgrp.Attributes.Remove("class");
                string cssRequired = "form-group";
                if (chkboxRetainMainRole.Checked)
                {
                    cssRequired = $"{cssRequired} required";
                }
                TRporg.Attributes.Add("class", cssRequired);
                TRpgrp.Attributes.Add("class", cssRequired);
            }
        }

        void BindPorgs()
        {

            Dictionary<string, string> dataSource = null;
            if (this.SelectedBusinessRole != string.Empty)
            {
                // public static Dictionary<string, string> GetSAPSRMPorg(string BuisnessRole, AdHocActionType adhocActionType, AccountType accType, int Userid)


                dataSource = DomainApiClient<Dto.SAPSRMBusinessRoleMapping>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SRM.SAPSRMBusinessRoleMappings.GetSAPSRMPorg,
                    new Dictionary<string, string> { {"BuisnessRole",SelectedBusinessRole } ,
                                                        {"adhocActionType",Convert.ToString((int)this.AdhocRequest.AdhocActionType) },
                                                        { "accType" ,Convert.ToString((int)this.AdhocRequest.AccountType)},
                                                        { "Userid", this.AdhocRequest.RequesteeUserId.ToString() },
                                                         { "UserName", SRMUserName }
                                                    });
            }

            if (dataSource != null && dataSource.Count > 0)
            {
                
                dataSource = dataSource.OrderBy(item => item.Key).ToDictionary(item => item.Key, value => value.Value);

                
                this.ddlPOrg.Enabled = true;
                this.PGrpComboBox.Enabled = true;
                this.ddlPOrg.DataSource = dataSource;
                this.ddlPOrg.DataTextField = "Key";
                this.ddlPOrg.DataTextField = "Value";
                this.ddlPOrg.DataBind();
                if (dataSource.Count > 0)
                {
                    this.ddlPOrg.Items.Insert(0, "--Select--");
                    ddlPOrg_SelectedIndexChanged(this, null);
                }
            }
            else
            {
                this.ddlPOrg.Items.Clear();
                this.chkboxRetainMainRole.Visible = false;
                this.ddlPOrg.Enabled = false;
                this.PGrpComboBox.Enabled = false;
                btnAdd.Enabled = true;
            }
        }
        protected void detail_OK_Click(object sender, EventArgs e)
        {
            CloseWindow();
        }
        void ShowWindow()
        {
            var key = "showSRMWindow";
            string script = "Sys.Application.remove_load(showWindow);Sys.Application.add_load(showSRMWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindow()
        {
            var key = "closeSRMWindow";
            string script = "Sys.Application.remove_load(closeWindow);Sys.Application.add_load(closeSRMWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        protected void SRMObjGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if(e.Item is GridEditableItem)
            {
                var dataItem = e.Item as GridEditableItem;
                if (dataItem != null)
                {
                    ImageButton editbutton = (ImageButton)dataItem["Details"].Controls[0];
                    var ctl = ((GridEditableItem)e.Item).GetDataKeyValue("RetainMainRole");
                    if(ctl != null && editbutton != null)
                    {
                        var RetainMainRole = Convert.ToBoolean(ctl);
                        editbutton.Visible = !RetainMainRole;
                    }
                    
                    
                }
            }
        }

        protected void ddlFlinsUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ddlFlinsUsers.SelectedIndex > 0)
            {
                SRMUserName = ddlFlinsUsers.SelectedItem.Text;
                BusinessRoleAutoCompleteBox.Enabled = true;
            }
            else
            {
                SRMUserName = string.Empty;
                BusinessRoleAutoCompleteBox.Enabled = false;
            }
        }
    }
}