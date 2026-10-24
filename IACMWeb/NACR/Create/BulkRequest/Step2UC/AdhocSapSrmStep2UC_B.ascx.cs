using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC
{
    public partial class AdhocSapSrmStep2UC_B :UIBaseStep2UC_B
    {
        private const string BARSRMObjlst = "BARSRMObjlst";
        public List<AdHocSAPSRMRequestDetail> SRMRoles
        {
            get
            {
                if(ViewState[BARSRMObjlst] == null)
                {
                    ViewState[BARSRMObjlst] = new List<AdHocSAPSRMRequestDetail>();
                }
                return (List<AdHocSAPSRMRequestDetail>)ViewState[BARSRMObjlst];
            }
            set
            {
                ViewState[BARSRMObjlst] = value;
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
                return AccontNameTxt.Text;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void InitializeStep()
        {
            ClearFields();
            BindRepeater();
            BindFlinsAccRepeater();
            if(this.AdhocRequest != null && this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                btnAdd.Visible = false;
            }
            else
            {
                btnAdd.Visible = true;
            }
        }

        void ClearFields()
        {
            chkboxRetainMainRole.Visible = false;
            chkboxRetainMainRole.Checked = false;
            SRMRoles = new List<AdHocSAPSRMRequestDetail>();
            BusinessRoleAutoCompleteBox.Entries.Clear();
            ddlPOrg.Items.Clear();
            PGrpComboBox.Items.Clear();
            SRMObjGrid.Visible = false;
            btnAdd.Enabled = true;
        }


        protected void ddlPOrg_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPOrg.SelectedIndex > 0)
            {
                BindPgrps();
            }
            else
            {

                PGrpComboBox.Items.Clear();
            }

        }

        void BindRepeater()
        {
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                Accountpanel.Visible = true;
                
                trAccount.Visible = true;
                Dictionary<string, Dictionary<long, string>> repeaterData = new Dictionary<string, Dictionary<long, string>>();
                AdhocRequest.BulkRequestDetails.ForEach(item =>
                {
                    var param = new Dictionary<string, string>
                    {
                        { "userId",item.UserId.ToString()},
                        { "endpointSystemId", AdhocRequest.EndpointSystemId.ToString()},
                        { "accountType", AccountType.User.ToString()}
                    };

                    List<AccountReconcilation> userAccounts = DomainApiClient<Dto.AccountReconcilation>.
                                                                    GetByAction<List<Dto.AccountReconcilation>>(
                                                                                    DomainApiAction.Provisioning.AccountReconcilation.GetByAccountTypeAndEndpointSystemId, param);


                    if (userAccounts != null && userAccounts.Count > 0)
                    {
                        var userNames = new Dictionary<long, string>();

                        var items = userAccounts.Distinct();
                        if (items.Count() > 1)
                        {
                            userNames.Add(0, "--Please Select Account--");
                        }
                        foreach (var i in items)
                        {
                            userNames.Add(i.EndpointUserId, i.AccountName);
                        }
                        repeaterData.Add(item.Name, userNames);
                    }
                });

                AccountRepeater.DataSource = repeaterData;
                AccountRepeater.DataBind();
                if (this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    TRBuisnessRole.Visible = false;
                    TRporg.Visible = false;
                    TRpgrp.Visible = false;
                    SRMObjGrid.Visible = false;
                }

            }
            else
            {
                trAccount.Visible = false;
                Accountpanel.Visible = false;
            }
        }

        void BindFlinsAccRepeater()
        {
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                Accountpanel.Visible = true;
                trAccountnameinfo.Visible = true;
            
                Dictionary<string, Dictionary<string, string>> repeaterData = new Dictionary<string, Dictionary<string, string>>();
                var userNames = new Dictionary<string, string>();
                var items = new Dictionary<string, string>();
                AdhocRequest.BulkRequestDetails.ForEach(item =>
                {
                    var dict = new Dictionary<string, string>();
                    EndPoint endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationTypeNonTrusted, new Dictionary<string, string> { { "applicationType", ((int)ApplicationType.HRMS_FLINS).ToString() } });
                    dict.Add("userid", item.UserId.ToString());
                    dict.Add("endpointsystemid", endPoint.Id.ToString());
                    dict.Add("accountType", Convert.ToString((int)AccountType.User));
                    AccountReconDto list = DomainApiClient<Dto.ADSUserInfo>.GetByAction<AccountReconDto>("GetFLINSAccountByDIUserId", dict);
                    if (list != null && list.AccountReconDictionary != null && list.AccountReconDictionary.Count > 0)
                    {
                        items = new Dictionary<string, string>();
                        userNames = new Dictionary<string, string>();
                        
                        foreach (var accRecon in list.AccountReconDictionary)
                        {
                            if (!userNames.ContainsKey(accRecon.Key.ToString()))
                            {
                                userNames.Add(accRecon.Key.ToString(), accRecon.Value);
                            }
                        }
                        repeaterData.Add(item.Name, userNames);
                    }
                 //   Dictionary<string, string> dataSource = DomainApiClient<Dto.SAPUserinfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAP.SAPUserinfo.GetActivePrimarySAPUserByDIUserId,
                 //new Dictionary<string, string> {
                 //   { "dIUserId", item.UserId.ToString() },
                 //   { "applicationType", Convert.ToString((int)ApplicationType.HRMS_FLINS) },
                 //   { "accountType", Convert.ToString((int)AccountType.User) } });
                     
                 //       var userNames = new Dictionary<string, string>();

                 //       var items = dataSource.Distinct().ToDictionary(key => key.Key, value => value.Value);

                 //       foreach (var i in items)
                 //       {
                 //           userNames.Add(i.Key, i.Value);
                 //       }
                 //       repeaterData.Add(item.Name, userNames); 
                });

                accNameRepeater.DataSource = repeaterData;
                accNameRepeater.DataBind();

            }
            else
            {

                trAccountnameinfo.Visible = false;
                accNameRepeater.Visible = false;
            }
        }

        void BindPorgs()
        {
            

            Dictionary<string, string> dataSource = null;
            if (this.SelectedBusinessRole != string.Empty)
            {
                dataSource = DomainApiClient<Dto.SAPSRMBusinessRoleMapping>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SRM.SAPSRMBusinessRoleMappings.GetSAPSRMPorgs,
                    new Dictionary<string, string> { { "role", this.SelectedBusinessRole } });
            }
            if (dataSource != null && dataSource.Count > 0)
            {
                dataSource = dataSource.OrderBy(item => item.Key).ToDictionary(item => item.Key, value => value.Value);
                if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    this.chkboxRetainMainRole.Visible = true;
                }
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
                //btnAdd.Enabled = true;
            }
        }
        void BindPgrps()
        {
          
            Dictionary<string, string> dataSource = DomainApiClient<Dto.SAPSRMBusinessRoleMapping>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SRM.SAPSRMBusinessRoleMappings.GetSAPSRMPgrps,
               new Dictionary<string, string>
               {
                    {"BuisnessRole",SelectedBusinessRole }, 
                    {"POrg",SelectedPorg }
               });


            dataSource = dataSource.OrderBy(item => item.Key).ToDictionary(item => item.Key, value => value.Value);
            if (dataSource.Count > 0)
            {
                this.PGrpComboBox.DataSource = dataSource;
                this.PGrpComboBox.DataTextField = "Key";
                this.PGrpComboBox.DataValueField = "Value";
                this.PGrpComboBox.DataBind();
            }
            else
            {
                this.PGrpComboBox.Enabled = false;
                btnAdd.Enabled = true;
            }
        }

        bool UpdateBulkReqAccountDetail()
        {
            if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                foreach (GridDataItem dataItem in AccountRepeater.Items)
                {

                    if (dataItem.ItemType == GridItemType.AlternatingItem || dataItem.ItemType == GridItemType.Item)
                    {
                        var userName = ((Label)dataItem.FindControl("lblAcountName")).Text;
                        var SelectedAccount = ((RadComboBox)dataItem.FindControl("ddlAccounts")).SelectedItem.Text;
                        //var endpointuserid = ((DropDownList)dataItem.FindControl("ddlAccounts")).SelectedItem.Value;
                        if (string.IsNullOrWhiteSpace(SelectedAccount))
                        {
                            ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter the Account Information."); 
                            return false;
                        }
                        //AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).EndpointUserId = Convert.ToInt32(endpointuserid);
                        AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).AccountName = SelectedAccount;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        bool UpdateBulkReqAccountNameDetail()
        {
            if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                foreach (GridDataItem dataItem in accNameRepeater.Items)
                {
                    if (dataItem.ItemType == GridItemType.AlternatingItem || dataItem.ItemType == GridItemType.Item)
                    {
                        var userName = ((Label)dataItem.FindControl("lblAcountName")).Text;
                        var SelectedAccount = ((DropDownList)dataItem.FindControl("dllFlinsAccount")).SelectedItem.Text;
                        AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.UserName == SelectedAccount).AccountName = SelectedAccount;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        bool AlreadyAdded()
        {
            var seleteditems = new List<AdHocSAPSRMRequestDetail>();
            if (SRMRoles.Count > 0)
            {
                seleteditems = SRMRoles;

                if (seleteditems.Count > 0)
                {
                    if (seleteditems.Any(item => item.Entitlement == this.SelectedBusinessRole && item.PurchaseOrg == this.SelectedPorg))
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, "The selected Purchasing Org is already added.");
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
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            
            bool validate = true;
            if (string.IsNullOrWhiteSpace(this.SelectedBusinessRole))
            {
                ShowAlertMessage(Infrastructure.AlertType.Warning, "Please select Business Role.");
                return;
            }
            if ((this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
            {

                if (ddlPOrg.Items.Count > 0 && string.IsNullOrWhiteSpace(SelectedPorg))
                {
                    ShowAlertMessage(AlertType.Error, "Please select Purchasing Org");
                    return;
                }
                else
                {
                    if (ddlPOrg.Items.Count > 0 && !string.IsNullOrWhiteSpace(SelectedPorg))
                    {
                        if (PGrpComboBox.Items.Count > 0 && PGrpComboBox.CheckedItems.Count == 0)
                        {
                            ShowAlertMessage(AlertType.Error, "Please select Purchasing Group");
                            return;
                        }
                    }
                }
            }
            if (AdhocRequest.AdhocActionType != AdHocActionType.AmendmentRemove)
            {
                validate = ADValidation();
            }

            validate = ADValidation(); 
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
                    else
                    {
                        SRMObjGrid.MasterTableView.GetColumn("RetainMainRole").Display = true;
                    }
                    SRMObjGrid.DataBind();
                    if (ddlPOrg.Items.Count > 0)
                        ddlPOrg.SelectedIndex = 0;
                    PGrpComboBox.Items.Clear();
                    ddlPOrg.Items.Clear();
                    BusinessRoleAutoCompleteBox.Entries.Clear();
                    //btnAdd.Enabled = false;
                }
            }
        }

        bool ADValidation()
        {
            if (this.SelectedBusinessRole != string.Empty)
            {
                bool AdGroupsExixst = true;
                var accountName = string.Empty;
                long userid = 0;
                Dictionary<long, string> accs = new Dictionary<long, string>();
                for (int i = 0; i < this.AdhocRequest.BulkRequestDetails.Count; i++)
                {
                    var item = this.AdhocRequest.BulkRequestDetails[i];
                    userid = 0;
                    accountName = string.Empty;
                    userid = item.UserId;
                    Label lblAcountName = null;
                    DropDownList drpAccount = null;
                    if (this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        lblAcountName = accNameRepeater.Items[i].FindControl("lblAcountName") as Label;
                        drpAccount = accNameRepeater.Items[i].FindControl("dllFlinsAccount") as DropDownList;

                        if (drpAccount == null || drpAccount.Items.Count == 0)
                        {
                            ShowAlertMessage(AlertType.Error, $"Flins account not found for user:{lblAcountName?.Text}");
                            return false;
                        }
                        if (drpAccount.Items.Count > 1 && drpAccount.SelectedIndex == 0)
                        {
                            ShowAlertMessage(AlertType.Error, $"Please select Flins account for user:{lblAcountName?.Text}");
                            return false;
                        }

                        accountName = drpAccount.SelectedItem.Text;
                    }
                    else
                    {
                        lblAcountName = AccountRepeater.Items[i].FindControl("lblAcountName") as Label;
                        var radAccount = AccountRepeater.Items[i].FindControl("ddlAccounts") as RadComboBox;

                        if (radAccount == null || radAccount.Items.Count == 0)
                        {
                            ShowAlertMessage(AlertType.Error, $"Flins account not found for user:{lblAcountName?.Text}");
                            return false;
                        }
                        if (radAccount.Items.Count > 1 && radAccount.SelectedIndex == 0)
                        {
                            ShowAlertMessage(AlertType.Error, $"Please select Flins account for user:{lblAcountName?.Text}");
                            return false;
                        }

                        accountName = radAccount.SelectedItem.Text;
                    }

                    if (userid == 0 || string.IsNullOrWhiteSpace(accountName))
                    {
                        ShowAlertMessage(AlertType.Error, $"Account detail doesnt exists for user:{item.AccountName}");
                        return false;
                    }
                    accs.Add(userid, accountName);

                }
                foreach (var item in accs)
                {
                    AdGroupsExixst = DomainApiClient<Dto.SAPSRMMainRoleMatrix>.GetByAction<bool>(DomainApiAction.Endpoint.SRM.SAPSRMMainRoleMatrix.IsAdGroupsExist,
                     new Dictionary<string, string> {
                                { "RoleName", this.SelectedBusinessRole },
                                {"actionType", ((int)AdhocRequest.AdhocActionType).ToString() },
                                {"accountType", ((int)AccountType.User).ToString() },
                                {"DIUserID",item.Key.ToString()  },
                                {"AccountName",item.Value  }
                     });
                    if (!AdGroupsExixst)
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, $"User {item.Value} AD account is required for selected role({this.SelectedBusinessRole}).");
                        return false;
                    }
                }
               
                //var AdGroupsExixst = DomainApiClient<Dto.SAPSRMMainRoleMatrix>.GetByAction<bool>(DomainApiAction.Endpoint.SRM.SAPSRMMainRoleMatrix.IsAdGroupsExist,
                //     new Dictionary<string, string> { { "RoleName", this.SelectedBusinessRole } });
                //if (AdGroupsExixst)
                //{
                   
                //    var users = AdhocRequest.BulkRequestDetails.Select(item => item.UserId).ToList();
                //    List<long> invalidUsers = new List<long>();
                //    users.ForEach(item =>
                //    {
                //        bool doesExist = DomainApiClient<Dto.ADSUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.DoesExistActiveADSUserInfoByAccountType,
                //           new Dictionary<string, string> { { "userId", item.ToString() }, { "accounttype", Convert.ToString((int)AccountType.User) } });


                       
                //        if (doesExist==false)
                //        {
                //            invalidUsers.Add(item);
                //        }
                //    });
                   
                //    List<string> InvalidUserNames = new List<string>();
                //    invalidUsers.ForEach(item =>
                //    {
                //        var usr = DomainApiClient<Dto.User>.GetById(item);
                //        if (usr != null)
                //        {
                //            InvalidUserNames.Add(usr.DisplayName);
                //        }
                //    });
                //    var errorMessage = "For the Following user(s) ";
                //    if (InvalidUserNames.Count > 0)
                //    {
                //        string usrs = string.Join(",", InvalidUserNames);
                //        errorMessage += usrs + " does not hold AD account";
                //        ShowAlertMessage(Infrastructure.AlertType.Error, errorMessage); 
                //        return false;
                //    }

                //}

                return AdGroupsExixst;
            }
            return false;
        }

        public override bool Validate()
        {
            return ValidateRoles() && UpdateBulkReqAccountDetail() && UpdateBulkReqAccountNameDetail();
        }

        private bool ValidateRoles()
        {
            if(SRMRoles.Count == 0 && this.AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                ShowAlertMessage(Infrastructure.AlertType.Warning, Constants.addToGridWarning);
                return false;
            }
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (accNameRepeater.Items.Count == 0)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, "No Account(s) found");
                    return false;
                }
                foreach (GridDataItem dataItem in accNameRepeater.Items)
                {
                    if (dataItem.ItemType == GridItemType.AlternatingItem || dataItem.ItemType == GridItemType.Item)
                    {
                        var ddlFlinslist = dataItem.FindControl("dllFlinsAccount") as DropDownList;
                        
                        if (ddlFlinslist != null && ddlFlinslist.SelectedIndex == -1)
                        {
                            ShowAlertMessage(Infrastructure.AlertType.Error, "Please select the Account.");
                            return false;
                        }
                        var SelectedAccount = ddlFlinslist.SelectedItem.Text;
                        AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.UserName == ddlFlinslist.SelectedItem.Text).AccountName = SelectedAccount;
                    }
                }
                return true;
            }
            return true;
        }

        protected override void SetEntitlementDetail()
        { 
            AdHocSAPSRMRequest srmRequest = new AdHocSAPSRMRequest();

            if (AdhocRequest != null)
            {
                var list = SRMRoles;
                srmRequest.AdHocSAPSRMRequestDetails = list;
                AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(srmRequest);
            }
            ClearFields(); 
        }

        protected void PGrpComboBox_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (PGrpComboBox.CheckedItems.Count > 0)
            {
                btnAdd.Enabled = true;
            }
            else
            {
                //btnAdd.Enabled = false;
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
                    SRMRoles = new List<AdHocSAPSRMRequestDetail>();
                    SRMObjGrid.Visible = false;
                    //btnAdd.Enabled = false;
                }
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
        protected void BusinessRoleAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (BusinessRoleAutoCompleteBox.Entries.Count > 0)
            {
                if (BusinessRoleAutoCompleteBox.Entries.Count > 1)
                {
                    BusinessRoleAutoCompleteBox.Entries.Remove(BusinessRoleAutoCompleteBox.Entries[1]);
                }
                BindPorgs();
            }
        }
        protected void BusinessRoleAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

            if (BusinessRoleAutoCompleteBox.Entries.Count == 0)
            {
                ddlPOrg.Items.Clear();
                PGrpComboBox.Items.Clear();
                //btnAdd.Enabled = false;
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
                ddlPOrg.Enabled = true;
                PGrpComboBox.Enabled = true;
                //btnAdd.Enabled = false;
                BindPorgs();
            }
        }
        
    }
}